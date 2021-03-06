using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Shouldly;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace jsreport.AspNetCore.Test
{
    [TestFixture]
    public class JsReportMiddlewareTest
    {
        [TearDown]
        public void TearDown()
        {
            new LocalReporting().KillRunningJsReportProcesses().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();           
        }
    
        [Test]
        public async Task BasicTest()
        {            
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

            var rd = new RequestDelegate(async (ctx) =>
            {
                context.JsReportFeature().Engine(Engine.None).Recipe(Recipe.Html);
                await ctx.Response.WriteAsync("Hello");                
            });

            var middleware = new JsReportMiddleware(rd, rs);

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            new StreamReader(context.Response.Body).ReadToEnd().ShouldBe("Hello");
        }
    }
}
