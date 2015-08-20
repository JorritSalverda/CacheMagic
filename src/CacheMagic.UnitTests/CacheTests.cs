using System;
using System.Web;
using Http.TestLibrary;
using Xunit;

namespace CacheMagic.UnitTests
{
    public class CacheTests
    {
        public class UpdateSettings
        {
            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.UpdateSettings(null));
            }

            [Fact]
            public void Updates_Settings_Property()
            {
                // act
                Cache.UpdateSettings(new CacheSettings(cacheDurationInSeconds: 25));

                Assert.Equal(25, Cache.Settings.CacheDurationInSeconds);
            }
        }

        public class GetWithoutSettings
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
                Assert.Throws<ArgumentNullException>(() => Cache.Get(null, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(string.Empty, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(" ", () => "value from slow system"));
            }
        }

        public class GetWithSettings
        {
            [Fact]
            public void Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                using (new HttpSimulator("/", @"c:\inetpub\").SimulateRequest())
                {
                    // act
                    var result = Cache.Get("keyname", () => "value from slow system", new CacheSettings());

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
                    var result = Cache.Get("keyname2", () => (string)null, new CacheSettings());

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
                    var result = Cache.Get("keyname3", () => "some other value by now", new CacheSettings());

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
                    var result = Cache.Get("keyname4", () => "some other value by now", new CacheSettings());

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
                    Cache.Get("keyname", () => "value from slow system", new CacheSettings());

                    CachedObject<string> objectFromCache = HttpContext.Current.Cache["CacheMagic_keyname"] as CachedObject<string>;
                    Assert.Equal("value from slow system", objectFromCache.Value);
                }
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(null, () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(string.Empty, () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(" ", () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get("keyname5", () => "value from slow system", null));
            }
        }
    }
}
