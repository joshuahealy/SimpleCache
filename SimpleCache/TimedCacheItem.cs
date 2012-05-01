using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache
{
	class TimedCacheItem<TItem> : IExpirable
	{
		public TimedCacheItem(TItem item, int lifespanMilliseconds)
		{
			if (lifespanMilliseconds <= 0)
			{
				throw new ArgumentOutOfRangeException("lifespanMilliseconds", "lifespanMilliseconds must have a positive value");
			}
			Item = item;
			LifespanMilliseconds = lifespanMilliseconds;
			ExpiryDateTime = DateTime.Now.AddMilliseconds(lifespanMilliseconds);
		}

		public virtual TItem Item { get; set; }
		public int LifespanMilliseconds { get; set; }
		public DateTime ExpiryDateTime { get; set; }
		
		public virtual bool HasExpired
		{
			get
			{
				return ExpiryDateTime <= DateTime.Now;
			}
		}

	}
}
