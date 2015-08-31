using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zoom3Web.Models
{
    public class CreditCardViewModel
    {
        public int CC_Id { get; set; }
        public string CC_Number { get; set; }
        public Nullable<int> CC_CVV2 { get; set; }
        public string CC_Name { get; set; }
        public Nullable<bool> CC_Primary { get; set; }
        public Nullable<bool> CC_SaveCard { get; set; }
        public Nullable<int> CC_CT_Id { get; set; }
        public int CC_U_Id { get; set; }
        public string CC_AccountNumber { get; set; }

        [Required]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Entidad no válida")]
        public string CC_Entity { get; set; }

        [Required]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Oficina no válida")]
        public string CC_Office { get; set; }
        [Required]
        [RegularExpression("^[0-9]{2}$", ErrorMessage = "Dígito de control No válido")]
        public string CC_ControlDigit { get; set; }
        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Número de cuenta no válido")]
        public string CC_ANumber { get; set; }

        public Nullable<int> CC_ExpiredMonth { get; set; }
        public Nullable<int> CC_ExpiredYear { get; set; }

        public virtual CreditCardType CreditCardType { get; set; }
        public virtual UserData UserData { get; set; }
    }
}