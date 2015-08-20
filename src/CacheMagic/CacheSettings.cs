using System;
using JitterMagic;
using RetryMagic;

namespace CacheMagic
{
    /// <summary>
    /// Holds the settings required by Cache and CacheInstance
    /// </summary>
    public class CacheSettings
    {
        /// <summary>
        /// Settings for the inner retry if <see cref="WrapInRetry"/> is <c>true</c>
        /// </summary>
        /// <value>The retry settings.</value>
        public RetrySettings RetrySettings { get; private set; }

        /// <summary>
        /// Jitter settings for applying to the <see cref="CacheDurationInSeconds"/>
        /// </summary>
        /// <value>The jitter settings.</value>
        public JitterSettings JitterSettings { get; private set; }

        /// <summary>
        /// The duration to cache a value for in seconds.
        /// </summary>
        public int CacheDurationInSeconds { get; private set; }

        /// <summary>
        /// Determines whether to function to call for a cache miss is wrapped in a retry mechanism by default.
        /// </summary>
        public bool WrapInRetry { get; private set; }

        public CacheSettings(int cacheDurationInSeconds = 60, bool wrapInRetry = true)
            :this(new RetrySettings(), new JitterSettings(), cacheDurationInSeconds, wrapInRetry)
        {
        }

        public CacheSettings(RetrySettings retrySettings, JitterSettings jitterSettings, int cacheDurationInSeconds = 60, bool wrapInRetry = true)
        {
            RetrySettings = retrySettings;
            JitterSettings = jitterSettings;
            CacheDurationInSeconds = cacheDurationInSeconds;
            WrapInRetry = wrapInRetry;

            Validate();
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        internal void Validate()
        {
            if (RetrySettings == null)
            {
                throw new ArgumentNullException("retrySettings");
            }
            if (JitterSettings == null)
            {
                throw new ArgumentNullException("jitterSettings");
            }
            if (CacheDurationInSeconds <= 0)
            {
                throw new ArgumentOutOfRangeException("cacheDurationInSeconds", "Cache duration must be greater than zero seconds");
            }
        }
    }
}

