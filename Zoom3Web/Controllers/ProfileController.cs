using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoom3Web.Models;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult ProfileInfo(int? id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();
            //var userdata = db.UserData.Where(c => c.US_Id == 5).Single();
           
            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userdata);
        }

        public ActionResult EditProfile()
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userdata);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(UserData user)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            //var userdata = db.UserData.Where(c => c.US_Id == 5).Single();
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();
            userdata.US_Name = user.US_Name;
            userdata.US_LastName = user.US_LastName;
            userdata.US_Email = user.US_Email;
            userdata.US_Birth = user.US_Birth;
            userdata.US_Phone = user.US_Phone;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(userdata).State = EntityState.Modified;
                    Session["UserName"] = user.US_Name;
                    Session["UserLastname"] = user.US_LastName;
                    db.SaveChanges();
                    return RedirectToAction("ProfileInfo");
                } catch(Exception e){
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
               
            }
            return View();
        }


        public ActionResult GetImage(int id)
        {
            byte[] imageData = ReturnImage(id);

            return new FileStreamResult(new System.IO.MemoryStream(imageData), "image/jpeg");
        }



        public byte[] ReturnImage(int id)
        {
           byte[] imageData = db.UserData.Find(id).US_ImageProfile.ToArray();
           return imageData;
        }

        public ActionResult ChangePassword()
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userdata);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserData user)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            //var userdata = db.UserData.Where(c => c.US_Id == 5).Single();
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();
            if (user.US_Password.Equals(userdata.US_Password) && user.US_NewPassword.Equals(user.US_ConfirmPassword))
            {
                userdata.US_Password = user.US_NewPassword;
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Entry(userdata).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("ProfileInfo");
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

        public ActionResult MyMarket()
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();

            UserData auxUser = new UserData();

            var photoList = db.Photo.Where(c => c.PH_US_Id == usId && c.PH_InMarket == true);
            foreach (var p in photoList)
            {
                auxUser.Photo.Add(p);
            }
            ViewBag.myPhotos = userdata.Photo;

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(auxUser);
        }


        public ActionResult MyPhotos()
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();

            var photoList = db.Photo.Where(c => c.PH_US_Id == usId);
            foreach (var p in photoList)
            {
                userdata.Photo.Add(p);
            }
            ViewBag.myPhotos = userdata.Photo;

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userdata);
        }

        public ActionResult MyPurchases(int? page)
        {
            var user = Convert.ToInt32(Session["UserId"]);
            var cart = db.Purchases.Where(c => c.PUR_US_Id == user);
            var photos = new List<Photo>();
            //decimal total = 0;
            foreach (var i in cart)
            {
                var ph = db.Photo.Find(i.PUR_PH_Id);
                photos.Add(ph);
                //total += Convert.ToDecimal(ph.PH_Price);
            }
            //ViewBag.Total = total;

            var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(photos.ToPagedList(pageNumber, pageSize));

            return View(photos);
        }

        public ActionResult DeleteImage(int id)
        {
            try
            {
                var image = db.Photo.Find(id);
                db.Photo.Remove(image);
                db.SaveChanges();

                var user = Convert.ToInt32(Session["UserId"]);
                var ph = db.Photo.Where(u => u.PH_US_Id == id);
                Session["PhotosNo"] =  ph.Count();
                return RedirectToAction("MyPhotos");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

        public ActionResult DeleteProfile(int id)
        {
            try
            {
                var user = db.UserData.Find(id);
                db.UserData.Remove(user);
                db.SaveChanges();
                Session["UserId"]= null;
                Session["UserName"] = null;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }


        public ActionResult EditImage(int id)
        {

            var usId = Convert.ToInt32(Session["UserId"]);
            var usuarioId = db.UserData.Find(usId);
            UserData userdata = new UserData();
            userdata.US_LastName = usuarioId.US_LastName;
            userdata.US_LastName = usuarioId.US_Name;
            var photo = db.Photo.Where(p=>p.PH_Id == id).Single();
            ViewBag.photoIdUser = photo.PH_Id;
            //ViewBag.photo = photo;
            userdata.Photo.Add(photo);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            ViewBag.PH_CAT_Id = new SelectList(db.Category, "CAT_Id", "CAT_Description");
            return View(userdata);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditImage(Photo photouser, int id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();
            var ph = userdata.Photo.Where(p => p.PH_Id == id).Single();
            ph.PH_Title = photouser.PH_Title;
            ph.PH_Description = photouser.PH_Description;
            ph.PH_InMarket = photouser.PH_InMarket;

            if(!userWithAccount(usId, ph.PH_InMarket)){
                ViewBag.PH_CAT_Id = new SelectList(db.Category, "CAT_Id", "CAT_Description");
                ViewBag.photoIdUser = ph.PH_Id;
                var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
                ViewBag.noti = not;
                ModelState.AddModelError("", "Para poner a la venta debe introducir un número de cuenta");
                return View();
            }
            ph.PH_Price = photouser.PH_Price;
            ph.PH_CAT_Id = photouser.PH_CAT_Id;
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(ph).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MyPhotos");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }

            }
            return View();
        }

        public ActionResult ChangeProfileImage()
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var userdata = db.UserData.Where(c => c.US_Id == usId).Single();

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userdata);
        }

        [HttpPost]
        public ActionResult ChangeProfileImage(FormCollection formCollection)
        {
            if (Request != null)
            {
                var usId = Convert.ToInt32(Session["UserId"]);
                var userd = db.UserData.Where(c => c.US_Id == usId).Single();
                HttpPostedFileBase file = Request.Files["US_ImageProfile"];

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + file.FileName);
                    userd.US_ImageProfile = fileBytes;
                    userd.US_HasImage = true;
                    try
                    {
                        db.Entry(userd).State = EntityState.Modified;
                        db.SaveChanges();

                        Session["ProfilePhoto"] = userd.US_HasImage;
                        return RedirectToAction("ProfileInfo");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        RedirectToAction("ErrorPage", "Error");
                    }
                }
            }

            return View("Index", "Home");
        }

        /// <summary>
        /// Check if user has account
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="inCart"></param>
        /// <returns></returns>
        public bool userWithAccount(int usuario, bool inCart)
        {
            bool result = false;

            //Check if photo have price
            if (inCart == true)
            {
                var account = db.CreditCard.Where(c => c.CC_U_Id == usuario);
                if (account.Count() > 0)
                {
                    var cuenta = account.Single();
                    if (cuenta.CC_AccountNumber != null && cuenta.CC_AccountNumber != "")
                    {
                        result = true;
                    }
                }
            }

            return result;

        }

        /// <summary>
        /// Edit Credit Card and Account User Information
        /// </summary>
        /// <returns></returns>
        public ActionResult CreditCardEdit(int? id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var editcc = db.CreditCard.Find(id);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;
            ViewBag.CC_CT_Id = new SelectList(db.UserData, "CT_Id", "CT_Type");
            return View(editcc);
        }

        [HttpPost]
        public ActionResult CreditCardEdit(CreditCard cc)
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;
            ViewBag.CC_CT_Id = new SelectList(db.UserData, "CT_Id", "CT_Type");

            var ccUser = db.CreditCard.Find(cc);
            ccUser.CC_Name = cc.CC_Name;
            ccUser.CC_Number = cc.CC_Number;
            ccUser.CC_ExpiredYear = cc.CC_ExpiredYear;
            ccUser.CC_ExpiredMonth = cc.CC_ExpiredMonth;
            ccUser.CC_CVV2 = cc.CC_CVV2;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(ccUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("CreditCardInfo");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }

            }
            return View();
        }

        /// <summary>
        /// Credit Card Info
        /// </summary>
        /// <returns></returns>
        public ActionResult CreditCardInfo()
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            var creditCard = db.CreditCard.Where(c => c.CC_U_Id == usId);
            if (creditCard.Count() > 0)
            {
                var cc = creditCard.First();
                return View(cc);
            }

            return View();
        }


        public ActionResult EditAccountNumber(int? id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);
            var editcc = db.CreditCard.Find(id);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;
            ViewBag.CC_CT_Id = new SelectList(db.UserData, "CT_Id", "CT_Type");

            AutoMapper.Mapper.CreateMap<CreditCard, CreditCardViewModel>();
            var cc = AutoMapper.Mapper.Map<CreditCard, CreditCardViewModel>(editcc);
            cc.CC_Entity = editcc.CC_AccountNumber.Substring(0, 4);
            cc.CC_Office = editcc.CC_AccountNumber.Substring(4, 4);
            cc.CC_ControlDigit = editcc.CC_AccountNumber.Substring(8, 2);
            cc.CC_ANumber = editcc.CC_AccountNumber.Substring(10, 10);

            return View(cc);
        }


        [HttpPost]
        public ActionResult EditAccountNumber(CreditCardViewModel cc)
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;
            ViewBag.CC_CT_Id = new SelectList(db.UserData, "CT_Id", "CT_Type");

            var number = cc.CC_Entity + cc.CC_Office + cc.CC_ControlDigit + cc.CC_ANumber;

            var ccUser = db.CreditCard.Find(cc.CC_Id);
            ccUser.CC_AccountNumber = number;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(ccUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("CreditCardInfo");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }

            }
            return View();
        }


        public ActionResult AddAccount()
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View();
        }

        [HttpPost]
        public ActionResult AddAccount(CreditCardViewModel cc)
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;
           
            var numero = cc.CC_Entity + cc.CC_Office + cc.CC_ControlDigit + cc.CC_ANumber;

            var findCC = db.CreditCard.Where(u => u.CC_U_Id == usId);
            if (findCC.Count() > 1)
            {
                findCC.Single();
                try
                {
                    db.Entry(findCC).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("CreditCardInfo");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
            else
            {
                CreditCard newCC = new CreditCard();
                newCC.CC_AccountNumber = numero;
                newCC.CC_U_Id = usId;

                try
                {
                    db.CreditCard.Add(newCC);
                    db.SaveChanges();
                    return RedirectToAction("CreditCardInfo");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
            


            return View();
        }


        /// <summary>
        /// Delete Credit Card
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCreditCard(int id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            var cc = db.CreditCard.Find(id);
            cc.CC_Name = null;
            cc.CC_CVV2 = null;
            cc.CC_ExpiredMonth = null;
            cc.CC_ExpiredYear = null;
            cc.CC_Number = null;

            try
            {
                db.Entry(cc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CreditCardInfo");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

        /// <summary>
        /// Delete Account Number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAccountNumber(int id)
        {
            var usId = Convert.ToInt32(Session["UserId"]);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usId && u.NOT_Leido == false);
            ViewBag.noti = not;

            var cc = db.CreditCard.Find(id);
            cc.CC_AccountNumber = null;

            try
            {
                db.Entry(cc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CreditCardInfo");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

    }
}
