using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace UniteErp.Models
{
    public partial class UniteErpBill_DBContext : DbContext
    {
        static UniteErpBill_DBContext()
        {
            Database.SetInitializer<UniteErpBill_DBContext>(null);
        }

        public UniteErpBill_DBContext()
            : base("Name=UniteErpBill_DBContext")
        {
        }

        public DbSet<vend_contractinv> vend_contractinv { get; set; }
        public DbSet<vend_contractinv_det> vend_contractinv_det { get; set; }
        public DbSet<incomeaccounts> incomeaccounts { get; set; }
        public DbSet<accountvoucher> accountvoucher { get; set; }
        public DbSet<billsubject> billsubject { get; set; }
        public DbSet<erpbilldoc> erpbilldoc { get; set; }

        public DbSet<mediaclass> mediaclass { get; set; }
        public DbSet<mediachannel> mediachannel { get; set; }
        public DbSet<allcompany> allcompany { get; set; }
        public DbSet<allcompany_media> allcompany_media { get; set; }
        public  DbSet<conbudgetdet> conbudgetdet { get; set; }
        public  DbSet<custproduct> custproduct { get; set; }
        public  DbSet<purchase> purchase { get; set; }
        public  DbSet<purchase_det> purchase_det { get; set; }
        public  DbSet<vend_contract> vend_contract { get; set; }
        public  DbSet<vend_contractdet> vend_contractdet { get; set; }

       
      //  public DbSet<vw_DocSearch_all> vw_DocSearch_all { get; set; }

      
        
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Configurations.Add(new AdbannerMap());
        //    modelBuilder.Configurations.Add(new AdworkbillMap());
        //    modelBuilder.Configurations.Add(new CategoryMap());
        //    modelBuilder.Configurations.Add(new CategoryclassMap());
        //  modelBuilder.Configurations.Add(new billsubjectMap());
        //    modelBuilder.Configurations.Add(new CompayMap());
        //    modelBuilder.Configurations.Add(new CouponbookbillMap());
        //    modelBuilder.Configurations.Add(new EmployeeMap());
        //    modelBuilder.Configurations.Add(new EpaperMap());
        //    modelBuilder.Configurations.Add(new EventMap());
        //    modelBuilder.Configurations.Add(new EventbillMap());
        //    modelBuilder.Configurations.Add(new EventbilldaydetMap());
        //    modelBuilder.Configurations.Add(new EventdaydetMap());
        //    modelBuilder.Configurations.Add(new ExhibtselectbillMap());
        //    modelBuilder.Configurations.Add(new PrivroleMap());
        //    modelBuilder.Configurations.Add(new PrivtbMap());
        //    modelBuilder.Configurations.Add(new RentobjectMap());
        //    modelBuilder.Configurations.Add(new SernoMap());
        //    modelBuilder.Configurations.Add(new SublevelMap());
        //    modelBuilder.Configurations.Add(new SystemlogMap());
        //    modelBuilder.Configurations.Add(new TempcardbillMap());
        //    modelBuilder.Configurations.Add(new TempcardbilldetMap());
        //    modelBuilder.Configurations.Add(new WebmaincontentMap());
        //}
    }
}
