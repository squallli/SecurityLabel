using System.Data.Entity;
using System.Data.Entity.Infrastructure;


namespace Aitag.Models
{
    public partial class Aitag_DBContext : DbContext
    {
        static Aitag_DBContext()
        {
            Database.SetInitializer<Aitag_DBContext>(null);
        }

        public Aitag_DBContext()
            : base("Name=Aitag_DBContext")
        {
        }
        public DbSet<battalog> battalog { get; set; }
        public DbSet<billflow> billflow { get; set; }
        public DbSet<Checkcode> Checkcode { get; set; }
        public DbSet<cardreallog> cardreallog { get; set; }
        public DbSet<cardjudgelog> cardjudgelog { get; set; }
        public DbSet<cardlog> cardlog { get; set; }

        public virtual DbSet<delholidaylog> delholidaylog { get; set; }
        public DbSet<holidaylog> holidaylog { get; set; }
        public DbSet<holidaycode> holidaycode { get; set; }
        public DbSet<holidayschedule> holidayschedule { get; set; }
        public DbSet<Company> Company { get; set; }
        //public DbSet<cardlog> cardlog { get; set; }

        public DbSet<docgroup> docgroup { get; set; }
        public DbSet<employee> employee { get; set; }

        public DbSet<empfunction> empfunction { get; set; }
        public DbSet<emprole> emprole { get; set; }
        public DbSet<emphdlog> emphdlog { get; set; }
        public DbSet<mainclass> mainclass { get; set; }
        public DbSet<menutab> menutab { get; set; }
        public DbSet<otworklog> otworklog { get; set; }

        public DbSet<privrole> privrole { get; set; }
        public DbSet<privtb> privtb { get; set; }

        public DbSet<progparam> progparam { get; set; }
        public DbSet<roleplay> roleplay { get; set; }

        public DbSet<restlogdet> restlogdet { get; set; }
        public DbSet<resthourlog> resthourlog { get; set; }
        //public DbSet<restlogdet> restlogdet { get; set; }
        public DbSet<sublevel1> sublevel1 { get; set; }
        public DbSet<subreadwrite> subreadwrite { get; set; }
        public DbSet<systemlog> systemlog { get; set; }
      
        public DbSet<Department> Department { get; set; }
        
        public DbSet<contupload> contupload { get; set; }
        public DbSet<maincontent> maincontent { get; set; }
        public DbSet<schedule> schedule { get; set; }
        public DbSet<schupload> schupload { get; set; }
        //public DbSet<otworklog> otworklog { get; set; }
        public DbSet<yearholiday> yearholiday { get; set; }
        public DbSet<yearholidaydet> yearholidaydet { get; set; }
        public DbSet<yearhddet> yearhddet { get; set; }
        public virtual DbSet<otapply> otapply { get; set; }
        public virtual DbSet<tmpemphdlog> tmpemphdlog { get; set; }
        public virtual DbSet<bag> bag { get; set; }
        public virtual DbSet<certifycheck> certifycheck { get; set; }
        public virtual DbSet<certifycheckdet> certifycheckdet { get; set; }
        public virtual DbSet<certifychecklog> certifychecklog { get; set; }
        public virtual DbSet<certifyitem> certifyitem { get; set; }
        public virtual DbSet<certifystamp> certifystamp { get; set; }
        public virtual DbSet<certifystampstock> certifystampstock { get; set; }
        public virtual DbSet<farmer> farmer { get; set; }
        public virtual DbSet<farmwork> farmwork { get; set; }
        public virtual DbSet<landcode> landcode { get; set; }
        public virtual DbSet<landdata> landdata { get; set; }
        public virtual DbSet<village> village { get; set; }
        public virtual DbSet<carea> carea { get; set; }

       

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
