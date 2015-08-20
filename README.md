# CacheMagic

A c# library for in-memory caching, using jitter and retries for stable systems at scale

[![Build Status .NET](https://ci.appveyor.com/api/projects/status/github/JorritSalverda/CacheMagic?svg=true)](https://ci.appveyor.com/project/JorritSalverda/CacheMagic/)
[![Build Status Mono](https://api.travis-ci.org/JorritSalverda/CacheMagic.svg)](https://travis-ci.org/JorritSalverda/CacheMagic/)
[![NuGet downloads](https://img.shields.io/nuget/dt/CacheMagic.svg)](https://www.nuget.org/packages/CacheMagic)
[![Version](https://img.shields.io/nuget/v/CacheMagic.svg)](https://www.nuget.org/packages/CacheMagic)
[![Issues](https://img.shields.io/github/issues/JorritSalverda/CacheMagic.svg)](https://github.com/JorritSalverda/CacheMagic/issues)
[![License](https://img.shields.io/github/license/JorritSalverda/CacheMagic.svg)](https://github.com/JorritSalverda/CacheMagic/blob/master/LICENSE)

Why?
--------------------------------
To cache any slowly fetched data in memory

Usage
--------------------------------
You at least have to provide a key name and a function to call in case of a cache miss; this function uses the default settings

```csharp
Cache.Get("KeyName", () => { return _databaseRepository.Get(id); });
```

### Changing defaults

The following default settings are used and can be changed by using the code below with different values

```csharp
Cache.UpdateSettings(new CacheSettings(
	cretrySettings: new RetrySettings(
		jitterSettings: new JitterSettings(percentage: 25), 
		maximumNumberOfAttempts: 5, 
		millisecondsPerSlot: 32, 
		truncateNumberOfSlots: true, 
		maximumNumberOfSlotsWhenTruncated: 16),
	jitterSettings: new JitterSettings(percentage: 25), 
	cacheDurationInSeconds: 60, 
	wrapInRetry: true));
```

Validation of settings always takes place during construction of the object so it fails as early as possible.

### Non-static usage

If you wish to be able to inject it - for example for having different settings in different places - you can use the `CacheInstance` class:

```csharp
ICacheInstance instance = new CacheInstance(new CacheSettings(
	cretrySettings: new RetrySettings(
		jitterSettings: new JitterSettings(percentage: 25), 
		maximumNumberOfAttempts: 5, 
		millisecondsPerSlot: 32, 
		truncateNumberOfSlots: true, 
		maximumNumberOfSlotsWhenTruncated: 16),
	jitterSettings: new JitterSettings(percentage: 25), 
	cacheDurationInSeconds: 60, 
	wrapInRetry: true));
```

This interface and class only has the Get methods without the settings parameter because you provide those during construction.

```csharp
instance.Get("KeyName", () => { return _databaseRepository.Get(id); });
```

Get it
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [CacheMagic](https://www.nuget.org/packages/CacheMagic/) from the package manager console:

    PM> Install-Package CacheMagic

CacheMagic is Copyright &copy; 2015 [Jorrit Salverda](http://blog.jorritsalverda.com/) and other contributors under the [MIT license](https://github.com/JorritSalverda/CacheMagic/blob/master/LICENSE).
