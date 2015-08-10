# CacheMagic

A c# library for in-memory caching, using jitter and retries for stable systems at scale

[![Build Status .NET](https://img.shields.io/appveyor/ci/JorritSalverda/CacheMagic.svg)](https://ci.appveyor.com/project/JorritSalverda/CacheMagic/)
[![Build Status Mono](https://img.shields.io/travis/JorritSalverda/CacheMagic.svg)](https://travis-ci.org/JorritSalverda/CacheMagic/)
[![NuGet downloads](https://img.shields.io/nuget/dt/CacheMagic.svg)](https://www.nuget.org/packages/CacheMagic)
[![Version](https://img.shields.io/nuget/v/CacheMagic.svg)](https://www.nuget.org/packages/CacheMagic)
[![Issues](https://img.shields.io/github/issues/JorritSalverda/CacheMagic.svg)](https://github.com/JorritSalverda/CacheMagic/issues)
[![License](https://img.shields.io/github/license/JorritSalverda/CacheMagic.svg)](https://github.com/JorritSalverda/CacheMagic/blob/master/LICENSE)

Why?
--------------------------------
To cache any slowly fetched data in memory

Usage
--------------------------------
You at least have to provide a key name and a function to call in case of a cache miss; this function uses the DefaultCacheDurationInSeconds value

```csharp
Cache.Get("KeyName", () => { return _databaseRepository.Get(id); });
```

You can provide the cache duration in seconds explicitly

```csharp
Cache.Get("KeyName", () => { return _databaseRepository.Get(id); }, 300);
```

### Changing defaults

The following defaults are used and can be changed by using the following code with different values

```csharp
Cache.DefaultCacheDurationInSeconds = 60;
```

```csharp
Cache.WrapInRetry = true;
```

Get it
--------------------------------
First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [CacheMagic](https://www.nuget.org/packages/CacheMagic/) from the package manager console:

    PM> Install-Package CacheMagic

CacheMagic is Copyright &copy; 2015 [Jorrit Salverda](http://blog.jorritsalverda.com/) and other contributors under the [MIT license](https://github.com/JorritSalverda/CacheMagic/blob/master/LICENSE).
