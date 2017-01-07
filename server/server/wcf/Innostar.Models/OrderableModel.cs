using System.Collections.Generic;
using System.Linq;
using Application.Core.Data;

namespace Innostar.Models
{
    public class OrderableModel : ModelBase
    {
        public int Order
        {
            get;
            set;
        }

		public OrderableModel CalculateOrder(IEnumerable<OrderableModel> allItems)
		{
			var items = allItems.OrderBy(e => e.Order).ToList();
			var maxOrder = items.Any() ? items.Max(e => e.Order) : -1;

			if (IsNew)
			{
				Order = maxOrder + 1;
			}

			return this;
		}

		public bool TryReOrder(IEnumerable<OrderableModel> allItems, out OrderableModel result, int orderDirection = 0)
		{
			var isSuccessfully = false;
			result = null;

			var items = allItems.OrderBy(e => e.Order).ToList();
			var maxOrder = items.Any() ? items.Max(e => e.Order) : -1;

			var possibleOrderValue = Order + orderDirection;

			if (possibleOrderValue > maxOrder)
			{
				Order = maxOrder;
			}
			else if (possibleOrderValue < 0)
			{
				Order = 0;
			}
			else
			{
				Order = possibleOrderValue;

				var itemIndex = items.IndexOf(items.FirstOrDefault(e => e.Id == Id));

				var neighborIndex = itemIndex + orderDirection;
				OrderableModel neighbor = null;

				if (neighborIndex >= 0 && neighborIndex < items.Count)
				{
					neighbor = items[neighborIndex];
				}

				if (neighbor != null)
				{
					neighbor.Order += -orderDirection;

					result = neighbor;
					isSuccessfully = true;
				}
			}

			return isSuccessfully;
		}
    }
}
