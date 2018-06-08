using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aitag.Models
{
    public class Password
    {

        [Required(ErrorMessage = "請輸入密碼")]
        public string epwd { get; set; }

    }
}