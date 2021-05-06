using Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Api.Auth
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ApplicationDbContext context,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, SignInManager<IdentityUser> signInManager)
            : base(options, logger, encoder, clock)
        {
            _context = context;
            _signInManager = signInManager;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (String.IsNullOrEmpty(Request.Headers["Authorization"].ToString()))
            {
                return AuthenticateResult.Fail("No authorization details provided.");
            }
            string[] userNameAndPassword = GetUserNameAndPassword();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userNameAndPassword[0]);
            var loginResult = await _signInManager
                .PasswordSignInAsync(user, userNameAndPassword[1], false, false);
            if (!loginResult.Succeeded)
                return AuthenticateResult.Fail("Login failed");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };
                        
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private string[] GetUserNameAndPassword()
        {
            var authorizationToken = Request.Headers["Authorization"].ToString().Split(" ");
            var decodeAuthToken = Encoding.UTF8.GetString(Convert.FromBase64String(authorizationToken[1]));
            var userNameAndPassword = decodeAuthToken.Split(":");
            return userNameAndPassword;
        }
    }
}
