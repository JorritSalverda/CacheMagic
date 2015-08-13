using System;
using System.Web;
using Http.TestLibrary;
using Xunit;

namespace CacheMagic.UnitTests
{
    public class CacheTests
    {
        public class Get
        {
            [Fact]
            public void Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    // act
                    var result = Cache.Get("keyname", () => "value from slow system");

                    Assert.Equal("value from slow system", result);
                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname"] as CachedObject<string>;
                    Assert.Equal("value from slow system", objectFromCache.Value);
                }
            }

            [Fact]
            public void Returns_Null_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    // act
                    var result = Cache.Get("keyname2", () => (string)null);

                    Assert.Equal(null, result);
                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname2"] as CachedObject<string>;
                    Assert.Equal(null, objectFromCache.Value);
                }
            }

            [Fact]
            public void Returns_NonNull_Value_From_Cache_If_It_Exists_In_Cache()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    Cache.Get("keyname3", () => "value from slow system");

                    // act
                    var result = Cache.Get("keyname3", () => "some other value by now");

                    Assert.Equal("value from slow system", result);
                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname3"] as CachedObject<string>;
                    Assert.Equal("value from slow system", objectFromCache.Value);
                }
            }

            [Fact]
            public void Returns_Null_Value_From_Cache_If_It_Exists_In_Cache()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    Cache.Get("keyname4", () => (string)null);

                    // act
                    var result = Cache.Get("keyname4", () => "some other value by now");

                    Assert.Equal(null, result);
                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname4"] as CachedObject<string>;
                    Assert.Equal(null, objectFromCache.Value);
                }
            }

            [Fact]
            public void Prepends_CacheMagic_Prefix_To_AspNet_Cache_Key()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    // act
                    Cache.Get("keyname", () => "value from slow system");

                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname"] as CachedObject<string>;
                    Assert.Equal("value from slow system", objectFromCache.Value);
                }
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(null, () => "value from slow system", 60, true, 8, 32, true, 16, 25, 25));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(string.Empty, () => "value from slow system", 60, true, 8, 32, true, 16, 25, 25));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(" ", () => "value from slow system", 60, true, 8, 32, true, 16, 25, 25));
            }

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_CacheDurationInSeconds_Is_Zero_Or_Less()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 0, true, 8, 32, true, 16, 25, 25));
            }              

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_RetryMaximumNumberOfAttempts_Is_Zero_Or_Less()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 0, 32, true, 16, 25, 25));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_RetryMillisecondsPerSlot_Is_Zero_Or_Less()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 0, true, 16, 25, 25));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_RetryMaximumNumberOfSlotsWhenTruncated_Is_Zero_Or_Less()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 32, true, 0, 25, 25));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_RetryJitterPercentage_Is_Less_Than_Zero()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 32, true, 16, -1, 25));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_RetryJitterPercentage_Is_One_Hundred_Or_Greater()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 32, true, 16, 100, 25));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_CacheDurationJitterPercentage_Is_Less_Than_Zero()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 32, true, 16, 25, -1));
            } 

            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_CacheDurationJitterPercentage_One_Hundred_Or_Greater()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname", () => "value from slow system", 60, true, 8, 32, true, 16, 25, 100));
            } 
        }
    }
}
