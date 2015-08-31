using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zoom3Web.Models
{
    public class UserDataViewModel
    {
        public UserDataViewModel()
        {
            this.Cart = new HashSet<Cart>();
            this.CreditCard = new HashSet<CreditCard>();
            this.Notifications = new HashSet<Notifications>();
            this.Photo = new HashSet<Photo>();
        }
    
        public int US_Id { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string US_Email { get; set; }

        [StringLength(100, ErrorMessage = "El {0} de Usuario debe tener al menos {2} carácteres.", MinimumLength = 3)]
        [Display(Name = "Nombre")]
        public string US_Name { get; set; }

        [StringLength(100, ErrorMessage = "El apellido debe tener al menos {2} carácteres.", MinimumLength = 3)]
        public string US_LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} carácteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string US_Password { get; set; }
        public Nullable<System.DateTime> US_Birth { get; set; }
        public Nullable<int> US_Phone { get; set; }
        public bool RememberMe { get; set; }
        public int US_ROL_Id { get; set; }

        public byte[] US_ImageProfile { get; set; }

        public bool US_HasImage { get; set; }
        public string US_NewPassword { get; set; }

        public string US_ConfirmPassword { get; set; }
    
        public virtual ICollection<Cart> Cart { get; set; }
        public virtual ICollection<CreditCard> CreditCard { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; }
        public virtual ICollection<Photo> Photo { get; set; }
        public virtual Roles Roles { get; set; }

    }
}