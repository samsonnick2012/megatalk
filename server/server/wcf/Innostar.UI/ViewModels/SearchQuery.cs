using Application.Core.Data;

namespace Innostar.UI.ViewModels
{
    public class SearchQuery
    {
        public string Query
        {
            get;
            set;
        }

        public bool SearchByTags
        {
            get;
            set;
        }

        public IPageInfo PageInfo
        {
            get;
            set;
        }

        public SearchQuery()
        {
            PageInfo = new PageInfo();
        }
    }
}