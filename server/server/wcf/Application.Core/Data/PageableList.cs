using System.Collections.Generic;
using System.Linq;

namespace Application.Core.Data
{
	public class PageableList<TItem> : IPageableList<TItem>
	{
		public int PageSizeLimit
		{
			get;
			private set;
		}

		public bool HasNext { get; set; }

		public bool HasPrevious { get; set; }

	    public int DisplayCount
	    {
	        get
	        {
	            return Items.Count;
	        }
	    }

	    public IList<TItem> Items { get; set; }

		public int TotalCount { get; set; }

		public IPageInfo PageInfo { get; set; }

		public PageableList()
		{
			PageInfo = new PageInfo();
			Items = new List<TItem>();
		}

		public PageableList(int pageSizeLimit)
			: this()
		{
			PageSizeLimit = pageSizeLimit;
			HasNext = false;
		}

		public void Add(TItem item)
		{
			Items.Add(item);
		}
    }
}
