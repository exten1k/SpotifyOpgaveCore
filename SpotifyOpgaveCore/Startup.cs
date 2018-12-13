/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SpotifyOpgaveCore.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SpotifyOpgaveCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            })

            .AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
            })

            .AddSpotify(options =>
            {
                options.ClientId = "49d56ad20adc4096a2e082401e817dcf";
                options.ClientSecret = "f11e461115a043769bfea0fc56384aee";
                options.SaveTokens = true;
                options.Scope.Add("playlist-modify-private");
                options.UserInformationEndpoint = "https://api.spotify.com/v1/me";

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                        HttpResponseMessage response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

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
            var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNetCore.NewDb;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<RoomContext>
                (options => options.UseSqlServer(connection));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
