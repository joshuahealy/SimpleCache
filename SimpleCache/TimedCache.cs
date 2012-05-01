using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCache.Exceptions;
using System.Threading.Tasks;
using System.Threading;

namespace SimpleCache
{
	public class TimedCache<TKey> : ISimpleCache<TKey>, IDisposable
	{
		protected BasicCache<TKey> Cache { get; set; }
		private Timer CheckItemExpirationTimer { get; set; }


		public TimedCache(int defaultItemLifespanMilliseconds, int checkItemExpirationIntervalMilliseconds)
			: base()
		{
			if (defaultItemLifespanMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException("defaultItemLifespanMilliseconds", "defaultItemLifespanMilliseconds must have a positive value");
			}
			if (checkItemExpirationIntervalMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException("checkItemExpirationIntervalMilliseconds", "checkItemExpirationIntervalMilliseconds must have a positive value");
			}
			DefaultItemLifespanMilliseconds = defaultItemLifespanMilliseconds;
			CheckItemExpirationIntervalMilliseconds = checkItemExpirationIntervalMilliseconds;
			Cache = new BasicCache<TKey>();
			CheckItemExpirationTimer = new Timer(CheckItemExpiration, null, CheckItemExpirationIntervalMilliseconds, CheckItemExpirationIntervalMilliseconds);

		}

		public int DefaultItemLifespanMilliseconds { get; private set; }
		public int CheckItemExpirationIntervalMilliseconds { get; private set; }

		private void CheckItemExpiration(object stateInfo)
		{
			lock (this)
			{
				var keysToRemove = Cache.Items
					.Where(kvp => (kvp.Value as IExpirable).HasExpired)
					.Select(kvp => kvp.Key);
				foreach (var key in keysToRemove)
				{
					RemoveItem(key);
				}
			}
		}

		public IEnumerable<KeyValuePair<TKey, object>> Items
		{
			get
			{
				CheckItemExpiration(null);
				return Cache.Items.Select(kvp => new KeyValuePair<TKey, object>(kvp.Key, (kvp.Value as TimedCacheItem<object>).Item));
			}
		}

		public bool IsItemCached(TKey key)
		{
			return Cache.IsItemCached(key) && !Cache.GetItem<IExpirable>(key).HasExpired;
		}

		public TItem GetItem<TItem>(TKey key)
		{
			if (IsItemCached(key))
			{
				return Cache.GetItem<TimedCacheItem<TItem>>(key).Item;
			}
			throw new ItemNotInCacheException(key);
		}

		public TItem GetOrSetItem<TItem>(TKey key, Func<TItem> itemFunc)
		{
			return GetOrSetItem(key, itemFunc, false);
		}

		public TItem GetOrSetItem<TItem>(TKey key, Func<TItem> itemFunc, bool autoRenew, int? lifespanMilliseconds = null)
		{
			TimedCacheItem<TItem> item = null;
			if (IsItemCached(key))
			{
				item = Cache.GetItem<TimedCacheItem<TItem>>(key);
			}
			if (item == null || (item is AutoRenewingCacheItem<TItem> && !autoRenew) || (!(item is AutoRenewingCacheItem<TItem>) && autoRenew))
			{
				item = SetItemHelper(key, item, itemFunc, autoRenew, lifespanMilliseconds);
			}
			return item.Item;
		}

		public void SetItem<TItem>(TKey key, Func<TItem> itemFunc, bool autoRenew, int? lifespanMilliseconds = null)
		{
			SetItemHelper(key, null, itemFunc, autoRenew, lifespanMilliseconds);
		}

		private TimedCacheItem<TItem> SetItemHelper<TItem>(TKey key, TimedCacheItem<TItem> currentItem, Func<TItem> itemFunc, bool autoRenew, int? lifespanMilliseconds)
		{
			if ((lifespanMilliseconds ?? 1) <= 0)
			{
				throw new ArgumentOutOfRangeException("lifespanMilliseconds", "lifespanMilliseconds must have a positive value");
			}
			var item = autoRenew ?
					new AutoRenewingCacheItem<TItem>(currentItem == null ? itemFunc() : currentItem.Item, lifespanMilliseconds ?? DefaultItemLifespanMilliseconds, itemFunc) :
					new TimedCacheItem<TItem>(currentItem == null ? itemFunc() : currentItem.Item, lifespanMilliseconds ?? DefaultItemLifespanMilliseconds);
			Cache.SetItem(key, item);
			return item;
		}

		public void SetItem<TItem>(TKey key, TItem item)
		{
			Cache.SetItem(key, new TimedCacheItem<TItem>(item, DefaultItemLifespanMilliseconds));
		}

		public void SetItem<TItem>(TKey key, Func<TItem> itemFunc)
		{
			SetItemHelper(key, null, itemFunc, false, null);
		}

		public void SetItems<TItem>(IEnumerable<KeyValuePair<TKey, TItem>> items)
		{
			foreach (var kvp in items)
			{
				SetItem(kvp.Key, kvp.Value);
			}
		}

		public void RemoveItem(TKey key)
		{
			Cache.RemoveItem(key);
		}

		public void Dispose()
		{
			if (CheckItemExpirationTimer != null)
			{
				CheckItemExpirationTimer.Change(Timeout.Infinite, Timeout.Infinite);
				CheckItemExpirationTimer.Dispose();
			}
		}
	}
}
