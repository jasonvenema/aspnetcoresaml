#pragma checksum "C:\Users\javene\source\repos\aspnetcoresaml\WebApplication1\Views\Account\LogOut.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8b989b5b55049c4939d4f7e034b13db27dc9d344"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Account_LogOut), @"mvc.1.0.view", @"/Views/Account/LogOut.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Account/LogOut.cshtml", typeof(AspNetCore.Views_Account_LogOut))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\Users\javene\source\repos\aspnetcoresaml\WebApplication1\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#line 2 "C:\Users\javene\source\repos\aspnetcoresaml\WebApplication1\Views\_ViewImports.cshtml"
using WebApplication1;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8b989b5b55049c4939d4f7e034b13db27dc9d344", @"/Views/Account/LogOut.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e2b9577d0fdcda3d9d98b0016b39511396f4c6ba", @"/Views/_ViewImports.cshtml")]
    public class Views_Account_LogOut : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "C:\Users\javene\source\repos\aspnetcoresaml\WebApplication1\Views\Account\LogOut.cshtml"
  
    ViewData["Title"] = "Signed out";

#line default
#line hidden
            BeginContext(46, 6, true);
            WriteLiteral("\r\n<h2>");
            EndContext();
            BeginContext(53, 17, false);
#line 5 "C:\Users\javene\source\repos\aspnetcoresaml\WebApplication1\Views\Account\LogOut.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(70, 57, true);
            WriteLiteral("</h2>\r\n<p>\r\n    You have successfully signed out.\r\n</p>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591