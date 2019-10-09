using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2;
using Microsoft.AspNetCore.Authentication;
using Sustainsys.Saml2.Saml2P;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = Saml2Defaults.Scheme;
                //sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            })
            .AddSaml2(Saml2Defaults.Scheme, options =>
            {
                options.SPOptions.EntityId = new EntityId("api://0a4f3144-0cf7-4734-bbf9-54b38afd04dc");
                options.SPOptions.ReturnUrl = new Uri($"https://localhost:44358/");

                var idp =
                    new IdentityProvider(new EntityId("https://sts.windows.net/72f988bf-86f1-41af-91ab-2d7cd011db47/"), options.SPOptions)
                    {
                        LoadMetadata = true,
                        AllowUnsolicitedAuthnResponse = true,
                        MetadataLocation = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/federationmetadata/2007-06/federationmetadata.xml",
                        SingleSignOnServiceUrl = new Uri("https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/saml2"),
                    };
                options.IdentityProviders.Add(idp);
            })
            // ++WS-Federation
            //.AddWsFederation(options =>
            //{
            //    options.Wtrealm = "api://0a4f3144-0cf7-4734-bbf9-54b38afd04dc";
            //    options.MetadataAddress = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/federationmetadata/2007-06/federationmetadata.xml";
            //    options.Events.OnMessageReceived = ctx =>
            //    {
            //        string userName = null;
            //        var token = ctx.ProtocolMessage.GetToken();
            //        var cache = services.BuildServiceProvider().GetService<IDistributedCache>();

            //        using (MemoryStream strm = new MemoryStream(Encoding.UTF8.GetBytes(token)))
            //        {
            //            XmlReader reader = XmlReader.Create(strm);
            //            while (reader.Read())
            //            {
            //                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Attribute")
            //                {
            //                    if (reader.GetAttribute("Name") == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
            //                    {
            //                        reader.Read();
            //                        reader.Read();
            //                        userName = reader.Value;
            //                        break;
            //                    }
            //                }
            //            }
            //        }

            //        if (!String.IsNullOrEmpty(userName))
            //        {
            //            if (ctx.ProtocolMessage.IsSignInMessage)
            //            {
            //                var wr = ctx.ProtocolMessage.Wresult;
            //                TokenCache.AddToken(userName, wr);
            //            }
            //            else if (ctx.ProtocolMessage.IsSignOutMessage)
            //            {
            //                TokenCache.RemoveToken(userName);
            //            }
            //        }

            //        return Task.FromResult(0);
            //    };
            //    options.Events.OnSecurityTokenValidated = ctx =>
            //    {
            //        var token = TokenCache.GetToken(ctx.Principal.Identity.Name);
            //        return Task.CompletedTask;
            //    };
            //    options.Events.OnSecurityTokenReceived = ctx =>
            //    {
            //        return Task.CompletedTask;
            //    };
            //})
            .AddCookie();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Needed for SAML-P token caching. Not needed if using WS-Federation.
            app.UseSamlResponseCache();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}