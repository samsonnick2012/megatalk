using System.Collections.Generic;
using System.Web.Mvc;
using Application.Core.Data;

namespace Innostar.UI.Areas.Administration.ViewModels
{
    public class UsersViewModel
    {
        public PageableList<ChatUserViewModel> chatUserViewModels { get; set; }
        public bool allSelected { get; set; }
        public string Message { get; set; }
        public string XmppAdminLogin { get; set; }
        public string XmppAdminPassword { get; set; }
        public string HttpBindAddress { get; set; }
        public int SelectedFilterType { get; set; }
        public IEnumerable<SelectListItem> FilterTypes { get; set; }
        public int SelectedMessageTemplate { get; set; }
        public IEnumerable<SelectListItem> MessageTemplates { get; set; }

        public UsersViewModel()
        {
            var filterTypeList = new List<SelectListItem>();
            filterTypeList.Add(new SelectListItem
            {
                Text = "Все",
                Value = "0"
            });
            filterTypeList.Add(new SelectListItem
            {
                Text = "Платные",
                Value = "1"
            });
            FilterTypes = filterTypeList;
        }
    }
}