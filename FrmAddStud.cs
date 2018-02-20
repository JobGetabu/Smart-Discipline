using custom_alert_notifications;
using MetroFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartDiscipline
{
    public partial class FrmAddStud : Form
    {
        public FrmAddStud()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.gData.Rows.Count == -1)
            {
                alert.Show("No data to save ....", alert.AlertType.info);
            }
            alert.Show("Saving ....", alert.AlertType.info);
            using (var context = new SmartdbEntities())
            {
                for (int i = 0; i < this.gData.Rows.Count - 1; i++)
                {
                    DataGridViewRow dr = gData.Rows[i];
                    if (dr != null && (dr.Cells["RegNo"].Value != null) && (dr.Cells["FullName"].Value != null))
                    {

                        try
                        {
                            context.Students.Add(new Student
                            {
                                //check for duplicate Admin number 
                                Regno = dr.Cells["RegNo"].Value.ToString(),
                                FullName = dr.Cells["FullName"].Value.ToString(),
                                School = dr.Cells["School"].Value.ToString(),
                                SchoolCode = dr.Cells["SchoolCode"].Value.ToString(),
                                LearningMode = dr.Cells["LearningMode"].Value.ToString(),
                            });
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Check all entries are correct and retry", "Error");
                           // throw;
                        }
                    }
                    else
                    {
                        MetroMessageBox.Show(this, "Critical error contact developer immediately \n :( :( :( :( :( :(:( :( ", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                try
                {
                    context.SaveChanges();
                    alert.Show("Saved Successfully", alert.AlertType.success);
                }
                catch (Exception) { }
            }


            this.Close();
        }
    }
}
