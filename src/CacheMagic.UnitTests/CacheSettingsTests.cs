using System;
using System.Web;
using Http.TestLibrary;
using Xunit;
using JitterMagic;
using RetryMagic;

namespace CacheMagic.UnitTests
{
    public class CacheSettingsTests
    {
        public class Constructor
        {
            [Fact]
            public void Throws_ArgumentOutOfRangeException_If_CacheDurationInSeconds_Is_Zero_Or_Less()
            {
                // act + assert
                Assert.Throws<ArgumentOutOfRangeException>(() => new CacheSettings(cacheDurationInSeconds: 0));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_RetrySettings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new CacheSettings(retrySettings: (RetrySettings)null, jitterSettings: new JitterSettings(), cacheDurationInSeconds: 45, wrapInRetry: false));
            }

            [Fact]
            public void Throws_ArgumentNullException_If_JitterSettings_Is_Null()
            {
                // act + assert
                Assert.Throws<ArgumentNullException>(() => new CacheSettings(retrySettings: new RetrySettings(), jitterSettings: (JitterSettings)null, cacheDurationInSeconds: 45, wrapInRetry: false));
            }
        }
    }
}
