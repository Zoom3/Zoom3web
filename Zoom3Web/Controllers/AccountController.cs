using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Zoom3Web.Filters;
using Zoom3Web.Models;
using System.Threading.Tasks;
using System.Web.Mail;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;

namespace Zoom3Web.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        ZOOM3Entities db = new ZOOM3Entities();
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserDataViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ZOOM3Entities db = new ZOOM3Entities()){
                   

                    var query = db.UserData.Where(e => e.US_Email.Equals(model.US_Email) && e.US_Password.Equals(model.US_Password)).FirstOrDefault();
                    if (query != null)
                    {
                        Session["UserId"] = query.US_Id;
                        Session["UserName"] = query.US_Name;
                        Session["UserLastname"] = query.US_LastName;
                        Session["PhotosNo"] = query.Photo.Count();
                        Session["ProfilePhoto"] = query.US_HasImage;
                        var cartUnits = db.Cart.Where(u => u.C_US_Id == query.US_Id);
                        var not = db.Notifications.Where(u => u.NOT_U_Id == query.US_Id && u.NOT_Leido == false);
                        Session["Cart"] = cartUnits.Count();
                        Session["Notifications"] = not.Count();
                        Session["Rol"] = query.US_ROL_Id;
                        ViewBag.noti = not;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email o Contraseña Incorrectos");
                        return View();
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }


        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserDataViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    // Attempt to register the user
            //    try
            //    {
            //        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
            //        WebSecurity.Login(model.UserName, model.Password);
            //        return RedirectToAction("Index", "Home");
            //    }
            //    catch (MembershipCreateUserException e)
            //    {
            //        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
            var email = 0;
            //Check if the email exits
            var query = (
               from c in db.UserData
               where c.US_Email == model.US_Email
               select c).Count();

            if (email == 0)
            {
                try
                {
                    model.RememberMe = false;
                    model.US_ROL_Id = 1;
                    if (ModelState.IsValid)
                    {
                        AutoMapper.Mapper.CreateMap<UserDataViewModel, UserData>();
                        var usuario = AutoMapper.Mapper.Map<UserDataViewModel, UserData>(model);
                        Session["UserId"] = model.US_Id;
                        Session["UserName"] = model.US_Name;
                        Session["UserLastname"] = model.US_LastName;
                        Session["PhotosNo"] = model.Photo.Count();
                        Session["ProfilePhoto"] = model.US_HasImage;
                        var cartUnits = db.Cart.Where(u => u.C_US_Id == model.US_Id);
                        var not = db.Notifications.Where(u => u.NOT_U_Id == model.US_Id && u.NOT_Leido == false);
                        Session["Cart"] = cartUnits.Count();
                        Session["Notifications"] = not.Count();
                        usuario.US_RegDate = DateTime.Now;

                        db.UserData.Add(usuario);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine("{0} Exception caught.", e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
            else
            {
                ModelState.AddModelError("", "El Usuario ya existe");
                //RedirectToAction("Error");
            }

            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
           // ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        //message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = "" });
        }

        //
        // GET: /Account/Manage

        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : "";
        //    ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        ////
        //// POST: /Account/Manage

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Manage(LocalPasswordModel model)
        //{
        //    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
        //    ViewBag.HasLocalPassword = hasLocalAccount;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasLocalAccount)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // ChangePassword will throw an exception rather than return false in certain failure scenarios.
        //            bool changePasswordSucceeded;
        //            try
        //            {
        //                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
        //            }
        //            catch (Exception)
        //            {
        //                changePasswordSucceeded = false;
        //            }

        //            if (changePasswordSucceeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a local password so remove any validation errors caused by a missing
        //        // OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            catch (Exception)
        //            {
        //                ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
        //            }
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // POST: /Account/ExternalLogin

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback

        //[AllowAnonymous]
        //public ActionResult ExternalLoginCallback(string returnUrl)
        //{
        //    AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //    if (!result.IsSuccessful)
        //    {
        //        return RedirectToAction("ExternalLoginFailure");
        //    }

        //    if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
        //    {
        //        return RedirectToLocal(returnUrl);
        //    }

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // If the current user is logged in add the new account
        //        OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // User is new, ask for their desired membership name
        //        string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
        //        ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
        //        ViewBag.ReturnUrl = returnUrl;
        //        return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        //return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(UserData model)
        {
            var user = db.UserData.Where(u=>u.US_Email.Equals(model.US_Email));
            if(user.Count() == 0){
                ModelState.AddModelError("","El Usuario no Existe");
            }
            else
            {
                var aux = user.Single();
                var userid = aux.US_Id.ToString();
                string url = "http://localhost:51065/Account/RecoverPassword/" + userid;
                if (ModelState.IsValid)
                {
                    var body = "<p>Email From: {0} ({1})</p><p style = 'color:red;'>Message:</p><p>{2}</p><a href={3}>Recuperar Contraseña</a>";
                    //var body = "<img src='../EmailTemplate2.gif' alt='emailTemplate'/><div class='email-content'><h2>Recuperar Contraseña</h2><hr/>";
                    //body += "<p>Para recuperar su contraseña pulse aquí:</p><div>{0}</div></div><style>.email-content{min-height: 490px;width: 70%;border: 2px solid #A1B2A6;";
                    //body += "border-radius: 5px;position: absolute;margin-top: -635px;padding: 10px;margin-left: 43px;background-color:white;}</style>";
                    var message = new System.Net.Mail.MailMessage();
                    message.To.Add(new MailAddress(model.US_Email));  // replace with valid value 
                    message.From = new MailAddress("zoomthreeweb@gmail.com");  // replace with valid value
                    message.Subject = "Recuperar Contraseña";
                    string emailMessage = "Para recuperar su contraseña, pinche en el siguiente enlace";
                    message.Body = string.Format(body, "Zoom3 - Admin", message.From, emailMessage, url);
                    message.IsBodyHtml = true;
                    //Send Files
                    //message.Attachments.Add(new Attachment(HttpContext.Server.MapPath("~/App_Data/Test.docx")));

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "zoomthreeweb@gmail.com",  // replace with valid value
                            Password = "realmadrid.15"  // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        try
                        {

                            await smtp.SendMailAsync(message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            RedirectToAction("ErrorPage", "Error");
                        }
                        return RedirectToAction("Index", "Home");
                    }
            }
            
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RecoverPassword(int? id)
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecoverPassword(UserData user, int? id)
        {
            var userdata = db.UserData.Where(c => c.US_Id == id).Single();
            if (user.US_Password.Equals(userdata.US_Password) && user.US_NewPassword.Equals(user.US_ConfirmPassword))
            {
                userdata.US_Password = user.US_NewPassword;
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(userdata).State = EntityState.Modified;
                        db.SaveChanges();
                        Session["UserId"] = userdata.US_Id;
                        Session["UserName"] = userdata.US_Name;
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        RedirectToAction("ErrorPage", "Error");
                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "Contraseñas no coinciden");
            }

            return View();
        }


    }
}
