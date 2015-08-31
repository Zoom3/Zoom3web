using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zoom3Web.Controllers
{
    public class NotificationsController : Controller
    {
        ZOOM3Entities db = new ZOOM3Entities();
        // GET: /Notifications/

        public ActionResult Notifications(int id)
        {
            var notId = db.Notifications.Find(id);

            Photo ph = new Photo();
            ph = db.Photo.Find(notId.NOT_PH_Id);

            var usuario = Convert.ToInt32(Session["UserId"]);

            var likes = db.Likes.Where(l => l.LIK_PH_Id == id && l.LIK_US_Id == usuario).Count();
            if (likes > 0)
            {
                ViewBag.buttonLike = false;
            }
            else
            {
                ViewBag.buttonLike = true;
            }

            var favs = db.Favourites.Where(f => f.FAV_PH_Id == id && f.FAV_US_Id == usuario).Count();
            if (favs > 0)
            {
                ViewBag.buttonFav = false;
            }
            else
            {
                ViewBag.buttonFav = true;
            }

            updateNotifications(id);
            diminishNot(usuario);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        /// <summary>
        /// Set a Notification as Read
        /// </summary>
        /// <param name="id"></param>
        /// <param name="usuario"></param>
        public void updateNotifications(int id)
        {
            var not = db.Notifications.Find(id);

            not.NOT_Leido = true;
           
            try
            {
                db.Entry(not).State = EntityState.Modified;
                db.SaveChanges();
            } catch(Exception e){
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }
        }

        public void diminishNot( int usuario)
        {
            var notif = db.Notifications.Where(n => n.NOT_U_Id == usuario && n.NOT_Leido == false).Count();
            Session["Notifications"] = notif;
        }

    }
}
