namespace Aitag.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AitagBill_DBContext : DbContext
    {
        public AitagBill_DBContext()
        {
            Database.SetInitializer<AitagBill_DBContext>(null);
        }

        public virtual DbSet<accountvoucher> accountvoucher { get; set; }
        public virtual DbSet<allcompany> allcompany { get; set; }
        public virtual DbSet<allcompany_media> allcompany_media { get; set; }
        public virtual DbSet<allcompany_rate> allcompany_rate { get; set; }
        public virtual DbSet<billsubject> billsubject { get; set; }
        public virtual DbSet<bonusrate> bonusrate { get; set; }
        public virtual DbSet<conbudgetdet> conbudgetdet { get; set; }
        public virtual DbSet<custproduct> custproduct { get; set; }
        public virtual DbSet<custpurchase> custpurchase { get; set; }
        public virtual DbSet<custpurchase_det> custpurchase_det { get; set; }
        public virtual DbSet<erpbilldoc> erpbilldoc { get; set; }
        public virtual DbSet<incomeaccounts> incomeaccounts { get; set; }
        public virtual DbSet<mediachannel> mediachannel { get; set; }
        public virtual DbSet<mediaclass> mediaclass { get; set; }
        public virtual DbSet<mediaitemtype> mediaitemtype { get; set; }

        public virtual DbSet<purchase> purchase { get; set; }
        public virtual DbSet<purchase_det> purchase_det { get; set; }

        public virtual DbSet<purchasemod> purchasemod { get; set; }
        public virtual DbSet<purchasemod_det> purchasemod_det { get; set; }
        public virtual DbSet<vend_contract> vend_contract { get; set; }
        public virtual DbSet<vend_contractdet> vend_contractdet { get; set; }
        public virtual DbSet<vend_contractinv> vend_contractinv { get; set; }
        public virtual DbSet<vend_contractinv_det> vend_contractinv_det { get; set; }

        public virtual DbSet<vend_contractinvclose> vend_contractinvclose { get; set; }
        public virtual DbSet<vend_contractinvclose_det> vend_contractinvclose_det { get; set; }

        public virtual DbSet<cust_contractinvclose> cust_contractinvclose { get; set; }
        public virtual DbSet<cust_contractinvclose_det> cust_contractinvclose_det { get; set; }
        public virtual DbSet<view_mcno_allcompany_media> view_mcno_allcompany_media { get; set; }
        public virtual DbSet<vend_monthmoney> vend_monthmoney { get; set; }

        public DbSet<workcard> workcard { get; set; }
        public DbSet<workcard_det> workcard_det { get; set; }
        public virtual DbSet<workhour> workhour { get; set; }
        public virtual DbSet<workitem> workitem { get; set; }
        public virtual DbSet<sales_competition> sales_competition { get; set; }
    


    }
}
