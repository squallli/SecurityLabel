using Aitag.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace certify.Models
{
    public class AitagDBContext : DbContext
    {
        public AitagDBContext()
            : base("name=AitagDBContext")
        {
        }

        public virtual DbSet<certifystampstock> Certifystampstock { get; set; }
        public virtual DbSet<looknumber> lookNumber { set; get; }
        public virtual DbSet<farmer> Farmer { set; get; }
        public virtual DbSet<certifyorder> certifYorder { set; get; }
        public virtual DbSet<viewcertifycheckdet> viewCertifycheckdet { set; get; }
        public virtual DbSet<barcodeRule> BarcodeRule { set; get; }
    }
}