//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zoom3Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserData
    {
        public UserData()
        {
            this.CreditCard = new HashSet<CreditCard>();
            this.Notifications = new HashSet<Notifications>();
            this.Photo = new HashSet<Photo>();
        }
    
        public int US_Id { get; set; }
        public string US_Email { get; set; }
        public string US_Name { get; set; }
        public string US_LastName { get; set; }
        public string US_Password { get; set; }
        public Nullable<System.DateTime> US_Birth { get; set; }
        public Nullable<int> US_Phone { get; set; }
        public bool RememberMe { get; set; }
        public int US_ROL_Id { get; set; }
        public string US_ConfirmPassword { get; set; }
        public byte[] US_ImageProfile { get; set; }
        public bool US_HasImage { get; set; }
        public string US_NewPassword { get; set; }
        public System.DateTime US_RegDate { get; set; }
    
        public virtual ICollection<CreditCard> CreditCard { get; set; }
        public virtual ICollection<Notifications> Notifications { get; set; }
        public virtual ICollection<Photo> Photo { get; set; }
        public virtual Roles Roles { get; set; }
    }
}