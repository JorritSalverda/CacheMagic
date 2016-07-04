using Microsoft.Extensions.Caching.Memory;
using System;
using JitterMagic;
using RetryMagic;

namespace CacheMagic
{
    /// <summary>
    /// Class to cache any values from slow responding sources in asp.net in-memory cache
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// The default settings
        /// </summary>
        /// <value>The settings.</value>
        public static CacheSettings Settings { get; private set; }

        /// <summary>
        /// Sets the defaults for the public properties.
        /// </summary>
        static Cache()
        {
            Settings = new CacheSettings();
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public static void UpdateSettings(CacheSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            Settings = settings;
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using the default settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the object.</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique.</param>
        /// <param name="functionToCallOnCacheMiss">The function to call when the value is not in cache.</param>
        /// <returns>The value from either cache or the function that is called to fill the cache.</returns>
        public static T Get<T>(IMemoryCache memoryCache, string cacheKey, Func<T> functionToCallOnCacheMiss)
        {
            return Get(memoryCache, cacheKey, functionToCallOnCacheMiss, Settings);
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using passed in settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the object.</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique.</param>
        /// <param name="functionToCallOnCacheMiss">Function to call on cache miss.</param>
        /// <param name="settings">The settings object.</param>
        public static T Get<T>(IMemoryCache memoryCache, string cacheKey, Func<T> functionToCallOnCacheMiss, CacheSettings settings)
        {
            if (memoryCache == null)
            {
                throw new ArgumentNullException("memoryCache");            
            }
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentNullException("cacheKey");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");            
            }

            string prefixedCacheKey = "CacheMagic_" + cacheKey;

            // get object from cache
            CachedObject<T> objectFromCache = memoryCache.Get(prefixedCacheKey) as CachedObject<T>;

            if (objectFromCache == null)
            {
                // not in cache, retrieve from the source and store in cache
                if (settings.WrapInRetry)
                {
                    objectFromCache = new CachedObject<T>(Retry.Function(functionToCallOnCacheMiss, settings.RetrySettings));
                }
                else
                {
                    objectFromCache = new CachedObject<T>(functionToCallOnCacheMiss.Invoke());    
                }

                memoryCache.Set(prefixedCacheKey, objectFromCache, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(Jitter.Apply(settings.CacheDurationInSeconds, settings.JitterSettings))));            
            }

            return objectFromCache.Value;
        }


    }
}