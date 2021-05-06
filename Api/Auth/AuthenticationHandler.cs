using Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var userName = Request.Headers["username"].ToString();
            if (String.IsNullOrWhiteSpace(userName))
                return AuthenticateResult.Fail("No username");
            var password = Request.Headers["password"].ToString();
            if (String.IsNullOrWhiteSpace(password))
                return AuthenticateResult.Fail("No password");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
            var loginResult = await _signInManager
                .PasswordSignInAsync(user, password, false, false);
            if (!loginResult.Succeeded)
                return AuthenticateResult.Fail("Login failed");

            var identity = new ClaimsIdentity(Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
