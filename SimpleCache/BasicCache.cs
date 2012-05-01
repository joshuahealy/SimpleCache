using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCache.Exceptions;

namespace SimpleCache
{
	public class BasicCache<TKey> : SimpleCacheBase<TKey>
	{
		protected Dictionary<TKey, object> Cache { get; set; }

		public BasicCache()
		{
			Cache = new Dictionary<TKey, object>();
		}

		public override IEnumerable<KeyValuePair<TKey, object>> Items 
		{
			get
			{
				return Cache.ToList();
			}
		}

		public override bool IsItemCached(TKey key)
		{
			return Cache.ContainsKey(key);
		}

		public override TItem GetItem<TItem>(TKey key)
		{
			if (IsItemCached(key))
			{
				object item = Cache[key];
				if (item is TItem)
				{
					return (TItem)item;
				}
				throw new ItemTypeIncorrectException(key, typeof(TItem), item.GetType());
			}
			throw new ItemNotInCacheException(key);
		}

		public override void SetItem<TItem>(TKey key, TItem item)
		{
			if (IsItemCached(key))
			{
				Cache.Add(key, item);
			}
			else
			{
				Cache[key] = item;
			}
		}

		public override void RemoveItem(TKey key)
		{
			if (IsItemCached(key))
			{
				Cache.Remove(key);
			}
		}
	}
}
