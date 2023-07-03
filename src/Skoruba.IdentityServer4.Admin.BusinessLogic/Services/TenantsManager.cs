using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class TenantsManager<TUser> : ITenantsManager<TUser>
		where TUser : ApplicationUser<string>, new()
    {
		private readonly ITenantsRepository tenantsRepository;

		private readonly ApplicationUserManager<TUser> applicationUserManager;

		private readonly IMapper mapper;

		private readonly IAuditEventLogger auditEventLogger;

        private readonly IPasswordGenerator passwordGenerator;

		private readonly ILogger<TenantsManager<TUser>> logger;

        private readonly IEmailSender emailSender;

		public TenantsManager(
			ITenantsRepository tenantsRepository,
			ApplicationUserManager<TUser> applicationUserManager,
			IMapper mapper,
			IAuditEventLogger auditEventLogger,
            IPasswordGenerator passwordGenerator,
			ILogger<TenantsManager<TUser>> logger,
            IEmailSender emailSender) 
		{
			this.tenantsRepository = tenantsRepository;
			this.applicationUserManager = applicationUserManager;
            this.mapper = mapper;
			this.auditEventLogger = auditEventLogger;
            this.passwordGenerator = passwordGenerator;
			this.logger = logger;
            this.emailSender = emailSender;
		}

		public async Task<TenantDto> CreateNewTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            var tenant = this.mapper.Map<Tenant>(tenantDto);

            tenant = await this.tenantsRepository.CreateTenantAsync(tenant, cancellationToken);

			this.logger.LogDebug("New tenant was created with Id \"{TenantId}\" and Name \"{TenantName}\"", tenant.Id, tenant.Name);

			// get the newly created tenant dto
            TenantDto newlyCreatedTenant = this.mapper.Map<TenantDto>(tenant);

            var adminUser = new TUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "",
                Email = tenant.Email,
                EmailConfirmed = false,
                TenantId = tenant.Id
            };

            string password = this.passwordGenerator.GeneratePassword();
            var identityResult = await this.applicationUserManager.CreateAsync(adminUser, password);
            if (identityResult.Succeeded)
            {
                var user = await this.applicationUserManager.FindByIdAsync(adminUser.Id);

                string body = $"New tenant was created for you with name {tenant.Name} and Id {tenant.Id} \r\n"
                    + $"Admin: {adminUser.UserName} \r\n"
                    + $"Email: {adminUser.Email}      (Email needs to be confirmed)\r\n"
                    + $"Initial Admin Password: {password} (you will be requested to change this initial password once you log in) \r\n"
                    + "Thanks,\r\n"
                    + "Advanced Packages Tracker System";
                await emailSender.SendEmailAsync(user.Email, "New Tenant Created", body);

                await this.auditEventLogger.LogEventAsync(new TenantCreatedEvent<TenantDto>(newlyCreatedTenant));
            }

            return newlyCreatedTenant;
		}

		public async Task<TenantDto> UpdateTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            // build the tenant model
            var tenant = this.mapper.Map<Tenant>(tenantDto);

            // do the update
            tenant = await this.tenantsRepository.UpdateTenantAsync(tenant, cancellationToken);

            TenantDto updatedTenantDto = this.mapper.Map<TenantDto>(tenant);

            await this.auditEventLogger.LogEventAsync(new TenantUpdatedEvent<TenantDto>(tenantDto, updatedTenantDto));

            return updatedTenantDto;
        }

		public async Task DeleteTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            await this.tenantsRepository.DeleteTenantAsync(tenantDto.Id, cancellationToken);

            await this.auditEventLogger.LogEventAsync(new TenantDeletedEvent<TenantDto>(tenantDto));
        }

		public async Task<TenantDto> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
		{
			Tenant tenant = await this.tenantsRepository.GetTenantAsync(tenantId, cancellationToken);

			if (tenant == null)
			{
				this.logger.LogDebug("Tenant with id {TenantId} was not found", tenantId);
				return default;
			}

			var tenantDto = this.mapper.Map<TenantDto>(tenant);

			await this.auditEventLogger.LogEventAsync(new TenantRequestedEvent<TenantDto>(tenantDto));

			return tenantDto;
		}

		public async Task<TenantsDto> GetTenantsAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			PagedList<Tenant> pagedList = await this.tenantsRepository.GetTenantsAsync(search, page, pageSize, cancellationToken);
			
			var tenantsDto = this.mapper.Map<TenantsDto>(pagedList);

			await this.auditEventLogger.LogEventAsync(new UsersRequestedEvent<TenantsDto>(tenantsDto));

			return tenantsDto;
		}

        private async Task<(TUser, string)> CreateTenantAdmin(Tenant tenant)
        {
            // create the tenant admin user.
            var tenantAdmin = new TUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = tenant.Email,
                FirstName = "Admin",
                LastName = string.Empty,
                TenantId = tenant.Id
            };

            string password = this.passwordGenerator.GeneratePassword();

            IdentityResult result = await this.applicationUserManager.CreateAsync(tenantAdmin, password);

            if (!result.Succeeded)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Failed to create Admin user for tenant {tenant}.");
                errorMessage.AppendLine("Errors: ");
                errorMessage.AppendJoin(Environment.NewLine, result.Errors.Select(o => $"Code: {o.Code}    Description: {o.Description}."));

                throw new Exception(errorMessage.ToString());
            }

            tenantAdmin = await this.applicationUserManager.FindByEmailAsync(tenant.Email);
            result = await this.applicationUserManager.AddToRoleAsync(tenantAdmin, "tenant_admin");

            if (!result.Succeeded)
            {
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Failed to add Admin user for tenant {tenant} to admin role.");
                errorMessage.AppendLine("Errors: ");
                errorMessage.AppendJoin(Environment.NewLine, result.Errors.Select(o => $"Code: {o.Code}    Description: {o.Description}."));

                throw new Exception(errorMessage.ToString());
            }

            this.logger.LogInformation("Admin user was created successfully for tenant {tenant}.", tenant);

            return (tenantAdmin, password);
        }
    }
}
