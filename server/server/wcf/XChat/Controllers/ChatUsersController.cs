using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XChat.Models.DB;
using XChat.Models;
using XChat.Service;

namespace XChat.Controllers
{   
    public class ChatUsersController : BaseController
    {
		private readonly IDataFileRepository imagefileRepository;
		private readonly IChatUserRepository chatuserRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public ChatUsersController() : this(new DataFileRepository(), new ChatUserRepository())
        {
        }

        public ChatUsersController(IDataFileRepository imagefileRepository, IChatUserRepository chatuserRepository)
        {
			this.imagefileRepository = imagefileRepository;
			this.chatuserRepository = chatuserRepository;
        }

        //
        // GET: /ChatUsers/

        public ViewResult Index()
        {
            return View(chatuserRepository.AllIncluding(chatuser => chatuser.Image));
        }

        //
        // GET: /ChatUsers/Details/5

        public ViewResult Details(int id)
        {
            return View(chatuserRepository.Find(id));
        }

        //
        // GET: /ChatUsers/Create

        public ActionResult Create()
        {
			ViewBag.PossibleImages = imagefileRepository.All;
            return View();
        } 

        //
        // POST: /ChatUsers/Create

        [HttpPost]
        public ActionResult Create(ChatUser chatuser)
        {
            if (ModelState.IsValid)
            {
                AuthData data = Service.Service.RegisterUser(chatuser.Login, chatuser.Password, chatuser.Name,
                                                             chatuser.Email, chatuser.Phone,
                                                             chatuser.PasswordRecoveryType.ToString(),
                                                             chatuser.PasswordRecoveryQuestion,
                                                             chatuser.PasswordRecoveryAnswer, 
                                                             chatuser.ImageTitle, chatuser.ImageContent);
                if (data.code.Equals("200"))
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Login", string.Format("Couldn't register user. {0}", data.message));
            }

			ViewBag.PossibleImages = imagefileRepository.All;
			return View();
        }
        
        //
        // GET: /ChatUsers/Edit/5
 
        public ActionResult Edit(int id)
        {
			ViewBag.PossibleImages = imagefileRepository.All;
             return View(chatuserRepository.Find(id));
        }

        //
        // POST: /ChatUsers/Edit/5

        [HttpPost]
        public ActionResult Edit(ChatUser chatuser)
        {
            if (ModelState.IsValid) {
                chatuserRepository.InsertOrUpdate(chatuser);
                chatuserRepository.Save();
                return RedirectToAction("Index");
            } else {
				ViewBag.PossibleImages = imagefileRepository.All;
				return View();
			}
        }

        //
        // GET: /ChatUsers/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(chatuserRepository.Find(id));
        }

        //
        // POST: /ChatUsers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            chatuserRepository.Delete(id);
            chatuserRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //imagefileRepository.Dispose();
                //chatuserRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

