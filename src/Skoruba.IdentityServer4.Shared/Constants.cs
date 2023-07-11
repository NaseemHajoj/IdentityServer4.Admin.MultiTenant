using System;

namespace Skoruba.IdentityServer4.Shared
{
    public static class Constants
    {
        /// <summary>
        /// Constants for the System tenant.
        /// </summary>
        public static class SystemTenant
        {
            /// <summary>
            /// The tenant ID of the system.
            /// </summary>
            public static readonly Guid Id = new Guid("48e6ecd3-d3a5-4da2-a53f-f5921784912a");

            public const string Name = "System";

            public const string DisplayName = "Advanced Packages Tracker System";

            public const string Email = "system@advancedpackagestrack.com";

            public const string Mobile = "+972508312025";

            public const string SystemUserId = "487eeea4-e470-419f-9bc3-c20e148b6a3a";

            public static readonly Guid LicenseKey = new Guid("D021DE7B-2B72-492C-B1D9-953FCB0D6C2B");
        }
    }
}