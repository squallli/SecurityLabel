using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace importExcelSerials
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationSettings.AppSettings["connectionString"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.Transaction = trans;

                        for (int i = 1; i <= 20000; i++)
                        {
                            cmd.CommandText = "insert into certifystampstock(csstatus,csid,cerno,barcode,cstime,comid,bmodid,bmoddate,csdate,codno,codid,randno,ifrand) values(" +
                                              "'0',35,'','AC" + i.ToString("00000") + "',1,'980','jf1','" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', " +
                                              "'" + DateTime.Now.ToString("yyyy/MM/dd") + "','',259,'1234','n')";
                            cmd.ExecuteNonQuery();
                        }

                    }

                    trans.Commit();
                }
                catch(Exception ex)
                {
                    trans.Rollback();
                }
                
            }
        }
    }
}
