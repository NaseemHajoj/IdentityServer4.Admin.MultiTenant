using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly PasswordOptions options;

        public PasswordGenerator(IOptions<IdentityOptions> identityOptions) 
        {
            this.options = identityOptions.Value.Password;

            if (this.options == null)
            {
                this.options = new PasswordOptions
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };
            }
        }

        public string GeneratePassword()
        {

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
            };

            List<char> chars = new List<char>();

            if (this.options.RequireUppercase)
            {
                chars.Insert(RandomNumberGenerator.GetInt32(0, chars.Count + 1),
                    randomChars[0][RandomNumberGenerator.GetInt32(0, randomChars[0].Length)]);
            }

            if (this.options.RequireLowercase)
            {
                chars.Insert(RandomNumberGenerator.GetInt32(0, chars.Count + 1),
                    randomChars[1][RandomNumberGenerator.GetInt32(0, randomChars[1].Length)]);
            }

            if (this.options.RequireDigit)
            {
                chars.Insert(RandomNumberGenerator.GetInt32(0, chars.Count + 1),
                    randomChars[2][RandomNumberGenerator.GetInt32(0, randomChars[2].Length)]);
            }

            if (this.options.RequireNonAlphanumeric)
            {
                chars.Insert(RandomNumberGenerator.GetInt32(0, chars.Count + 1),
                    randomChars[3][RandomNumberGenerator.GetInt32(0, randomChars[3].Length)]);
            }

            for (int i = chars.Count; i < this.options.RequiredLength
                || chars.Distinct().Count() < this.options.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[RandomNumberGenerator.GetInt32(0, randomChars.Length + 1)];
                chars.Insert(RandomNumberGenerator.GetInt32(0, chars.Count + 1),
                    rcs[RandomNumberGenerator.GetInt32(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
