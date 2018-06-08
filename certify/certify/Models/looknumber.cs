using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace certify.Models
{
    [Table("looknumber")]
    public class looknumber
    {
        [StringLength(30)]
        public string barcode
        {
            get;
            set;
        }

        public DateTime? bmoddate
        {
            get;
            set;
        }

        public int? lookno
        {
            get;
            set;
        }

        [Key]
        public int noid
        {
            get;
            set;
        }

        public looknumber()
        {
        }
    }
}