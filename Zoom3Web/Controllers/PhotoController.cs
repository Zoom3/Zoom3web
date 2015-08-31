using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zoom3Web.Controllers
{
    public class PhotoController : Controller
    {
        //
        // GET: /Photo/

        ZOOM3Entities db = new ZOOM3Entities();

        public ActionResult MainPage(int id)
        {
            List<Photo> ph = new List<Photo>();

            var photoList = db.Photo.Where(c => c.PH_US_Id == 5).Take(12);
            foreach (var p in photoList)
            {
                ph.Add(p);
            }
            calculateNextMainPhoto(id);
            ViewBag.idMainPhoto = id;

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        public ActionResult PhotoViewer(int id)
        {
            Photo ph = new Photo();
            ph = db.Photo.Find(id);

            var usuario = Convert.ToInt32(Session["UserId"]);
           
            var likes = db.Likes.Where(l => l.LIK_PH_Id == id && l.LIK_US_Id == usuario).Count();
            if(likes>0){
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

            calculateNextMainPhoto(id);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        /// <summary>
        /// Add Like to a Photo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SumLike(int id)
        {
            var ph = db.Photo.Find(id);
            var likes = ph.PH_Favourites +1;
            ph.PH_Likes = likes;


            int usuario = Convert.ToInt32(Session["UserId"]);

            var likeadd = new Likes();
            likeadd.LIK_PH_Id = id;
            likeadd.LIK_US_Id = usuario;
            likeadd.LIK_ButtonLike = false;

            ViewBag.buttonLike = false;
            try
            {
                db.Likes.Add(likeadd);
                db.Entry(ph).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }
            addToNotifications(id, 3);

            return RedirectToAction("PhotoViewer", "Photo", new { id = id});
        }

        /// <summary>
        /// Add photo to Favourites
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddToFavourites(int id)
        {
            int usuario = Convert.ToInt32(Session["UserId"]);
            ViewBag.buttonFav = false;
            if(usuario == 0 || usuario == null){
                ModelState.AddModelError("", "Para añadir a Favoritos es necesario estar registrado.");
            }
            else
            {
                var fav = new Favourites();
                fav.FAV_PH_Id = id;
                fav.FAV_US_Id = usuario;

                var ph = db.Photo.Find(id);
                var favs = ph.PH_Favourites + 1;
                ph.PH_Favourites = favs;
                

                var aux = db.Favourites.Where(f => f.FAV_US_Id == id && f.FAV_PH_Id == fav.FAV_US_Id);

                if (aux.Count() > 0)
                {
                    ModelState.AddModelError("", "Foto ya añadida a Favoritos");
                }
                else
                {
                    try
                    {
                        db.Entry(ph).State = EntityState.Modified;
                        db.Favourites.Add(fav);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        RedirectToAction("ErrorPage", "Error");
                    }
                }
                addToNotifications(id, 1);
            }
            return RedirectToAction("PhotoViewer", "Photo", new { id = id });
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
                db.SaveChanges();

                var noti = db.Notifications.Where(n => n.NOT_U_Id == photo.PH_US_Id && n.NOT_Leido == false);
                Session["Notifications"] = noti.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }
        }

        /// <summary>
        /// Add photo to Shopping Cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddToCart(int id)
        {
            var cart = new Cart();
            var usuario = Convert.ToInt32(Session["UserId"]);
            cart.C_US_Id = usuario;
            cart.C_PH_Id = id;
            var foto = db.Photo.Find(id);
            foto.PH_InCart = true;
            try
            {
                db.Entry(foto).State = EntityState.Modified;
                db.Cart.Add(cart);
                db.SaveChanges();

                var cartUnits = db.Cart.Where(u => u.C_US_Id == usuario);
                Session["Cart"] = cartUnits.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }
           

            return RedirectToAction("PhotoViewer", "Photo", new { id = id });
        }

        /// <summary>
        /// Diminish likes of a photo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DiminishLike(int id)
        {
            var like = db.Likes.Where(l=>l.LIK_PH_Id == id).Single();
            var ph = db.Photo.Find(id);
            ph.PH_Likes -= ph.PH_Likes;

            try
            {

                db.Entry(ph).State = EntityState.Modified;
                db.Likes.Remove(like);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return RedirectToAction("PhotoViewer", "Photo", new { id = id });
        }

        /// <summary>
        /// Remove to Favourites
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveToFavourites(int id)
        {
            var favs = db.Favourites.Where(l => l.FAV_PH_Id == id).Single();
            var ph = db.Photo.Find(id);
            ph.PH_Favourites -= ph.PH_Favourites;

            try
            {

                db.Entry(ph).State = EntityState.Modified;
                db.Favourites.Remove(favs);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                RedirectToAction("ErrorPage", "Error");
            }

            return RedirectToAction("PhotoViewer", "Photo", new { id = id });
        }


        public void calculateNextMainPhoto(int id)
        {
            var numQuery =
                       from ph1 in db.Photo
                       orderby ph1.PH_Likes descending
                       select ph1;

            numQuery.Take(16);
            var next = 0;
            var prev = 0;
            var current = 0;
            int[] positions = new int[numQuery.Count()];
            int i = 0;
            foreach (var item in numQuery)
            {
                var num = numQuery.Count();
                positions[i] = item.PH_Id;
                
                if (item.PH_Id == id)
                {
                    current = id;
                    if (i == 0)
                    {
                        prev = numQuery.Count() -1;
                        next = i + 1;
                    }
                    else if (i == numQuery.Count()-1)
                    {
                        next = 0;
                    }
                    else
                    {
                        next = i + 1;
                        prev = i - 1;
                    }
                }
                i++;
               
            }

            prev = positions[prev];
            next = positions[next];
           
            ViewBag.next = next;
            ViewBag.prev = prev;

        }


        public ActionResult MarketViewer(int id)
        {
            Photo ph = new Photo();
            ph = db.Photo.Find(id);

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

            calculateNextMarketPhoto(id);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        public void calculateNextMarketPhoto(int id)
        {
            var numQuery =
                       from ph1 in db.Photo
                       where(ph1.PH_InMarket == true)
                       select ph1;

            //numQuery.Take(16);
            var next = 0;
            var prev = 0;
            var current = 0;
            int[] positions = new int[numQuery.Count()];
            int i = 0;
            foreach (var item in numQuery)
            {
                var num = numQuery.Count();
                positions[i] = item.PH_Id;

                if (item.PH_Id == id)
                {
                    current = id;
                    if (i == 0)
                    {
                        prev = numQuery.Count() - 1;
                        next = i + 1;
                    }
                    else if (i == numQuery.Count() - 1)
                    {
                        next = 0;
                    }
                    else
                    {
                        next = i + 1;
                        prev = i - 1;
                    }
                }
                i++;

            }

            prev = positions[prev];
            next = positions[next];

            ViewBag.next = next;
            ViewBag.prev = prev;

        }

        public ActionResult FavouritesView(int id)
        {
            Photo ph = new Photo();
            ph = db.Photo.Find(id);

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

            calculateNextFavouritePhoto(id);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        public void calculateNextFavouritePhoto(int id)
        {
            var usuario = Convert.ToInt32(Session["UserId"]);
             
            var numQuery =
                       from ph1 in db.Favourites
                       where (ph1.FAV_US_Id == usuario)
                       select ph1;

            //numQuery.Take(16);
            var next = 0;
            var prev = 0;
            var current = 0;
            int[] positions = new int[numQuery.Count()];
            int i = 0;
            foreach (var item in numQuery)
            {
                var num = numQuery.Count();
                positions[i] = item.FAV_Id;
                if(positions.Length == 1){
                    next = 0;
                    prev = 0;
                }
                else
                {
                    if (item.FAV_Id == id)
                    {
                        current = id;
                        if (i == 0)
                        {
                            prev = numQuery.Count() - 1;
                            next = i + 1;
                        }
                        else if (i == numQuery.Count() - 1)
                        {
                            next = 0;
                        }
                        else
                        {
                            next = i + 1;
                            prev = i - 1;
                        }
                    }
                }
                
                i++;

            }

            prev = positions[prev];
            next = positions[next];

            ViewBag.next = next;
            ViewBag.prev = prev;

        }


        public ActionResult CategoryViewer(int id)
        {
            Photo ph = new Photo();
            ph = db.Photo.Find(id);
            int category = ph.PH_CAT_Id;

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

            calculateNextCategoryPhoto(id, category);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        public void calculateNextCategoryPhoto(int id, int category)
        {
            var usuario = Convert.ToInt32(Session["UserId"]);

            var numQuery =
                       from ph1 in db.Photo
                       where (ph1.PH_CAT_Id == category)
                       select ph1;

            //numQuery.Take(16);
            var next = 0;
            var prev = 0;
            var current = 0;
            int[] positions = new int[numQuery.Count()];
            int i = 0;
            foreach (var item in numQuery)
            {
                var num = numQuery.Count();
                positions[i] = item.PH_Id;
                if (positions.Length == 1)
                {
                    next = 0;
                    prev = 0;
                }
                else
                {
                    if (item.PH_Id == id)
                    {
                        current = id;
                        if (i == 0)
                        {
                            prev = numQuery.Count() - 1;
                            next = i + 1;
                        }
                        else if (i == numQuery.Count() - 1)
                        {
                            next = 0;
                        }
                        else
                        {
                            next = i + 1;
                            prev = i - 1;
                        }
                    }
                }

                i++;

            }

            prev = positions[prev];
            next = positions[next];

            ViewBag.next = next;
            ViewBag.prev = prev;

        }

        /// <summary>
        /// View all photos of a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PhotosByProfile(int id)
        {
            var userProfile = db.UserData.Find(id);

            var photosByUser = db.Photo.Where(i => i.PH_US_Id == userProfile.US_Id).ToList();

            foreach (var item in photosByUser)
            {
                userProfile.Photo.Add(item);
            }

            var usuario = Convert.ToInt32(Session["UserId"]);
            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(userProfile);
        }

        /// <summary>
        /// View Photo by User with options
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PhotosByProfileViewer(int id)
        {
            Photo ph = new Photo();
            ph = db.Photo.Find(id);
            var photoUser = ph.PH_US_Id;
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

            calculateNextProfilePhoto(id, photoUser);

            var not = db.Notifications.Where(u => u.NOT_U_Id == usuario && u.NOT_Leido == false);
            ViewBag.noti = not;

            return View(ph);
        }

        /// <summary>
        /// Calculate prev. and next photo of a User
        /// </summary>
        /// <param name="id"></param>
        public void calculateNextProfilePhoto(int id, int photoUser)
        {
            var numQuery =
                       from ph1 in db.Photo
                       where (ph1.PH_US_Id == photoUser)
                       select ph1;

            var next = 0;
            var prev = 0;
            int[] positions = new int[numQuery.Count()];
            int i = 0;
            foreach (var item in numQuery)
            {
                var num = numQuery.Count();
                positions[i] = item.PH_Id;
                if (positions.Length == 1)
                {
                    next = 0;
                    prev = 0;
                }
                else
                {
                    if (item.PH_Id == id)
                    {
                        if (i == 0)
                        {
                            prev = numQuery.Count() - 1;
                            next = i + 1;
                        }
                        else if (i == numQuery.Count() - 1)
                        {
                            next = 0;
                        }
                        else
                        {
                            next = i + 1;
                            prev = i - 1;
                        }
                    }
                }

                i++;

            }

            prev = positions[prev];
            next = positions[next];

            ViewBag.next = next;
            ViewBag.prev = prev;

        }


    }
}
