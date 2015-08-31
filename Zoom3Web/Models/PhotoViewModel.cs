using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zoom3Web.Models
{
    public class PhotoViewModel
    {
        public PhotoViewModel()
        {
            this.Email = new HashSet<Email>();
            this.Favourites = new HashSet<Favourites>();
            this.Notifications = new HashSet<Notifications>();
        }

        public HttpPostedFileBase File { get; set;}

        public int PH_Id { get; set; }
        public string PH_Title { get; set; }
        public string PH_Description { get; set; }
        public Nullable<decimal> PH_Price { get; set; }
        public int PH_Favourites { get; set; }
        public bool PH_InCart { get; set; }
        public bool PH_InMarket { get; set; }
        public Nullable<int> PH_CAT_Id { get; set; }
        public int PH_US_Id { get; set; }
        public byte[] PH_Image { get; set; }
        public string PH_Path { get; set; }
        public string PH_FileName { get; set; }
        public Nullable<System.DateTime> PH_UploadDate { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual ICollection<Email> Email { get; set; }
        public virtual ICollection<Favourites> Favourites { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; }
        public virtual UserData UserData { get; set; }
    }
}