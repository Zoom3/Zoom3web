using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class CategoriesController : Controller
    {
        //
        // GET: /Categories/
        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult Arquitecture(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 3);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Art(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 4);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Movies(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 5);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Food(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 6);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Sport(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 7);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Music(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 10);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Nature(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 11);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult People(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 13);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Transport(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 14);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Other(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 16);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Landscape(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_CAT_Id == 12);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult MostPopular(int? page)
        {
            var usuario = Convert.ToInt32(Session["UserId"]);
            List<Photo> ph = new List<Photo>();
            var photoList =
                       from ph1 in db.Photo
                       orderby ph1.PH_Likes descending
                       select ph1;

            foreach (var p in photoList)
            {
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

        public ActionResult GenericCategory(int? cat, int? page)
        {

            List<Photo> ph = new List<Photo>();
            if (cat == 1)
            {
                var photoList =
                       from ph1 in db.Photo
                       orderby ph1.PH_Likes descending
                       select ph1;
                foreach (var p in photoList)
                {
                    ph.Add(p);
                }
            }

            else
            {
                var photoList = db.Photo.Where(c => c.PH_CAT_Id == cat);
                foreach (var p in photoList)
                {
                    ph.Add(p);
                }
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));
        }
    }
}
