using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XChat.Filters;
using XChat.Helpers;
using XChat.Models.DB;
using XChat.Models;
using XChat.Models.ViewModels;
using WebMatrix.WebData;

namespace XChat.Controllers
{
    [CustomAuthorize(Roles = CustomAuthorizeAttribute.ROLE_ADMIN)]
    public class UserProfilesController : BaseController
    {
		private readonly IDataFileRepository datafileRepository;
		private readonly IUserProfileRepository userprofileRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public UserProfilesController() : this(new DataFileRepository(), new UserProfileRepository())
        {
        }

        public UserProfilesController(IDataFileRepository imagefileRepository, IUserProfileRepository userprofileRepository)
        {
            this.datafileRepository = imagefileRepository;
			this.userprofileRepository = userprofileRepository;
        }

        private void SaveMainImage(HttpPostedFileBase imageFile, UserProfile user)
        {
            string filename = string.Format(@"{0}{1}", Guid.NewGuid().ToString("N"), Path.GetExtension(imageFile.FileName));
            imageFile.SaveAs(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["ImagesPath"], filename));
            DataFile file = new DataFile();
            file.LocalFileName = filename;
            file.OriginalFileName = filename;
            datafileRepository.InsertOrUpdate(file);
            user.Image = file;
        }

        private void PopulateRolesData(UserProfile user)
        {
            var allRoles = Roles.GetAllRoles();
            var userRoles = new HashSet<string>();
            if (user != null)
            {
                userRoles = new HashSet<string>(Roles.GetRolesForUser(user.UserName));
            }
            var viewModel = new List<AssignedData>();
            foreach (var role in allRoles)
            {
                viewModel.Add(new AssignedData()
                {
                    Uid = role,
                    Title = Helper.LocalizeRole(role),
                    Assigned = userRoles.Contains(role)
                });
            }
            ViewBag.UserRoles = viewModel;
        }

        private void UpdateRoles(string[] selected, UserProfile userToUpdate)
        {

            if (selected == null)
            {
                return;
            }

            var userRoles = new HashSet<string>(Roles.GetRolesForUser(userToUpdate.UserName));
            var allRoles = Roles.GetAllRoles();

            foreach (var role in allRoles)
            {
                if (selected.Contains(role))
                {
                    if (!userRoles.Contains(role))
                    {
                        Roles.AddUserToRole(userToUpdate.UserName, role);
                    }
                }
                else
                {
                    if (userRoles.Contains(role))
                    {
                        Roles.RemoveUserFromRole(userToUpdate.UserName, role);
                    }
                }
            }
        }


        //
        // GET: /UserProfiles/

        public ViewResult Index()
        {
            return View(userprofileRepository.AllIncluding(userprofile => userprofile.Image));
        }

        //
        // GET: /UserProfiles/Details/5

        public ViewResult Details(int id)
        {
            return View(userprofileRepository.Find(id));
        }

        //
        // GET: /UserProfiles/Create

        public ActionResult Create()
        {
            PopulateRolesData(null);
            ViewBag.PossibleImages = datafileRepository.All;
            return View(new UserProfile());
        } 

        //
        // POST: /UserProfiles/Create

        [HttpPost]
        public ActionResult Create(UserProfile userprofile, FormCollection formCollection, string[] selectedRoles)
        {
            HttpPostedFileBase imageFile = userprofile.ImageFile;
            if (imageFile == null && userprofile.ImageId == null)
            {
                ModelState.AddModelError("ImageFile", "Изображение должно быть загружено");
            }
            if (ModelState.IsValid) {
                if (imageFile != null)
                {
                    SaveMainImage(imageFile, userprofile);
                }
                userprofileRepository.InsertOrUpdate(userprofile);
                userprofileRepository.Save();

                WebSecurity.CreateAccount(userprofile.UserName, userprofile.Password);
                UpdateRoles(selectedRoles, userprofile);

                return RedirectToAction("Index");
            } else {
                PopulateRolesData(null);
                ViewBag.PossibleImages = datafileRepository.All;
				return View(userprofile);
			}
        }
        
        //
        // GET: /UserProfiles/Edit/5
 
        public ActionResult Edit(int id)
        {
            var user = userprofileRepository.Find(id);
            PopulateRolesData(user);
            ViewBag.PossibleImages = datafileRepository.All;
             return View(user);
        }

        //
        // POST: /UserProfiles/Edit/5

        [HttpPost]
        public ActionResult Edit(UserProfile userprofile, FormCollection formCollection, string[] selectedRoles)
        {
            HttpPostedFileBase imageFile = userprofile.ImageFile;
            if (imageFile == null && userprofile.ImageId == null)
            {
                ModelState.AddModelError("ImageFile", "Изображение должно быть загружено");
            }
            if (ModelState.IsValid) {
                
                if (imageFile != null)
                {
                    SaveMainImage(imageFile, userprofile);
                }

                userprofileRepository.InsertOrUpdate(userprofile);
                userprofileRepository.Save();

                UpdateRoles(selectedRoles, userprofile);

                return RedirectToAction("Index");
            } else {
                ViewBag.PossibleImages = datafileRepository.All;
                PopulateRolesData(userprofile);
                return View(userprofileRepository.Find(userprofile.Id));
			}
        }

        //
        // GET: /UserProfiles/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(userprofileRepository.Find(id));
        }

        //
        // POST: /UserProfiles/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            userprofileRepository.Delete(id);
            userprofileRepository.Save();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //imagefileRepository.Dispose();
                //userprofileRepository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

