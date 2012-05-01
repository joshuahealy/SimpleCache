using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache
{
	public abstract class SimpleCacheBase<TKey> : ISimpleCache<TKey>
	{
		public abstract IEnumerable<KeyValuePair<TKey, object>> Items { get; }

		public abstract bool IsItemCached(TKey key);

		public abstract TItem GetItem<TItem>(TKey key);

		public virtual TItem GetOrSetItem<TItem>(TKey key, Func<TItem> itemFunc)
		{
			if (IsItemCached(key))
			{
				return GetItem<TItem>(key);
			}
			TItem item = itemFunc();
			SetItem(key, item);
			return item;
		}

		public abstract void SetItem<TItem>(TKey key, TItem item);

		public virtual void SetItem<TItem>(TKey key, Func<TItem> itemFunc)
		{
			TItem item = itemFunc();
			SetItem(key, item);
		}

		public virtual void SetItems<TItem>(IEnumerable<KeyValuePair<TKey, TItem>> items)
		{
			foreach (var kvp in items)
			{
				SetItem(kvp.Key, kvp.Value);
			}
		}

		public abstract void RemoveItem(TKey key);
	}
}
