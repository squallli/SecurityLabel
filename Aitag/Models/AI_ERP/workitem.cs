namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("workitem")]
    public partial class workitem
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(5)]
        public string corp_no { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(5)]
        public string work_no_code { get; set; }

        [StringLength(50)]
        public string workitemname { get; set; }

        [StringLength(500)]
        public string workitemcomment { get; set; }

         [StringLength(200)]
        public string hourgroup { get; set; }
        

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }
    }
}
