namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("employee")]
    public partial class employee
    {
        [Key]
        public int emid { get; set; }

        [StringLength(2)]
        public string ifuse { get; set; }

        [StringLength(1)]
        public string ifapply { get; set; }

        [StringLength(2)]
        public string emptype { get; set; }

        [StringLength(1)]
        public string empstatus { get; set; }

        public DateTime? usedate { get; set; }

        [StringLength(10)]
        public string empno { get; set; }

        [StringLength(20)]
        public string empid { get; set; }

        [StringLength(20)]
        public string cardno { get; set; }

        [StringLength(10)]
        public string empworkcomp { get; set; }

        [StringLength(10)]
        public string empworkdepid { get; set; }

        [StringLength(20)]
        public string emppasswd { get; set; }

        [StringLength(50)]
        public string empname { get; set; }

        [StringLength(50)]
        public string empenname { get; set; }

        [StringLength(2)]
        public string empsex { get; set; }

        public DateTime? empbirth { get; set; }

        public DateTime? jobdate { get; set; }

        [StringLength(100)]
        public string eaddress { get; set; }

        [StringLength(100)]
        public string enaddress { get; set; }

        [StringLength(20)]
        public string entel { get; set; }

        [StringLength(30)]
        public string enmob { get; set; }

        [StringLength(100)]
        public string enemail { get; set; }

        [StringLength(10)]
        public string msid { get; set; }

        [StringLength(500)]
        public string empcomment { get; set; }

        public int? logincount { get; set; }

        [StringLength(50)]
        public string efromip { get; set; }

        [StringLength(10)]
        public string epagesize { get; set; }

        [StringLength(2)]
        public string eicon { get; set; }

        public DateTime? logindate { get; set; }

        public int? logintime { get; set; }

        [StringLength(50)]
        public string loginip { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(1)]
        public string eifpriv { get; set; }

        [StringLength(10)]
        public string etab { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? inboxsize { get; set; }

        [StringLength(20)]
        public string ltoken { get; set; }

        [StringLength(10)]
        public string yhid { get; set; }

        [StringLength(200)]
        public string empmeetsign { get; set; }

        [StringLength(10)]
        public string joblevel1 { get; set; }

        public int? hsalary { get; set; }

        [StringLength(2)]
        public string empflag { get; set; }

        [StringLength(2)]
        public string empclass { get; set; }

        [StringLength(50)]
        public string empsttitle { get; set; }

        [StringLength(100)]
        public string econaddress { get; set; }

        [StringLength(20)]
        public string efax { get; set; }

        [StringLength(50)]
        public string empper { get; set; }

        [StringLength(10)]
        public string empperidno { get; set; }

        [StringLength(20)]
        public string emppertel { get; set; }

        [StringLength(20)]
        public string emppermob { get; set; }

        [StringLength(100)]
        public string empperadd { get; set; }

        [StringLength(50)]
        public string empcon { get; set; }

        [StringLength(20)]
        public string empconrelation { get; set; }

        [StringLength(20)]
        public string empcontel { get; set; }

        [StringLength(20)]
        public string empconmob { get; set; }

        [StringLength(100)]
        public string empconadd { get; set; }

        public DateTime? cdate { get; set; }

        public DateTime? empperbirth { get; set; }

        [StringLength(20)]
        public string cardno1 { get; set; }

        [StringLength(1)]
        public string ifholdata { get; set; }

        [StringLength(20)]
        public string adaccount { get; set; }

        [StringLength(20)]
        public string studylevel { get; set; }

        [StringLength(50)]
        public string school { get; set; }

        [StringLength(20)]
        public string empidno { get; set; }

        public DateTime? leavedate { get; set; }

        [StringLength(1000)]
        public string comcon { get; set; }
    }
}
