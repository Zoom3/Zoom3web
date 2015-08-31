using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Zoom3Web.Controllers
{
    public class MarketController : Controller
    {
        //
        // GET: /Market/

        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult Market(int? page)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_InMarket == true).Take(12);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }
            ViewBag.mainPhotos = ph;

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(ph.ToPagedList(pageNumber, pageSize));

        }

    }
}
