using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aitag.Models
{
    public class Logoin
    {

        [Required(ErrorMessage = "請輸入帳號")]
         public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password
        {
            get;
            set;
        }
        

    }

    public class Comlogin
    {

        [Required(ErrorMessage = "請輸入帳號")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password
        {
            get;
            set;
        }


    }
}