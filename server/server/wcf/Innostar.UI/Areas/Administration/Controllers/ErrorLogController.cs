using System;
using System.Linq;
using System.Web.Mvc;
using Application.Core.Data;
using Innostar.Dal.Repositories;
using Innostar.Models;
using Innostar.UI.Areas.Administration.ViewModels;

namespace Innostar.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class ErrorLogController : Controller
    {
        public ActionResult Index(int? pageNumber = 1)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                var errors = repositoriesContainer.RepositoryFor<Error>().GetPageableBy(new PageableQuery<Error>(new PageInfo
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = 15
                }).OrderBy(new SortCriteria("Date", SortDirection.Desc)));

                var viewModels = errors.Items.Select(e => new ErrorViewModel
                {
                    Id = e.Id,
                    Title = e.Message,
                    Time = e.Date,
                });

                return View(new PageableList<ErrorViewModel>
                {
                    Items = viewModels.ToList(),
                    HasNext = errors.HasNext,
                    HasPrevious = errors.HasPrevious,
                    TotalCount = errors.TotalCount,
                    PageInfo = errors.PageInfo
                });
            }
        }

        public ActionResult Error(int id)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                var error = repositoriesContainer.RepositoryFor<Error>().GetBy(new Query<Error>(e => e.Id == id));

                return View(new ErrorViewModel
                {
                    AuthorizedUzer = error.Identity,
                    DetailedException = error.Exception,
                    Id = error.Id,
                    Time = error.Date,
                    Title = error.Message
                });
            }
        }
    }
}
