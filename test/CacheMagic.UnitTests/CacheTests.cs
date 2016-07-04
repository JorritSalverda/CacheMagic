using System;
using Microsoft.Extensions.Caching.Memory;
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
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                var result = Cache.Get(memoryCache, "keyname", () => "value from slow system");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                var result = Cache.Get(memoryCache, "keyname2", () => (string)null);

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname2") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Returns_NonNull_Value_From_Cache_If_It_Exists_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
                Cache.Get(memoryCache, "keyname3", () => "value from slow system");

                // act
                var result = Cache.Get(memoryCache, "keyname3", () => "some other value by now");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname3") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_From_Cache_If_It_Exists_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
                Cache.Get(memoryCache, "keyname4", () => (string)null);

                // act
                var result = Cache.Get(memoryCache, "keyname4", () => "some other value by now");

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname4") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Prepends_CacheMagic_Prefix_To_AspNet_Cache_Key()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                Cache.Get(memoryCache, "keyname", () => "value from slow system");

                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Throws_ArgumentNullException_If_MemoryCache_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(null, "keyname5", () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Null()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, null, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
                
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, string.Empty, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
                
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, " ", () => "value from slow system"));
            }
        }

        public class GetWithSettings
        {
            [Fact]
            public void Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                var result = Cache.Get(memoryCache, "keyname", () => "value from slow system", new CacheSettings());

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                var result = Cache.Get(memoryCache, "keyname2", () => (string)null, new CacheSettings());

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname2") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Returns_NonNull_Value_From_Cache_If_It_Exists_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                Cache.Get(memoryCache, "keyname3", () => "value from slow system");

                // act
                var result = Cache.Get(memoryCache, "keyname3", () => "some other value by now", new CacheSettings());

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname3") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_From_Cache_If_It_Exists_In_Cache()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                Cache.Get(memoryCache, "keyname4", () => (string)null);

                // act
                var result = Cache.Get(memoryCache, "keyname4", () => "some other value by now", new CacheSettings());

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname4") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Prepends_CacheMagic_Prefix_To_AspNet_Cache_Key()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act
                Cache.Get(memoryCache, "keyname", () => "value from slow system", new CacheSettings());

                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Throws_ArgumentNullException_If_MemoryCache_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(null, "keyname5", () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Null()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, null, () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, string.Empty, () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, " ", () => "value from slow system", new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

                // act + assert
                Assert.Throws<ArgumentNullException>(() => Cache.Get(memoryCache, "keyname6", () => "value from slow system", null));
            }
        }
    }
}
