using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zoom3Web.Models;

namespace Zoom3Web.Views.Upload
{
    public class UploadController : Controller
    {

        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult Upload()
        {
            ViewBag.PH_CAT_Id = new SelectList(db.Category, "CAT_Id", "CAT_Description");
            ViewBag.PH_US_Id = new SelectList(db.UserData, "US_Id", "US_Email");

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View();
        }

        /// <summary>
        /// Save an Image into a DataBase
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {
            var user = Convert.ToInt32(Session["UserId"]);
            if (Request != null)
            {
                

                Photo ph = new Photo();
                HttpPostedFileBase file = Request.Files["PH_Image"];

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + file.FileName);

                    ph.PH_CAT_Id = Convert.ToInt32(formCollection["PH_CAT_Id"]);
                    ph.PH_Description = formCollection["PH_Description"];
                    ph.PH_Title = formCollection["PH_Title"];
                    ph.PH_Price = Convert.ToDecimal(formCollection["PH_Price"]);
                    var carro = formCollection["PH_InMarket"];
                    bool inMarket = obtainCartValue(carro);
                    if (!userWithAccount(user, inMarket))
                    {
                        ViewBag.PH_CAT_Id = new SelectList(db.Category, "CAT_Id", "CAT_Description");
                        var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
                        ViewBag.noti = not;
                        ModelState.AddModelError("", "Para poner a la venta debe introducir un número de cuenta");
                        return View();
                    }
                    ph.PH_InMarket = Convert.ToBoolean(inMarket);
                    
                    ph.PH_FileName = fileName;
                    ph.PH_Image = fileBytes;
                    ph.PH_US_Id = Convert.ToInt32(Session["UserId"]);
                    ph.PH_UploadDate = DateTime.Now;

                    
                    try
                    {
                        db.Photo.Add(ph);
                        db.SaveChanges();

                        
                        var query = db.Photo.Where(u => u.PH_US_Id == user);
                        Session["PhotosNo"] = query.Count();
                        return RedirectToAction("Index", "Home");
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
        /// Check If User has Account Number
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

        public bool obtainCartValue(string inCart)
        {
            bool result = false;
            if (inCart.StartsWith("t")) result = true;
            else result = false;

            return result;
        }

        /// <summary>
        /// Check if User has Account Number
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult checkUserAccount(int? user)
        {
            var usuario = Convert.ToInt32(Session["UserId"]);
            JsonResult result = Json("error", JsonRequestBehavior.AllowGet);

            var dbUser = db.UserData.Find(usuario);
            var creditCard = db.CreditCard.Where(c=>c.CC_U_Id == usuario);
            //Check if User has Bank Dates
            if(creditCard.Count()>0){
                 var UsercreditCard = db.CreditCard.Where(c=>c.CC_U_Id == usuario).Single();
                //Check if user has account number
                 if (UsercreditCard.CC_AccountNumber != null)
                {
                    result = Json(new
                    {
                        modal = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result = Json(new
                    {
                        modal = false
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                result = Json(new
                {
                    modal = false
                }, JsonRequestBehavior.AllowGet);
            }

           
            return result;
        }
    }
}
