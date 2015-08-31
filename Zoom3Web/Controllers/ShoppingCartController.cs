using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Zoom3Web.Controllers
{
    public class ShoppingCartController : Controller
    {

        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult CartStep1()
        {
            
            var user = Convert.ToInt32(Session["UserId"]);
            var cart = db.Cart.Where(c => c.C_US_Id == user);
            var photos = new List<Photo>();
            decimal total = 0;
            foreach (var i in cart)
            {
                var ph = db.Photo.Find(i.C_PH_Id);
                photos.Add(ph);
                total += Convert.ToDecimal(ph.PH_Price);
            }
            ViewBag.Total = total;

            var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(photos);
        }

        public ActionResult CartStep2()
        {

            var user = Convert.ToInt32(Session["UserId"]);
            var cart = db.Cart.Where(c => c.C_US_Id == user);
            var photos = new List<Photo>();
            decimal total = 0;
            foreach (var i in cart)
            {
                var ph = db.Photo.Find(i.C_PH_Id);
                photos.Add(ph);
                total += Convert.ToDecimal(ph.PH_Price);
            }
            ViewBag.Total = total;

            var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
            ViewBag.noti = not;


            var userInfo = new UserData();
            userInfo.Photo = photos;

            var cc = db.CreditCard.Where(u => u.CC_U_Id == user);
            if (cc.Count() > 1)
            {
                userInfo.CreditCard = cc.ToList();
            }
            List<CreditCard> ccnew = new List<CreditCard>();
            ccnew.Add(new CreditCard
            {
                CC_Id = 999,
                CC_U_Id= user
            });

            userInfo.CreditCard = ccnew;

            return View(userInfo);
        }


        [HttpPost]
        public ActionResult CartStep2(FormCollection formcollection)
        {

            var user = Convert.ToInt32(Session["UserId"]);
           
            var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
            ViewBag.noti = not;
            var carro = formcollection["CC_SaveCard"];
            bool save = obtainCartValue(carro);

            CreditCard cc = new CreditCard()
            {
                CC_U_Id = user,
                CC_CVV2 = Convert.ToInt32(formcollection["CC_CVV2"]),
                CC_ExpiredMonth = Convert.ToInt32(formcollection["CC_ExpiredMonth"]),
                CC_ExpiredYear = Convert.ToInt32(formcollection["CC_ExpiredYear"]),
                CC_Name = formcollection["CC_Name"].ToString(),
                CC_Number = formcollection["CC_Number"].ToString(),
                CC_SaveCard = save
            };

            if (cc.CC_SaveCard == true)
            {
                var existscc = db.CreditCard.Where(u => u.CC_U_Id == user);
                if (existscc.Count() > 1)
                {
                    var tarjeta = existscc.Single();
                    try
                    {
                        db.Entry(tarjeta).State = EntityState.Modified;
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
                    try
                    {
                        db.CreditCard.Add(cc);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        RedirectToAction("ErrorPage", "Error");
                    }
                }
                
            }


            return RedirectToAction("BuyCofirmation", "ShoppingCart");
        }


        public bool obtainCartValue(string inCart)
        {
            bool result = false;
            if (inCart.StartsWith("t")) result = true;
            else result = false;

            return result;
        }


        public ActionResult DeleteItem(int id)
        {
            try
            {
                var cart = db.Cart.Where(c=>c.C_Id == id ).Single();
                db.Cart.Remove(cart);
                Session["Cart"] = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

        public ActionResult BuyConfirmation()
        {
            return View();
        }

        public async Task<ActionResult> BuyCofirmation()
        {

            var user = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == user && u.NOT_Leido == false);
            ViewBag.noti = not;

            var cart = db.Cart.Where(c => c.C_US_Id == user);

            var ud = db.UserData.Find(user);

            UserData usuario = new UserData() 
            { 
                US_Email = ud.US_Email

            };
            
            //Remove items from shopping cart and add to User purchases
            foreach(var c in cart){

                var photoUs = db.Photo.Find(c.C_PH_Id);
                usuario.Photo.Add(photoUs);
                Purchases pur = new Purchases();
                try
                {
                    pur.PUR_PH_Id = c.C_PH_Id;
                    pur.PUR_US_Id = c.C_US_Id;
                    pur.PUR_DatePurchase = DateTime.Now;
                    pur.Photo = c.Photo;
                    db.Purchases.Add(pur);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    RedirectToAction("ErrorPage", "Error");
                }
                DeleteItem(c.C_Id);

                addToNotifications(c.C_PH_Id, 2);
            }


            db.SaveChanges();

            ActionResult x = await SendPhoto(usuario);
            return View();

        }

       

        /// <summary>
        /// Send Email With Photos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> SendPhoto(UserData model)
        {
            var user = db.UserData.Where(u => u.US_Email.Equals(model.US_Email));
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
                    var body = "<p>Email From: {0} ({1})</p><p>Mensaje:</p><p>{2}</p><div class='row'>{3}</div>{4}";
                    var message = new System.Net.Mail.MailMessage();
                    message.To.Add(new MailAddress(model.US_Email));  
                    message.From = new MailAddress("zoomthreeweb@gmail.com");  
                    message.Subject = "Su Compra";
                    string emailMessage = "Gracias por comprar en Zoom3. Aquí tiene su compra";
                    message.IsBodyHtml = true;
                    //Send Files
                    decimal total = 0;
                    var resume = "<table class='table table-responsive table-striped'><thead><tr><td>Foto</td><td>Autor</td><td>Precio</td></tr></thead>";
                    foreach (var p in model.Photo)
                    {
                        var fileName = p.PH_FileName.ToString();
                        byte[] resumeBytes = p.PH_Image;
                        message.Attachments.Add(new Attachment(new MemoryStream(resumeBytes), fileName));

                        resume += "<tr><td>" + p.PH_Title + "</td><td>" + p.UserData.US_Name + p.UserData.US_LastName + "</td><td>" + p.PH_Price + "</td></tr>";
                        total += Convert.ToDecimal(p.PH_Price);
                    }
                    resume += "<tr><td>Total</td><td></td><td>"+total+"</td></tr></table>";

                    var style = "<style></style>";

                    message.Body = string.Format(body, "Zoom3 - Admin", message.From, emailMessage, resume, style);

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
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }

            }
            return View();
        }



        /// <summary>
        /// Add To Notifications
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notType"></param>
        public void addToNotifications(int id, int notType)
        {
            var photo = db.Photo.Find(id);

            Notifications not = new Notifications();
            not.NOT_U_Id = photo.PH_US_Id;
            not.NOT_NT_Id = notType;
            not.NOT_PH_Id = id;
            not.NOT_Leido = false;
            not.NOT_Date = DateTime.Now;
            if (notType == 1)
            {
                not.NOT_Description = Session["UserName"].ToString() + Session["UserLastname"].ToString() + " Ha marcado como favorita una foto tuya";
            }
            else if (notType == 2)
            {
                not.NOT_Description = Session["UserName"].ToString() + Session["UserLastname"].ToString() + " Ha comprado una foto tuya";
            }
            else if (notType == 3)
            {
                not.NOT_Description = Session["UserName"].ToString() + Session["UserLastname"].ToString() + " Ha indicado que le gusta una foto tuya";
            }

            try
            {
                db.Notifications.Add(not);

                var noti = db.Notifications.Where(n => n.NOT_U_Id == photo.PH_US_Id && n.NOT_Leido == false);
                Session["Notifications"] = noti.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }
        }
    }
}