using System.Collections.Generic;

namespace Application.Core.Data
{
    public interface IPageableList<TItem> : IPageableList
    {
        void Add(TItem item);

        IList<TItem> Items { get; set; }
    }

    public interface IPageableList
    {
        bool HasNext { get; set; }

        bool HasPrevious { get; set; }

        int DisplayCount
        {
            get;
        }

        int TotalCount { get; set; }

        IPageInfo PageInfo { get; set; }
    }
}