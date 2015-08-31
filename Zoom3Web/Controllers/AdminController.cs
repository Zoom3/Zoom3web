using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class AdminController : Controller
    {
        ZOOM3Entities db = new ZOOM3Entities();
        //
        // GET: /Admin/

        public ActionResult AdminPhotos(int? page)
        {
           
            var usuario = Convert.ToInt32(Session["UserId"]);

            var photos = db.Photo.ToList();

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(photos.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult AdminUsers(int? page)
        {
            var users = db.UserData.ToList();

            var usuario = Convert.ToInt32(Session["UserId"]);
            var purchases = db.Purchases.Where(p => p.PUR_US_Id == usuario);
            ViewBag.purchases = purchases.Count();

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Delete User Data in Cascade
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteUser(int id)
        {
            deleteNotifications(id);
            deletePhotos(id);
            deleteCreditCard(id);

            var users = db.UserData.Where(n => n.US_Id == id).Single();
            try
            {
                db.UserData.Remove(users);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

        /// <summary>
        /// Delete Cascade Photo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeletePhoto(int id)
        {
            deleteNotificationsByPhoto(id);
            deletePurchasesByPhoto(id);
            deleteCartByPhoto(id);
            deleteFavouritesByPhoto(id);

            var photos = db.Photo.Where(n => n.PH_Id == id).Single();
            try
            {
                db.Photo.Remove(photos);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return View();
        }

        /// <summary>
        /// Delete Notifications by User
        /// </summary>
        /// <param name="id"></param>
        public void deleteNotifications(int id){
            var not = db.Notifications.Where(n => n.NOT_U_Id == id).ToList();

            if (not.Count() > 0)
            {
                try
                {
                    foreach (var n in not)
                    {
                        db.Notifications.Remove(n);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
               
        }

        /// <summary>
        /// Delete Photos By User
        /// </summary>
        /// <param name="id"></param>
        public void deletePhotos(int id)
        {
            var photos = db.Photo.Where(n => n.PH_Id == id).ToList();
            if (photos.Count() > 0)
            {
                try
                {
                    foreach (var ph in photos)
                    {
                        deletePurchasesByPhoto(ph.PH_Id);
                        deleteCartByPhoto(ph.PH_Id);
                        deleteFavouritesByPhoto(ph.PH_Id);
                        db.Photo.Remove(ph);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
            
        }

        /// <summary>
        /// Delete Credit Card Data by User
        /// </summary>
        /// <param name="id"></param>
        public void deleteCreditCard(int id)
        {
            var card = db.CreditCard.Where(n => n.CC_U_Id == id).ToList();
            if (card.Count() > 0)
            {
                try
                {
                    foreach (var c in card)
                    {
                        db.CreditCard.Remove(c);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }
            
        }

        /// <summary>
        /// Delete Notifications By Photo
        /// </summary>
        /// <param name="id"></param>
        public void deleteNotificationsByPhoto(int id)
        {
            var not = db.Notifications.Where(n => n.NOT_PH_Id == id).ToList();

            if (not.Count() > 0)
            {
                try
                {
                    foreach (var n in not)
                    {
                        db.Notifications.Remove(n);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }

        }

        /// <summary>
        /// Delete Photo Purchases
        /// </summary>
        /// <param name="id"></param>
        public void deletePurchasesByPhoto(int id)
        {
            var purc = db.Purchases.Where(n => n.PUR_PH_Id == id).ToList();

            if (purc.Count() > 0)
            {
                try
                {
                    foreach (var p in purc)
                    {
                        db.Purchases.Remove(p);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }

        }

        /// <summary>
        /// Delete Shopping Cart By Photo
        /// </summary>
        /// <param name="id"></param>
        public void deleteCartByPhoto(int id)
        {
            var cart = db.Cart.Where(n => n.C_PH_Id == id).ToList();

            if (cart.Count() > 0)
            {
                try
                {
                    foreach (var c in cart)
                    {
                        db.Cart.Remove(c);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }

        }

        /// <summary>
        /// Delete Favourites By Photo
        /// </summary>
        /// <param name="id"></param>
        public void deleteFavouritesByPhoto(int id)
        {
            var fav = db.Favourites.Where(n => n.FAV_PH_Id == id).ToList();

            if (fav.Count() > 0)
            {
                try
                {
                    foreach (var f in fav)
                    {
                        db.Favourites.Remove(f);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    RedirectToAction("ErrorPage", "Error");
                }
            }

        }


    }
}
