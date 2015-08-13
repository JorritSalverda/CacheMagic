using System;
using System.Web;
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
        /// The default duration to cache a value for.
        /// </summary>
        public static int DefaultCacheDurationInSeconds { get; set; }

        /// <summary>
        /// Determines whether to function to call for a cache miss is wrapped in a retry mechanism by default.
        /// </summary>
        public static bool WrapInRetry { get; set; }

        /// <summary>
        /// The default maximum number of attempts for inner retry.
        /// </summary>
        public static int RetryMaximumNumberOfAttempts { get; set; }

        /// <summary>
        /// The default milliseconds per slot for inner retry.
        /// </summary>
        public static int RetryMillisecondsPerSlot { get; set; }

        /// <summary>
        /// Indicates whether to truncate the number of slots for the inner retry by default.
        /// </summary>
        public static bool RetryTruncateNumberOfSlots { get; set; }

        /// <summary>
        /// The default maximum number of slots when <see cref="RetryTruncateNumberOfSlots"/> is true for the inner retry.
        public static int RetryMaximumNumberOfSlotsWhenTruncated { get; set; }

        /// <summary>
        /// The default percentage of jitter to apply to the interval of the inner retry.
        /// </summary>
        public static int RetryJitterPercentage { get; set; }

        /// <summary>
        /// The default percentage of jitter to apply to the duration of the cache expiry.
        /// </summary>
        public static int CacheDurationJitterPercentage { get; set; }

        /// <summary>
        /// Sets the defaults for the public properties.
        /// </summary>
        static Cache()
        {
            DefaultCacheDurationInSeconds = 60;
            WrapInRetry = true;
            RetryMaximumNumberOfAttempts = 8;
            RetryMillisecondsPerSlot = 32;
            RetryTruncateNumberOfSlots = true;
            RetryMaximumNumberOfSlotsWhenTruncated = 16;
            RetryJitterPercentage = 25;
            CacheDurationJitterPercentage = 25;
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using the default settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the object.</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique.</param>
        /// <param name="functionToCallOnCacheMiss">The function to call when the value is not in cache.</param>
        /// <returns>The value from either cache or the function that is called to fill the cache.</returns>
        public static T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss)
        {
            return Get(cacheKey, functionToCallOnCacheMiss, DefaultCacheDurationInSeconds, WrapInRetry, RetryMaximumNumberOfAttempts, RetryMillisecondsPerSlot, RetryTruncateNumberOfSlots, RetryMaximumNumberOfSlotsWhenTruncated, RetryJitterPercentage, CacheDurationJitterPercentage);
        }

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using passed in settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the object.</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique.</param>
        /// <param name="functionToCallOnCacheMiss">Function to call on cache miss.</param>
        /// <param name="expirationInSeconds">The duration to cache a value for.</param>
        /// <param name="wrapInRetry">Determines whether to function to call for a cache miss is wrapped in a retry mechanism.</param>
        /// <param name="retryMaximumNumberOfAttempts">The maximum number of attempts for inner retry.</param>
        /// <param name="retryMillisecondsPerSlot">The milliseconds per slot for inner retry.</param>
        /// <param name="retryTruncateNumberOfSlots">Indicates whether to truncate the number of slots for the inner retry.</param>
        /// <param name="retryMaximumNumberOfSlotsWhenTruncated">The maximum number of slots when <see cref="retryTruncateNumberOfSlots"/> is true for the inner retry.</param>
        /// <param name="cacheDurationJitterPercentage">The percentage of jitter to apply to the interval of the inner retry.</param>
        /// <param name="retryJitterPercentage">The percentage of jitter to apply to the duration of the cache expiry.</param>
        public static T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss, int expirationInSeconds, bool wrapInRetry, int retryMaximumNumberOfAttempts, int retryMillisecondsPerSlot, bool retryTruncateNumberOfSlots, int retryMaximumNumberOfSlotsWhenTruncated, int retryJitterPercentage, int cacheDurationJitterPercentage)
        {
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                throw new ArgumentNullException("cacheKey");
            }

            ValidateParameters(expirationInSeconds, wrapInRetry, retryMaximumNumberOfAttempts, retryMillisecondsPerSlot, retryTruncateNumberOfSlots, retryMaximumNumberOfSlotsWhenTruncated, retryJitterPercentage, cacheDurationJitterPercentage);

            string prefixedCacheKey = "CacheMagic_" + cacheKey;

            // get object from cache
            CachedObject<T> objectFromCache = HttpContext.Current.Cache.Get(prefixedCacheKey) as CachedObject<T>;

            if (objectFromCache == null)
            {
                // not in cache, retrieve from the source and store in cache
                if (wrapInRetry)
                {
                    objectFromCache = new CachedObject<T>(Retry.Function(functionToCallOnCacheMiss, retryMaximumNumberOfAttempts, retryMillisecondsPerSlot, retryTruncateNumberOfSlots, retryMaximumNumberOfSlotsWhenTruncated, retryJitterPercentage));
                }
                else
                {
                    objectFromCache = new CachedObject<T>(functionToCallOnCacheMiss.Invoke());    
                }

                HttpContext.Current.Cache.Insert(prefixedCacheKey, objectFromCache, null, DateTime.Now.Add(TimeSpan.FromSeconds(Jitter.Apply(expirationInSeconds, cacheDurationJitterPercentage))), TimeSpan.Zero);
            }

            return objectFromCache.Value;
        }

        /// <summary>
        /// Validates the parameters.
        /// </summary>
        /// <param name="functionToCallOnCacheMiss">Function to call on cache miss.</param>
        /// <param name="expirationInSeconds">The duration to cache a value for.</param>
        /// <param name="wrapInRetry">Determines whether to function to call for a cache miss is wrapped in a retry mechanism.</param>
        /// <param name="retryMaximumNumberOfAttempts">The maximum number of attempts for inner retry.</param>
        /// <param name="retryMillisecondsPerSlot">The milliseconds per slot for inner retry.</param>
        /// <param name="retryTruncateNumberOfSlots">Indicates whether to truncate the number of slots for the inner retry.</param>
        /// <param name="retryMaximumNumberOfSlotsWhenTruncated">The maximum number of slots when <see cref="retryTruncateNumberOfSlots"/> is true for the inner retry.</param>
        /// <param name="cacheDurationJitterPercentage">The percentage of jitter to apply to the interval of the inner retry.</param>
        /// <param name="retryJitterPercentage">The percentage of jitter to apply to the duration of the cache expiry.</param>
        internal static void ValidateParameters(int expirationInSeconds, bool wrapInRetry, int retryMaximumNumberOfAttempts, int retryMillisecondsPerSlot, bool retryTruncateNumberOfSlots, int retryMaximumNumberOfSlotsWhenTruncated, int retryJitterPercentage, int cacheDurationJitterPercentage)
        {
            if (expirationInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("expirationInSeconds", "Expiration must be greater than zero seconds");
            }
            if (retryMaximumNumberOfAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException("retryMaximumNumberOfAttempts", "Maximum number of retry attempts must be greater than zero");
            }
            if (retryMillisecondsPerSlot <= 0)
            {
                throw new ArgumentOutOfRangeException("retryMillisecondsPerSlot", "Number of milliseonds per slot for retry must be greater than zero");
            }
            if (retryMaximumNumberOfSlotsWhenTruncated <= 0)
            {
                throw new ArgumentOutOfRangeException("retryMaximumNumberOfSlotsWhenTruncated", "Maximum number of slots when truncated for retry must be greater than zero");
            }
            if (retryJitterPercentage < 0 || retryJitterPercentage >= 100)
            {
                throw new ArgumentOutOfRangeException("retryMaximumNumberOfSlotsWhenTruncated", "Jitter percentage for retry must be between zero and one hundred");
            }
            if (cacheDurationJitterPercentage < 0 || cacheDurationJitterPercentage >= 100)
            {
                throw new ArgumentOutOfRangeException("cacheDurationJitterPercentage", "Jitter percentage for cache duration must be between zero and one hundred");
            }
        }
    }
}