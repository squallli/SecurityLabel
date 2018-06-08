using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace certify.Models
{
    [Table("certifyorder")]
    public class certifyorder
    {
        public DateTime? adddate
        {
            get;
            set;
        }

        public int? allmoney
        {
            get;
            set;
        }

        public DateTime? bmoddate
        {
            get;
            set;
        }

        [StringLength(20)]
        public string bmodid
        {
            get;
            set;
        }

        [StringLength(20)]
        public string certime
        {
            get;
            set;
        }

        public DateTime? cgivedate
        {
            get;
            set;
        }

        [StringLength(20)]
        public string citemid
        {
            get;
            set;
        }

        [StringLength(500)]
        public string cocomment
        {
            get;
            set;
        }

        [Key]
        [StringLength(20)]
        public string codno
        {
            get;
            set;
        }

        [StringLength(10)]
        public string comid
        {
            get;
            set;
        }

        [StringLength(2)]
        public string costatus
        {
            get;
            set;
        }

        public int? csid
        {
            get;
            set;
        }

        [StringLength(50)]
        public string doccheck
        {
            get;
            set;
        }

        [StringLength(20)]
        public string farmerno
        {
            get;
            set;
        }

        [StringLength(20)]
        public string vendno
        {
            get;
            set;
        }

    }
}