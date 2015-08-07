using System.Web;
using Http.TestLibrary;
using Xunit;

namespace CacheMagic.UnitTests
{
    public class CacheTests
    {
        [Fact]
        public void Get_Returns_Value_If_Exists_In_Cache()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                // act
                var result = Cache.Get("keyname", () => "value from slow system");

                Assert.Equal("value from slow system", result);
                Assert.Equal("value from slow system", HttpContext.Current.Cache["keyname"]);
            }
        }
    }
}
