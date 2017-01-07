using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using XChat.Helpers;
using XChat.Models;
using XChat.Models.ViewModels;

namespace XChat.Controllers
{
    public class RecoverController : BaseController
    {
        //
        // GET: /Recover/

        [HttpGet]
        public ActionResult Password(string key)
        {
            var recoverRepository = new RecoverPasswordRequestRepository();

            var request = recoverRepository.Get(u => u.RequestKey.Equals(key) && u.IsActive).FirstOrDefault();
            try
            {
                if (request == null)
                {
                    throw new Exception("Данного запроса не существует");
                }

                var userRepository = new ChatUserRepository();
                var user = userRepository.Get(e => e.Id == request.UserId).FirstOrDefault();

                if (user == null)
                {
                    throw new Exception("Произошла ошибка в обработке запроса. Обратитесь в службу поддержки");
                }

                return View(new RecoverPasswordModel { RequestId = request.Id, RequestKey = key, UserLogin = user.Login });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Recover", new { error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Password(RecoverPasswordModel requestModel)
        {
            var recoverRepository = new RecoverPasswordRequestRepository();

            var request = recoverRepository.Get(u => u.Id == requestModel.RequestId 
                && u.RequestKey == requestModel.RequestKey 
                && u.IsActive).FirstOrDefault();

            try
            {
                if (request == null)
                {
                    throw new Exception("Данного запроса не существует");
                }

                var userRepository = new ChatUserRepository();
                var user = userRepository.Get(e => e.Id == request.UserId).FirstOrDefault();

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

                userRepository.Update(user);
                userRepository.Save();

                //request.IsActive = false;
                //recoverRepository.Update(request);
                //recoverRepository.Save();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Recover", new { error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Error(string error)
        {
            return View(model: error);
        }
    }
}
