using System;

namespace CacheMagic
{
    /// <summary>
    /// Used when needing separate instances of Cache class with settings; otherwise use the static class and methods.
    /// </summary>    
    public class CacheInstance:ICacheInstance
    {
        /// <summary>
        /// Settings for this instance.
        /// </summary>
        /// <value>The settings.</value>
        public CacheSettings Settings { get; private set; }

        public CacheInstance(CacheSettings settings)
        {
            UpdateSettings(settings);
        }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        public void UpdateSettings(CacheSettings settings)
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
        public T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss)
        {
            return Cache.Get(cacheKey, functionToCallOnCacheMiss, Settings);
        }            
    }
}

