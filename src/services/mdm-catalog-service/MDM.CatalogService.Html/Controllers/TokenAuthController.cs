using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using MDM.CatalogService.Html.Authentication.External;
using MDM.CatalogService.Html.Authentication.JwtBearer;
using MDM.CatalogService.Html.Models.TokenAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MDMPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : MDMPlatformControllerBase
    {
        private readonly ITenantCache _tenantCache;
        private readonly TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;

        public TokenAuthController(
            ITenantCache tenantCache,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager)
        {
            _tenantCache = tenantCache;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }


        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken);
        }
    }
}
