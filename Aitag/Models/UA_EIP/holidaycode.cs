namespace Aitag.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("holidaycode")]
    public partial class holidaycode
    {
        [Key]
        [StringLength(10)]
        public string hdayid { get; set; }

        [StringLength(50)]
        public string hdaytitle { get; set; }

        [StringLength(10)]
        public string mergehdayid { get; set; }

        [StringLength(1)]
        public string ifdelholiday { get; set; }

        public int? cgivehour { get; set; }

        public int? mgivehour { get; set; }

        public int? sgivehour { get; set; }

        [StringLength(500)]
        public string cnote { get; set; }

        [StringLength(500)]
        public string mnote { get; set; }

        [StringLength(500)]
        public string snote { get; set; }

        [StringLength(10)]
        public string comid { get; set; }

        [StringLength(20)]
        public string bmodid { get; set; }

        public DateTime? bmoddate { get; set; }

        [StringLength(2)]
        public string ifdelsalary { get; set; }

        [StringLength(10)]
        public string salitemid { get; set; }

        [StringLength(200)]
        public string calculator { get; set; }

        [StringLength(100)]
        public string htype { get; set; }

        [StringLength(1)]
        public string ifholiday { get; set; }

        [StringLength(2)]
        public string hunit { get; set; }
    }
}
