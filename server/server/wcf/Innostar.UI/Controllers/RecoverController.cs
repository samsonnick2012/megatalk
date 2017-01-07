using System;
using System.Linq;
using System.Web.Mvc;
using Innostar.Dal.Infrastructure;
using Innostar.Dal.Repositories;
using Innostar.UI.Helpers;
using Innostar.UI.ViewModels;

namespace Innostar.UI.Controllers
{
    public class RecoverController : Controller
    {
        //
        // GET: /Recover/

        [HttpGet]
        public ActionResult Password(string key)
        {
            using (var dataContext = new InnostarModelsContext())
            {
                var recoverRepository = new RecoverPasswordRequestRepository(dataContext);

                var requests = recoverRepository._Get(u => u.RequestKey.Equals(key) && u.IsActive).ToList();

                if (requests.Any())
                {
                    var request = requests.FirstOrDefault();

                    if (request == null)
                    {
                        throw new Exception("Данного запроса не существует");
                    }

                    var userRepository = new ChatUserRepository(dataContext);
                    var user = userRepository._Get(e => e.Id == request.UserId).FirstOrDefault();

                    if (user == null)
                    {
                        throw new Exception("Произошла ошибка в обработке запроса. Обратитесь в службу поддержки");
                    }

                    return View(new RecoverPasswordModel { RequestId = request.Id, RequestKey = key, UserLogin = user.Login });
                }
                else
                {
                    return RedirectToAction("NotFound", "Error");
                }
            }

        }

        [HttpPost]
        public ActionResult Password(RecoverPasswordModel requestModel)
        {
            using (var dataContext = new InnostarModelsContext())
            {
                var recoverRepository = new RecoverPasswordRequestRepository(dataContext);

                var request = recoverRepository._Get(u => u.Id == requestModel.RequestId
                                                         && u.RequestKey == requestModel.RequestKey
                                                         && u.IsActive).FirstOrDefault();


                if (request == null)
                {
                    throw new Exception("Данного запроса не существует");
                }

                var userRepository = new ChatUserRepository(dataContext);
                var user = userRepository._Get(e => e.Id == request.UserId).FirstOrDefault();

                if (user == null)
                {
                    throw new Exception("Произошла ошибка в обработке запроса. Обратитесь в службу поддержки");
                }

                if (!string.IsNullOrEmpty(requestModel.NewPassword))
                {
                    var pu = new PasswordUtility(requestModel.NewPassword.ToCharArray());
                    user.PasswordHash = pu.Hash;
                    user.PasswordSalt = pu.Salt;
                }

                userRepository._Update(user);
                userRepository._Save();

                //request.IsActive = false;
                //recoverRepository.Update(request);
                //recoverRepository.Save();

                return RedirectToAction("Index", "Home");
            }

        }


    }
}
