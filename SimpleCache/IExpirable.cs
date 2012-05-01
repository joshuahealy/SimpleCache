using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCache
{
	interface IExpirable
	{
		int LifespanMilliseconds { get; set; }
		DateTime ExpiryDateTime { get; set; }
		bool HasExpired { get; }
	}
}
