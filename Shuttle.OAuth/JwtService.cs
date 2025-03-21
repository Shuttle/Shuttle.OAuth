using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shuttle.Core.Contract;

namespace Shuttle.OAuth;

public class JwtService : IJwtService
{
    private static readonly MemoryCache Cache = new(new MemoryCacheOptions());
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonWebTokenHandler _jwtHandler = new();
    private readonly OAuthOptions _oauthOptions;

    public JwtService(IOptions<OAuthOptions> oauthOptions, IHttpClientFactory httpClientFactory)
    {
        _oauthOptions = Guard.AgainstNull(Guard.AgainstNull(oauthOptions).Value);
        _httpClientFactory = Guard.AgainstNull(httpClientFactory);
    }

    public async ValueTask<string> GetIdentityNameAsync(string token)
    {
        var jsonToken = _jwtHandler.ReadJsonWebToken(Guard.AgainstNullOrEmptyString(token));
        var options = GetOptions(jsonToken);

        if (options == null)
        {
            return string.Empty;
        }

        Claim? claim = null;

        foreach (var identityNameClaimType in options.Issuer.IdentityNameClaimTypes)
        {
            claim = jsonToken.Claims.FirstOrDefault(item => item.Type.Equals(identityNameClaimType, StringComparison.InvariantCultureIgnoreCase));

            if (claim != null)
            {
                break;
            }
        }

        return await ValueTask.FromResult(claim?.Value ?? string.Empty);
    }

    public async Task<TokenValidationResult> ValidateTokenAsync(string token)
    {
        var jwt = _jwtHandler.ReadJsonWebToken(Guard.AgainstNullOrEmptyString(token));
        var options = GetOptions(jwt);

        if (options == null)
        {
            return new()
            {
                Exception = new InvalidOperationException(string.Format(Resources.OAuthProviderOptionsIssuerNotFoundException, jwt.Issuer, string.Join(',', jwt.Audiences ?? Enumerable.Empty<string>())))
            };
        }

        var keys = await GetSigningKeysAsync(options);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = options.Issuer.Uri,
            ValidateAudience = options.Issuer.Audiences.Any(),
            ValidAudiences = options.Issuer.Audiences,
            ValidateLifetime = true,
            ClockSkew = options.Issuer.ClockSkew,
            IssuerSigningKeys = keys
        };

        return await _jwtHandler.ValidateTokenAsync(token, validationParameters);
    }

    private OAuthProviderOptions? GetOptions(JsonWebToken jwt)
    {
        return _oauthOptions.Providers.FirstOrDefault(item =>
            item.Issuer.Uri.Equals(jwt.Issuer, StringComparison.CurrentCultureIgnoreCase) &&
            (
                !item.Issuer.Audiences.Any() ||
                item.Issuer.Audiences.Intersect(jwt.Audiences).Any()
            ));
    }

    private async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync(OAuthProviderOptions options)
    {
        if (Cache.TryGetValue(options.Issuer.JwksUri, out IEnumerable<SecurityKey>? cachedKeys))
        {
            return cachedKeys!;
        }

        var httpClient = _httpClientFactory.CreateClient("Shuttle.OAuth");
        var response = await httpClient.GetAsync(options.Issuer.JwksUri);

        var cacheControlHeader = response.Headers.CacheControl;
        var cacheDuration = cacheControlHeader?.MaxAge.HasValue == true
            ? cacheControlHeader.MaxAge.Value
            : options.Issuer.SigningKeyCacheDuration;

        var jwksContent = await response.Content.ReadAsStringAsync();
        var jwks = JsonDocument.Parse(jwksContent);
        var keys = new List<SecurityKey>();

        foreach (var key in jwks.RootElement.GetProperty("keys").EnumerateArray())
        {
            var e = Base64UrlEncoder.DecodeBytes(key.GetProperty("e").GetString());
            var n = Base64UrlEncoder.DecodeBytes(key.GetProperty("n").GetString());
            var rsa = new RSAParameters { Exponent = e, Modulus = n };
            keys.Add(new RsaSecurityKey(rsa));
        }

        Cache.Set(options.Issuer.JwksUri, keys, cacheDuration);

        return keys;
    }
}