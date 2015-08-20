using System;

namespace CacheMagic
{
    /// <summary>
    /// Used when needing separate instances of Cache class with settings; otherwise use the static class and methods.
    /// </summary>    
    public interface ICacheInstance
    {
        /// <summary>
        /// Settings for this instance.
        /// </summary>
        /// <value>The settings.</value>
        CacheSettings Settings { get; }

        /// <summary>
        /// Updates the settings.
        /// </summary>
        /// <param name="settings">Settings.</param>
        void UpdateSettings(CacheSettings settings);

        /// <summary>
        /// Fetches a value from memory cache or calls a function to get the value from if it's missing in cache using the default settings.
        /// </summary>
        /// <typeparam name="T">The generic type of the object.</typeparam>
        /// <param name="cacheKey">The name of the cache key; needs to be unique.</param>
        /// <param name="functionToCallOnCacheMiss">The function to call when the value is not in cache.</param>
        /// <returns>The value from either cache or the function that is called to fill the cache.</returns>
        T Get<T>(string cacheKey, Func<T> functionToCallOnCacheMiss);
    }
}

