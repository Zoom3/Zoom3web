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
    
    public partial class NotificationType
    {
        public NotificationType()
        {
            this.Notifications = new HashSet<Notifications>();
        }
    
        public int NT_Id { get; set; }
        public string NT_Type { get; set; }
    
        public virtual ICollection<Notifications> Notifications { get; set; }
    }
}
