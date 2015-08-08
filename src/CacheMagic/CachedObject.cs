using System;

namespace CacheMagic
{
    /// <summary>
    /// A wrapper for the cached object so null values can be stored
    /// </summary>
    public class CachedObject<T>
    {
    	public T Value { get; private set; }

    	public CachedObject(T value)
    	{
    		Value = value;
    	}
    }
}