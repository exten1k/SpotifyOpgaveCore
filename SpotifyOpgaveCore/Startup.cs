/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SpotifyOpgaveCore.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotifyOpgaveCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDataProtection();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })

            .AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = async context =>
                    {
                        if (context.Principal.Identity.IsAuthenticated)
                        {
                            var tokens = context.Properties.GetTokens();
                            var refreshToken = tokens.FirstOrDefault(t => t.Name == "refresh_token");
                            var accessToken = tokens.FirstOrDefault(t => t.Name == "access_token");
                            var exp = tokens.FirstOrDefault(t => t.Name == "expires_at");
                            var expires = DateTime.Parse(exp.Value);
                            if (expires < DateTime.Now)
                            {
                                var tokenEndpoint = "https://accounts.spotify.com/api/token";
                                string clientId = "49d56ad20adc4096a2e082401e817dcf";
                                string clientSecret = "f11e461115a043769bfea0fc56384aee";
                                var tokenClient = new TokenClient(tokenEndpoint, clientId, clientSecret);
                                var tokenResponse = tokenClient.RequestRefreshTokenAsync(refreshToken.Value).Result;
                                if (tokenResponse.IsError)
                                {
                                    context.RejectPrincipal();
                                    await Task.CompletedTask;
                                }
                                accessToken.Value = tokenResponse.AccessToken;
                                var newExpires = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
                                exp.Value = newExpires.ToString("o", CultureInfo.InvariantCulture);
                                context.Properties.StoreTokens(tokens);
                                context.ShouldRenew = true;
                                await Task.CompletedTask;
                            }
                        }
                        await Task.CompletedTask;

                    }
                };
            })

            .AddSpotify(options =>
            {
                options.ClientId = "49d56ad20adc4096a2e082401e817dcf";
                options.ClientSecret = "f11e461115a043769bfea0fc56384aee";
                options.SaveTokens = true;
                options.Scope.Add("playlist-modify-private");
                options.Scope.Add("streaming");
                options.Scope.Add("user-read-playback-state");
                options.Scope.Add("user-read-currently-playing");

                options.UserInformationEndpoint = "https://api.spotify.com/v1/me";

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        HttpResponseMessage response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        context.Properties.IsPersistent = true;
                        context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);

                        var user = JObject.Parse(await response.Content.ReadAsStringAsync());

                        var name = user.Value<string>("display_name");
                        if (!string.IsNullOrEmpty(name))
                        {
                            context.Identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                        }
                    }
                };
            });


            services.AddMvc();
            var connection = @"Data Source=nl1-wsq1.a2hosting.com; Initial Catalog=spotifya_spot; User ID=spotifya_test; Password='Test123';";
            services.AddDbContext<RoomContext>
                (options => options.UseSqlServer(connection));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseMvcWithDefaultRoute();
        }
    }
}
