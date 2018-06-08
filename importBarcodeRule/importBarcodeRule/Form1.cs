using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace importBarcodeRule
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
            string filePath = System.Configuration.ConfigurationSettings.AppSettings["filePath"];

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

                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            string str = "";
                            while ((str = sr.ReadLine()) != null)
                            {
                                if (str.Trim() == "") continue;
                                cmd.CommandText = "insert into barcodeRule(barcode,positionRule) values(" +
                                                 "'" + str.Split(',')[0] + "'," + str.Split(',')[1] + ")";
                                cmd.ExecuteNonQuery();
                            }
                        }


                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
