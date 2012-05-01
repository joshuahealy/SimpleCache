using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache
{
	class AutoRenewingCacheItem<TItem> : TimedCacheItem<TItem>
	{
		public AutoRenewingCacheItem(TItem item, int lifespanMilliseconds, Func<TItem> renewalFunction)
			: base(item, lifespanMilliseconds)
		{
			RenewalFunction = renewalFunction;
		}

		public override TItem Item
		{
			get
			{
				if (base.HasExpired)
				{
					base.Item = RenewalFunction();
					ExpiryDateTime = DateTime.Now.AddMilliseconds(LifespanMilliseconds);
				}
				return base.Item;
			}
			set
			{
				base.Item = value;
			}
		}

		public override bool HasExpired
		{
			get
			{
				return false;
			}
		}

		public Func<TItem> RenewalFunction { get; set; }
	}
}
