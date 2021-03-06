﻿using System;
using Microsoft.Extensions.Caching.Memory;
using Http.TestLibrary;
using Xunit;

namespace CacheMagic.UnitTests
{
    public class CacheInstanceTests
    {
        public class Constructor
        {
            [Fact]
            public void Throws_ArgumentNullException_If_CacheMemory_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new CacheInstance(null, new CacheSettings()));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new CacheInstance(new MemoryCache(new MemoryCacheOptions()), null));
            }
        }

        public class UpdateSettings
        {
            private readonly ICacheInstance instance;

            public UpdateSettings()
            {
                instance = new CacheInstance(new MemoryCache(new MemoryCacheOptions()), new CacheSettings(cacheDurationInSeconds: 45));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_Settings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => instance.UpdateSettings(null));
            }

            [Fact]
            public void Updates_Settings_Property()
            {
                // act
                instance.UpdateSettings(new CacheSettings(cacheDurationInSeconds: 25));

                Assert.Equal(25, instance.Settings.CacheDurationInSeconds);
            }
        }

        public class Get
        {
            private readonly ICacheInstance instance;
            private readonly IMemoryCache memoryCache; 

            public Get()
            {
                memoryCache = new MemoryCache(new MemoryCacheOptions());
                instance = new CacheInstance(memoryCache, new CacheSettings(cacheDurationInSeconds: 45));
            }

            [Fact]
            public void Returns_NonNull_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                // act
                var result = instance.Get("keyname", () => "value from slow system");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_And_Stores_It_In_Cache_If_It_Does_Not_Exist_In_Cache()
            {
                // act
                var result = instance.Get("keyname2", () => (string)null);

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname2") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Returns_NonNull_Value_From_Cache_If_It_Exists_In_Cache()
            {
                instance.Get("keyname3", () => "value from slow system");

                // act
                var result = instance.Get("keyname3", () => "some other value by now");

                Assert.Equal("value from slow system", result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname3") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Returns_Null_Value_From_Cache_If_It_Exists_In_Cache()
            {
                instance.Get("keyname4", () => (string)null);

                // act
                var result = instance.Get("keyname4", () => "some other value by now");

                Assert.Equal(null, result);
                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname4") as CachedObject<string>;
                Assert.Equal(null, objectFromCache.Value);
            }

            [Fact]
            public void Prepends_CacheMagic_Prefix_To_AspNet_Cache_Key()
            {
                // act
                instance.Get("keyname", () => "value from slow system");

                CachedObject<string> objectFromCache = memoryCache.Get("CacheMagic_keyname") as CachedObject<string>;
                Assert.Equal("value from slow system", objectFromCache.Value);
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => instance.Get(null, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_Empty_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => instance.Get(string.Empty, () => "value from slow system"));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_CacheKey_Is_WhiteSpace_String()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => instance.Get(" ", () => "value from slow system"));
            }
        }
    }
}
