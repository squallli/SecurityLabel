namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("allcompany")]
    public partial class allcompany
    {
      
        [Key]
        [Required]
        [StringLength(20)]
        public string comid { get; set; }

        [StringLength(2)]
        public string comstatus { get; set; }

        [StringLength(1)]
        public string cflag { get; set; }

        [StringLength(10)]
        public string clid { get; set; }

        [StringLength(20)]
        public string comsno { get; set; }

        [StringLength(20)]
        public string comtype { get; set; }

        [StringLength(20)]
        public string comflag { get; set; }

        [StringLength(250)]
        public string comtitle { get; set; }

        [StringLength(250)]
        public string comentitle { get; set; }

        [StringLength(250)]
        public string comsttitle { get; set; }

        [StringLength(250)]
        public string comstentitle { get; set; }

        [StringLength(3)]
        public string compost { get; set; }

        [StringLength(3)]
        public string comcity { get; set; }

        [StringLength(250)]
        public string comadd { get; set; }

        [StringLength(250)]
        public string comenadd { get; set; }

        [StringLength(3)]
        public string invcity { get; set; }

        [StringLength(3)]
        public string invpost { get; set; }

        [StringLength(250)]
        public string invadd { get; set; }

        [StringLength(100)]
        public string ownman { get; set; }

        [StringLength(100)]
        public string ownenman { get; set; }

        [StringLength(100)]
        public string comemail { get; set; }

        [StringLength(100)]
        public string comhttp { get; set; }

        [StringLength(20)]
        public string taxno { get; set; }

        public DateTime? setdate { get; set; }

        [StringLength(20)]
        public string laborno { get; set; }

        [StringLength(20)]
        public string healthno { get; set; }

        [StringLength(20)]
        public string salesno { get; set; }

        [StringLength(50)]
        public string genmgr { get; set; }

        [StringLength(50)]
        public string genenmgr { get; set; }

        [StringLength(100)]
        public string comtel { get; set; }

        [StringLength(100)]
        public string comfax { get; set; }

        public int? licensecount { get; set; }

        public DateTime? deadline { get; set; }

        [StringLength(3)]
        public string comno { get; set; }

        [StringLength(50)]
        public string pid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(4)]
        public string Curyearterm { get; set; }

        [StringLength(2)]
        public string Curcyc { get; set; }

        [StringLength(20)]
        public string sycode { get; set; }

        [StringLength(20)]
        public string syscode { get; set; }

        [StringLength(20)]
        public string syecode { get; set; }

        [StringLength(20)]
        public string ljcode { get; set; }

        public int? fileno { get; set; }

        [StringLength(10)]
        public string acomid { get; set; }

        [StringLength(20)]
        public string ownmantel { get; set; }

        [StringLength(20)]
        public string ownmanmob { get; set; }

        [StringLength(20)]
        public string genmgrmob { get; set; }

        [StringLength(20)]
        public string genmgrtel { get; set; }

        [StringLength(50)]
        public string bankaccount { get; set; }

        public int? ppayday { get; set; }

        public int? pmoneyday { get; set; }

        [StringLength(2)]
        public string ifinv { get; set; }

        [StringLength(2)]
        public string iftransfer { get; set; }

        [StringLength(2)]
        public string iftransfertype { get; set; }

        [StringLength(20)]
        public string capital { get; set; }

        [StringLength(20)]
        public string cwdcarea { get; set; }

        [StringLength(20)]
        public string salesman { get; set; }

        [StringLength(500)]
        public string salescontent { get; set; }

        [StringLength(10)]
        public string comclass { get; set; }

        [StringLength(10)]
        public string comlevel { get; set; }

        [StringLength(500)]
        public string comcomment { get; set; }

        [StringLength(10)]
        public string comid1 { get; set; }

        [StringLength(2)]
        public string contracttype { get; set; }

        [StringLength(20)]
        public string salesmoney { get; set; }

        [StringLength(2)]
        public string ifcomb1 { get; set; }

        [StringLength(2)]
        public string ifcomb2 { get; set; }

        public int? id1 { get; set; }

        public int? comscore { get; set; }

        [StringLength(50)]
        public string comwebsite { get; set; }

        [StringLength(50)]
        public string comacctitle { get; set; }

        [StringLength(2)]
        public string ifstop { get; set; }

        public DateTime? badddate { get; set; }

        [StringLength(10)]
        public string baddid { get; set; }

        [StringLength(10)]
        public string servicepart1 { get; set; }
        [StringLength(10)]
        public string servicepart2 { get; set; }
        [StringLength(10)]
        public string servicepart3 { get; set; }
        [StringLength(10)]
        public string servicepart4 { get; set; }
        [StringLength(2)]
        public string ifwork { get; set; }
        [StringLength(2)]
        public string iftax { get; set; }
        [StringLength(2)]
        public string paytype { get; set; }
        [StringLength(10)]
        public string paypart1 { get; set; }
        [StringLength(10)]
        public string paypart2 { get; set; }
        [StringLength(2)]
        public string docday { get; set; }
        [StringLength(100)]
        public string doctype { get; set; }
        [StringLength(20)]
        public string newbillno { get; set; }
    }
}
