﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Enums;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Repositories
{
    public class LogRepository<TDbContext> : ILogRepository
        where TDbContext : DbContext, IAdminLogDbContext
    {
        protected readonly TDbContext DbContext;

        public LogRepository(TDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
        {
            var logsToDelete = await DbContext.Logs.Where(x => x.TimeStamp < deleteOlderThan.Date).ToListAsync();

            if(logsToDelete.Count == 0) return;

            this.DbContext.Logs.RemoveRange(logsToDelete);

            await AutoSaveChangesAsync();
        }

        public virtual async Task<PagedList<Log>> GetLogsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Log>();
            Expression<Func<Log, bool>> searchCondition = x => x.LogEvent.Contains(search) || x.Message.Contains(search) || x.Exception.Contains(search);
            var logs = await DbContext.Logs
                .WhereIf(!string.IsNullOrEmpty(search), searchCondition)                
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(logs);
            pagedList.PageSize = pageSize;
            pagedList.TotalCount = await DbContext.Logs.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();

            return pagedList;
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public bool AutoSaveChanges { get; set; } = true;
    }
}