using System.Collections.Generic;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class ListViewModel<TViewModel>
    {
        public int Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string TypeName
        {
            get;
            set;
        }

        public IEnumerable<TViewModel> Items
        {
            get; set;
        }

        public ListViewModel()
        {
            Items = new List<TViewModel>();
        }
    }
}