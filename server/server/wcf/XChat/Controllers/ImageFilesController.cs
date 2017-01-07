using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XChat.Filters;
using XChat.Models.DB;
using XChat.Models;

namespace XChat.Controllers
{
    [CustomAuthorize(Roles = CustomAuthorizeAttribute.ROLE_ADMIN)]
    public class ImageFilesController : BaseController
    {
		private readonly IDataFileRepository datafileRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public ImageFilesController() : this(new DataFileRepository())
        {
        }

        public ImageFilesController(IDataFileRepository imagefileRepository)
        {
            this.datafileRepository = imagefileRepository;
        }

        //
        // GET: /ImageFiles/

        public ViewResult Index()
        {
            return View(datafileRepository.All);
        }

        //
        // GET: /ImageFiles/Details/5

        public ViewResult Details(int id)
        {
            return View(datafileRepository.Find(id));
        }

        //
        // GET: /ImageFiles/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ImageFiles/Create

        [HttpPost]
        public ActionResult Create(DataFile imagefile)
        {
            if (ModelState.IsValid) {
                datafileRepository.InsertOrUpdate(imagefile);
                datafileRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }
        
        //
        // GET: /ImageFiles/Edit/5
 
        public ActionResult Edit(int id)
        {
             return View(datafileRepository.Find(id));
        }

        //
        // POST: /ImageFiles/Edit/5

        [HttpPost]
        public ActionResult Edit(DataFile imagefile)
        {
            if (ModelState.IsValid) {
                datafileRepository.InsertOrUpdate(imagefile);
                datafileRepository.Save();
                return RedirectToAction("Index");
            } else {
				return View();
			}
        }

        //
        // GET: /ImageFiles/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(datafileRepository.Find(id));
        }

        //
        // POST: /ImageFiles/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            datafileRepository.Delete(id);
            datafileRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //imagefileRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

