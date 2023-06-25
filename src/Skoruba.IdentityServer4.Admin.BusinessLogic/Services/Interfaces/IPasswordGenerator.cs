namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
    public interface IPasswordGenerator
    {
        /// <summary>
        /// Generate a strong password.
        /// </summary>
        /// <returns></returns>
        string GeneratePassword();
    }
}