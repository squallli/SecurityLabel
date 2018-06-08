using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace certify.Models
{
    [Table("farmer")]
    public class farmer
    {
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

        [StringLength(10)]
        public string comid
        {
            get;
            set;
        }

        [StringLength(200)]
        public string faddr
        {
            get;
            set;
        }

        [StringLength(50)]
        public string farmername
        {
            get;
            set;
        }

        [Key]
        [StringLength(20)]
        public string farmerno
        {
            get;
            set;
        }

        [StringLength(500)]
        public string fcomment
        {
            get;
            set;
        }

        [StringLength(50)]
        public string fmob
        {
            get;
            set;
        }

        [StringLength(20)]
        public string fmpassword
        {
            get;
            set;
        }

        [StringLength(2)]
        public string fstatus
        {
            get;
            set;
        }

        [StringLength(50)]
        public string ftel
        {
            get;
            set;
        }

        [StringLength(2)]
        public string ftype
        {
            get;
            set;
        }

        [StringLength(10)]
        public string vcode
        {
            get;
            set;
        }

        [StringLength(100)]
        public string venmail
        {
            get;
            set;
        }

        [DataType("decimal(10 ,2")]
        public decimal? venwarehouse
        {
            get;
            set;
        }

    }
}