using System.Linq;
using System.Web.Mvc;
using Application.Core.Data;
using Application.Core.UI.Controllers;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.Models;
using Innostar.UI.Areas.Administration.ViewModels;

namespace Innostar.UI.Areas.Administration.Controllers
{
    [Authorize]
    public class MessageTemplateController : BasicController
    {
        public ActionResult Index(int? pageNumber = 1)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                var templates = repositoriesContainer.RepositoryFor<MessageTemplate>().GetPageableBy(new PageableQuery<MessageTemplate>(new PageInfo
                {
                    PageNumber = pageNumber ?? 1,
                    PageSize = 15
                }));

                var viewModels = templates.Items.Select(e => new MessageTemplateViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Blocked = e.Blocked
                });

                return View(new PageableList<MessageTemplateViewModel>
                {
                    Items = viewModels.ToList(),
                    HasNext = templates.HasNext,
                    HasPrevious = templates.HasPrevious,
                    TotalCount = templates.TotalCount,
                    PageInfo = templates.PageInfo
                });
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var repositoriesContainer = new InnostarRepositoriesContainer())
            {
                var template = repositoriesContainer.RepositoryFor<MessageTemplate>().GetBy(new Query<MessageTemplate>(e => e.Id == id));

                return View(new MessageTemplateViewModel
                {
                    Id = template.Id,
                    Title = template.Title,
                    Text = template.Message,
                    Blocked = template.Blocked
                });
            }
        }

        [HttpPost]
        public ActionResult Edit(MessageTemplateViewModel model)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new MessageTemplateRepository(context);
                var template = repository._Get(e => e.Id == model.Id).FirstOrDefault();

                template.Message = model.Text;

                if (!model.Blocked)
                {
                    template.Title = model.Title;
                }

                repository._Update(template);
                repository._Save();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new MessageTemplateViewModel());
        }

        [HttpPost]
        public ActionResult Create(MessageTemplateViewModel model)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new MessageTemplateRepository(context);
                var template = new MessageTemplate
                {
                    Blocked = false,
                    SpecialTemplate = 0,
                    Message = model.Text,
                    Title = model.Title
                };

                repository._Insert(template);
                repository._Save();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            using (var context = new InnostarModelsContext())
            {
                var repository = new MessageTemplateRepository(context);
                var template = repository._Get(e => e.Id == id).FirstOrDefault();

                if (!template.Blocked)
                {
                    repository._Delete(template);
                    repository._Save();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
