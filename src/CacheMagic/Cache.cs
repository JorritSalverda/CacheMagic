using System;
using System.Web;
using JitterMagic;
using RetryMagic;

namespace CacheMagic
{
    /// <summary>
    /// Class to cache any values from slow responding sources in memory
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// The default duration to cache a value for
        /// </summary>
        public static int DefaultCacheDurationInSeconds { get; set; }

        /// <summary>
        /// Determines whether to function to call for a cache miss is wrapped in a retry mechanism
        /// </summary>
        public static bool WrapInRetry { get; set; }

        /// <summary>
        /// Sets the defaults for the public properties
        /// </summary>
        static Cache()
        {
            DefaultCacheDurationInSeconds = 60;
            WrapInRetry = true;
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using the <see cref="DefaultCacheDurationInSeconds"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the object</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique</param>
        /// <param name="functionToCallOnCacheMiss">The function to call when the value is not in cache</param>
        /// <returns>The value from either cache or the function that is called to fill the cache</returns>
        public static T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss)
        {
            return Get(cacheKey, functionToCallOnCacheMiss, DefaultCacheDurationInSeconds);
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using <see cref="expirationInSeconds"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the object</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique</param>
        /// <param name="functionToCallOnCacheMiss">The function to call when the value is not in cache</param>
        /// <param name="expirationInSeconds">The expiration time for this cache key</param>
        /// <returns>The value from either cache or the function that is called to fill the cache</returns>
        public static T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss, int expirationInSeconds)
        {
            // get object from cache
            CachedObject<T> objectFromCache = HttpContext.Current.Cache.Get(cacheKey) as CachedObject<T>;

            if (objectFromCache == null)
            {
                // not in cache, retrieve from the source and store in cache
                if (WrapInRetry)
                {
                    objectFromCache = new CachedObject<T>(Retry.Function(functionToCallOnCacheMiss));
                }
                else
                {
                    objectFromCache = new CachedObject<T>(functionToCallOnCacheMiss.Invoke());    
                }

                HttpContext.Current.Cache.Insert(cacheKey, objectFromCache, null, DateTime.Now.Add(TimeSpan.FromSeconds(Jitter.Apply(expirationInSeconds))), TimeSpan.Zero);
            }

            return objectFromCache.Value;
        }
    }
}