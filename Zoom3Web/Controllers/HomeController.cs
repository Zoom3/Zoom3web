using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Zoom3Web.Models;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class HomeController : Controller
    {
        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult Index(int? page)
        {
            var usuario = Convert.ToInt32(Session["UserId"]);
            List<Photo> ph = new List<Photo>();
            var photoList =
                       from ph1 in db.Photo
                       orderby ph1.PH_UploadDate descending
                       select ph1;
            //var photoList = db.Photo.Where(c => c.PH_US_Id == 5).Take(12);
            //photoList.Take(16);

            foreach(var p in photoList){
                ph.Add(p);
            }
            ViewBag.mainPhotos = ph;
            ViewBag.notifications = 0;
            ViewBag.cartItems = 0;
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

       

        public ActionResult Contact()
        {
            var usuario = Convert.ToInt32(Session["UserId"]);

            List<Category> cat = ListaEmail();

            ViewBag.QueryType = new SelectList(cat, "CAT_Id", "CAT_Description");

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Contact(FormCollection formcollection)
        {
            var email = formcollection["UserEmail"];
            var mensaje = formcollection["ContactMessage"];
            var user = db.UserData.Where(u => u.US_Email.Equals(email));
            if (user.Count() == 0)
            {
                ModelState.AddModelError("", "El Usuario no Existe");
            }
            else
            {
                var aux = user.Single();
                var userid = aux.US_Id.ToString();
                if (ModelState.IsValid)
                {
                    var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                    var message = new System.Net.Mail.MailMessage();
                    message.To.Add(new MailAddress("zoomthreeweb@gmail.com"));  
                    message.From = new MailAddress(aux.US_Email); 
                    message.Subject = "Contacto Zoom3 -" + formcollection["QueryType"];
                    string emailMessage = "Para recuperar su contraseña, pinche en el siguiente enlace";
                    message.Body = string.Format(body, "Zoom3 - Contact", message.From, emailMessage);
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "zoomthreeweb@gmail.com",  
                            Password = "realmadrid.15"  
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

        public ActionResult FAQ()
        {
            var usuario = Convert.ToInt32(Session["UserId"]);
            ViewBag.Message = "Your app description page.";
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;
            return View();
        }

        public ActionResult GetImage(int id)
        {
            byte[] imageData = ReturnImage(id);

            //instead of what augi wrote using the binarystreamresult this was the closest thing i found so i am assuming that this is what it evolved into 
            return new FileStreamResult(new System.IO.MemoryStream(imageData), "image/jpeg");
        }

        //in my repository class where i have all the methods for accessing data i have this

        public byte[] ReturnImage(int id)
        {
            // i tried his way of selecting the right record and preforming the toArray method in the return statment 
            // but it kept giving me an error about converting linq.binary to byte[] tried a cast that didnt work so i came up with this
            byte[] imageData = db.Photo.Find(id).PH_Image.ToArray();
            return imageData;
        }

        public ActionResult LogOff(int id)
        {
            Session["UserName"] = null;
            Session["UserId"] = null;
            Session["Cart"] = 0;
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Option List for Contact
        /// </summary>
        /// <returns></returns>
        public static List<Category> ListaEmail()
        {
            List<Category> catList = new List<Category>();

            var cat = new Category
            {
                CAT_Id = 4,
                CAT_Description = "Compra"
            };

            var cat2 = new Category
            {
                CAT_Id = 5,
                CAT_Description = "Venta"
            };

            var cat3 = new Category
            {
                CAT_Id = 6,
                CAT_Description = "Cuenta"
            };

            var cat4 = new Category
            {
                CAT_Id = 7,
                CAT_Description = "Otros"
            };

            catList.Add(cat);
            catList.Add(cat2);
            catList.Add(cat3);
            catList.Add(cat4);

            return catList;
        }

    }
}
