using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace certify.Models
{
    [Table("viewcertifycheckdet")]
    public class viewcertifycheckdet
    {
        
        [Key]
        public string farmerno { get; set; }

        public string ltitle { get; set; }

        public string landno1 { set; get; }
    }
}