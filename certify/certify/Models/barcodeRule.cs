using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace certify.Models
{
    [Table("barcodeRule")]
    public class barcodeRule
    {
        [Key]
        public string barcode { get; set; }

        public int positionRule { set; get; }
    }
}