using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class FavouritesController : Controller
    {
        //
        // GET: /Favourites/
        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult Favourites(int? page)
        {
            List<Favourites> fa = new List<Favourites>();
            List<Photo> ph = new List<Photo>();
            
            db.Favourites.Include("Favourites.Photo").Include("Photo");
            var us = Convert.ToInt32(Session["UserId"]);
            var photoFavList = db.Favourites.Where(c => c.FAV_US_Id == us);
            foreach (var p in photoFavList)
            {
                fa.Add(p);
                var photoList = db.Photo.Where(c => c.PH_Id == p.FAV_PH_Id).Single();
                p.Photo = photoList;

            }
            ViewBag.favouritePhotos = fa;

            var not = db.Notifications.Where(u => u.NOT_U_Id == us && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(fa.ToPagedList(pageNumber, pageSize));
       
        }

    }
}
