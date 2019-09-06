using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Xml;
using System.IO;
using System.Text;
using System;
using WebApplication1.Util;

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
                sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
            })
            .AddWsFederation(options =>
            {
                options.SaveTokens = true;
                options.Wtrealm = "api://0a4f3144-0cf7-4734-bbf9-54b38afd04dc";
                options.MetadataAddress = "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/federationmetadata/2007-06/federationmetadata.xml";
                options.Events.OnMessageReceived = ctx =>
                {
                    string userName = null;
                    var token = ctx.ProtocolMessage.GetToken();

                    using (MemoryStream strm = new MemoryStream(Encoding.UTF8.GetBytes(token)))
                    {
                        XmlReader reader = XmlReader.Create(strm);
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Attribute")
                            {
                                if (reader.GetAttribute("Name") == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                                {
                                    reader.Read();
                                    reader.Read();
                                    userName = reader.Value;
                                    break;
                                }
                            }
                        }
                    }

                    if (ctx.ProtocolMessage.IsSignInMessage)
                    {
                        TokenCache.AddToken(userName, token);
                    }
                    else if (ctx.ProtocolMessage.IsSignOutMessage)
                    {
                        TokenCache.RemoveToken(userName);
                    }
                    return Task.FromResult(0);
                };
                options.Events.OnSecurityTokenValidated = ctx =>
                {
                    var token = ctx.SecurityToken as Saml2SecurityToken;
                    var assertion = token.Assertion;

                    var settings = new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true,
                        Indent = false,
                        Encoding = Encoding.UTF8
                    };

                    StringBuilder sb = new StringBuilder();
                    StringWriter stringWriter = new StringWriter(sb);
                    XmlWriter responseWriter = XmlWriter.Create(stringWriter, settings);
                    new Saml2Serializer().WriteAssertion(responseWriter, assertion);
                    responseWriter.Flush();
                    var assertionXml = sb.ToString();

                    using (FileStream fs = new FileStream("C:\\Temp\\assert.xml", FileMode.OpenOrCreate))
                    {
                        var bytes = Encoding.UTF8.GetBytes(assertionXml);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }

                    return Task.CompletedTask;
                };
            })
            .AddCookie();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}