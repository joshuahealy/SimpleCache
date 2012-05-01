using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache.Exceptions
{
	public class ItemNotInCacheException : Exception
	{
		public ItemNotInCacheException(object key)
			: base("The key specified did not exist in the cache")
		{
			Key = key;
		}

		public object Key { get; private set; }
	}
}
