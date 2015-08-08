using System.Web;
using Http.TestLibrary;
using Xunit;

namespace CacheMagic.UnitTests
{
    public class CacheTests
    {
        [Fact]
        public void Get_Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                // act
                var result = Cache.Get("keyname", () => "value from slow system");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = HttpContext.Current.Cache["keyname"] as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }
        }

        [Fact]
        public void Get_Returns_Null_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                // act
                var result = Cache.Get("keyname2", () => null);

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = HttpContext.Current.Cache["keyname2"] as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }
        }

        [Fact]
        public void Get_Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Exists_In_Cache()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                Cache.Get("keyname3", () => "value from slow system", 60);

                // act
                var result = Cache.Get("keyname3", () => "some other value by now");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = HttpContext.Current.Cache["keyname3"] as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }
        }

        [Fact]
        public void Get_Returns_Null_Value_And_Stores_It_In_Cache_If_It_Exists_In_Cache()
        {
            using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
            {
                Cache.Get("keyname4", () => null, 60);

                // act
                var result = Cache.Get("keyname4", () => "some other value by now");

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = HttpContext.Current.Cache["keyname4"] as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }
        }
    }
}
