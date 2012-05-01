using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache
{
	public interface ISimpleCache<TKey>
	{
		IEnumerable<KeyValuePair<TKey,object>> Items { get; }
		bool IsItemCached(TKey key);
		TItem GetItem<TItem>(TKey key);
		TItem GetOrSetItem<TItem>(TKey key, Func<TItem> itemFunc);
		void SetItem<TItem>(TKey key, TItem item);
		void SetItem<TItem>(TKey key, Func<TItem> itemFunc);
		void SetItems<TItem>(IEnumerable<KeyValuePair<TKey, TItem>> items);
		void RemoveItem(TKey key);
	}
}
