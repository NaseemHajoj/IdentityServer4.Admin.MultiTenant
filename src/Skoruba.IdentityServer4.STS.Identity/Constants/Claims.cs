using IdentityModel;

namespace Skoruba.IdentityServer4.STS.Identity.Constants
{
	/// <summary>
	/// The claims supported by the IdentityServer
	/// </summary>
	public static class Claims
	{
		/// <summary>
		///  Claims Keys
		/// </summary>
		public static class Keys
		{
			/// <summary>
			/// The user fullname claim key.
			/// </summary>
			public const string FullName = JwtClaimTypes.Name;

			/// <summary>
			/// The user given\first name claim key.
			/// </summary>
			public const string GivenName = JwtClaimTypes.GivenName;

			/// <summary>
			/// The user family\last name claim key.
			/// </summary>
			public const string FamilyName = JwtClaimTypes.FamilyName;

			/// <summary>
			/// The user email claim key.
			/// </summary>
			public const string Email = JwtClaimTypes.Email;

			/// <summary>
			/// The user role claim key.
			/// </summary>
			public const string Role = JwtClaimTypes.Role;

			/// <summary>
			/// The user Id claim key.
			/// </summary>
			public const string UserId = "user_id";

			/// <summary>
			/// The tenant Id claim key.
			/// </summary>
			public const string TenantId = "tid";

			/// <summary>
			/// The "tid" claim is mapped to this key claim in asp.net core for some reason
			/// https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/issues/550
			/// </summary>
			public const string TenantIdStandard = "http://schemas.microsoft.com/identity/claims/tenantid";

			/// <summary>
			/// The authentication type claim key.
			/// </summary>
			public const string AuthType = "auth_type";

			/// <summary>
			/// The locale claim key.
			/// </summary>
			public const string Locale = "locale";
		}

		/// <summary>
		/// Claims Values Constants
		/// </summary>
		public static class Values
		{
			public const string ClientCredentialsAuthType = "client_credentials";

			public const string PasswordAuthType = "password";

			public const string DefaultLocal = "heb";
		}
	}
}
