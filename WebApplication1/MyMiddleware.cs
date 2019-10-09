using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WebApplication1.Util;

namespace WebApplication1
{
    public class SamlResponseCache
    {
        private const string SAML_RESPONSE_KEY = "SAMLResponse";
        private const string CLAIM_USER_NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        private readonly RequestDelegate _next;

        public SamlResponseCache(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // SAML response will arrive via POST
            if (String.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
            {
                var samlResponse = await ReadSaml2ResponseFromBody(context);
                AddSamlResponseToCache(samlResponse);
            }

            await this._next.Invoke(context).ConfigureAwait(false);
        }

        private static void AddSamlResponseToCache(string samlResponse)
        {
            string userName = default(string);
            using (MemoryStream strm = new MemoryStream(Encoding.UTF8.GetBytes(samlResponse)))
            {
                XmlReader reader = XmlReader.Create(strm);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Attribute")
                    {
                        if (reader.GetAttribute("Name") == CLAIM_USER_NAME)
                        {
                            reader.Read();
                            reader.Read();
                            userName = reader.Value;
                            break;
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(userName))
            {
                TokenCache.AddToken(userName, samlResponse);
            }
        }

        private static async Task<string> ReadSaml2ResponseFromBody(HttpContext context)
        {
            // Allow downstream middleware to read the stream by buffering our own copy
            context.Request.EnableBuffering();

            string saml2ResponseString = default(string);

            // Read the stream body
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                var body = await reader.ReadToEndAsync();

                // Check for a SAML response in the body
                if (body.Contains(SAML_RESPONSE_KEY))
                {
                    var bodyPairs = body.Split(new char[] { '&' });
                    foreach (var pair in bodyPairs)
                    {
                        var tuple = pair.Split(new char[] { '=' });
                        if (tuple.Length > 1)
                        {
                            if (String.Equals(tuple[0], SAML_RESPONSE_KEY, StringComparison.OrdinalIgnoreCase))
                            {
                                saml2ResponseString = DecodeSamlResponse(tuple[1]);
                            }
                        }
                    }
                }

                context.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            return saml2ResponseString;
        }

        private static string DecodeSamlResponse(string encodedResponse)
        {
            string saml2ResponseString;
            var saml2Decoded = HttpUtility.UrlDecode(encodedResponse);
            var saml2Response = Convert.FromBase64String(saml2Decoded);
            saml2ResponseString = Encoding.UTF8.GetString(saml2Response);
            return saml2ResponseString;
        }
    }

    public static class SamlResponseCacheExtensions
    {
        public static IApplicationBuilder UseSamlResponseCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SamlResponseCache>();
        }
    }
}
