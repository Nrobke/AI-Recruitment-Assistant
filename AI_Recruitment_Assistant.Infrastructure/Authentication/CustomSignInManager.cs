
using System.Security.Claims;
using AI_Recruitment_Assistant.Application.Abstractions.Authentication;
using AI_Recruitment_Assistant.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI_Recruitment_Assistant.Infrastructure.Authentication;

public class CustomSignInManager<TUser>(
    UserManager<TUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<TUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<TUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<TUser> confirmation,
    ITokenProvider tokenProvider,
    IServiceProvider serviceProvider) : SignInManager<TUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) where TUser : class
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IAuthenticationSchemeProvider _schemes = schemes;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private sealed class TwoFactorAuthenticationInfo
    {
        public required TUser User { get; init; }
        public string? LoginProvider { get; init; }
    }

    protected override async Task<SignInResult> SignInOrTwoFactorAsync(TUser user, bool isPersistent, string? loginProvider = null, bool bypassTwoFactor = false)
    {
        // Check if two-factor authentication is enabled
        if (!bypassTwoFactor && await IsTwoFactorEnabledAsync(user))
        {
            if (!await IsTwoFactorClientRememberedAsync(user))
            {
                // Store two-factor info for the user
                _ = new TwoFactorAuthenticationInfo
                {
                    User = user,
                    LoginProvider = loginProvider,
                };

                // If the two-factor scheme exists, store user info for later two-factor flow
                if (await _schemes.GetSchemeAsync(IdentityConstants.TwoFactorUserIdScheme) != null)
                {
                    var userId = await UserManager.GetUserIdAsync(user);
                    await Context.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, StoreTwoFactorInfo(userId, loginProvider));
                }

                return SignInResult.TwoFactorRequired;
            }
        }

        // Cleanup external cookie if using an external login provider
        if (loginProvider != null)
        {
            await Context.SignOutAsync(IdentityConstants.ExternalScheme);
        }

        if (loginProvider == null && user is User appUser)
        {
            var roles = await UserManager.GetRolesAsync(user);
            var tokenResult = await _tokenProvider.Create(appUser, roles);

            Context.Items["AccessTokenResult"] = tokenResult;
        }
        else
        {
            // Handle external logins if necessary
            await SignInAsync(user, isPersistent, loginProvider);
        }

        return SignInResult.Success;
    }
    internal static ClaimsPrincipal StoreTwoFactorInfo(string userId, string? loginProvider)
    {
        var identity = new ClaimsIdentity(IdentityConstants.TwoFactorUserIdScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, userId));
        if (loginProvider != null)
        {
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, loginProvider));
        }
        return new ClaimsPrincipal(identity);
    }
}

