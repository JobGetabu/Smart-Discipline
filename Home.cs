﻿using custom_alert_notifications;
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
    public partial class Home : Form
    {

        private int disLevel;
        private int disAction;

        public Home()
        {
            InitializeComponent();
        }

        private Student currentStudent;
        private async void Home_Load(object sender, EventArgs e)
        {
            SetToolTip(tbLegalInfo, $"Info: \n Keywords like Good, bad, improving, indisciplined, positive, questionable \n are being tracked to determine overall discipline");

            List<Student> studs = await Task.Factory.StartNew(() =>
            {
                using (var context = new SmartdbEntities())
                {
                    return context.Students.OrderBy(c => c.Regno)
                    .ToList();
                }
            });

            //first student in db default
            currentStudent = studs.FirstOrDefault();
            UIset(currentStudent);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            SetToolTip(tbLegalInfo, $"Info: \n Keywords like Good, bad, improving, indisciplined, positive, questionable \n are being tracked to determine overall discipline");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SetToolTip(Control ctl, string message)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.UseFading = true;
            toolTip1.UseAnimation = true;
            toolTip1.IsBalloon = true;
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(ctl, message);
        }

        private void btnChangePic_Click(object sender, EventArgs e)
        {

        }

        private void SetUpDrops(Disciplinary dsss)
        {
            //check drop values
            if (dsss.level > 79)
            {
                dropLevels.selectedIndex = 0;
            }
            if (dsss.level >50 & dsss.level < 80)
            {
                dropLevels.selectedIndex = 2;
            }
            if (dsss.level >= 0 & dsss.level < 50)
            {
                dropLevels.selectedIndex = 1;
            }

        }
        private async void UIset(Student fst)
        {
            lblName.Text = fst.FullName;
            lblSchoolAndNo.Text = $"{fst.School} - {fst.SchoolCode}";//EschoolKe Secondary - 20154245
            lblMOL.Text = $"Mode of Learning - {fst.LearningMode}";//Mode of Learning - Bording
            lblRegno.Text = $"Reg NO - {fst.Regno}";//Reg NO - 17052

            //look for data on discpline

            Disciplinary mDis = null;

            List<Disciplinary> studdis = await Task.Factory.StartNew(() =>
            {
                using (var context = new SmartdbEntities())
                {
                    return context.Disciplinaries
                    .Where(x => x.Regno == fst.Regno)
                    .ToList();
                }
            });

            //latest case
            mDis = studdis.OrderBy(x => x.Id).FirstOrDefault();

            if (mDis != null)
            {
                tbLegalInfo.Text = mDis.Details;
               // SetUpDrops(mDis)
            }

            //charts consume this data


        }

        private async Task<Student> StudFoundAsync(string regno)
        {
            var studentList = await Task.Factory.StartNew(() =>
           {
               using (var context = new SmartdbEntities())
               {
                   return context.Students.ToList();
               }
           });

            foreach (var stud in studentList.Where(a => a.Regno.Equals(regno)))
            {
                if (regno == stud.Regno)
                {
                    return stud;
                }
            }
            return null;
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {


            //do search set currentStudent
            if (tbSearch.Text != string.Empty)
            {
                currentStudent = await StudFoundAsync(tbSearch.Text);

                if (currentStudent != null)
                {
                    //populate the UI
                    UIset(currentStudent);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //verification
            if (currentStudent == null)
            {
                alert.Show("Required info \n Search for Student", alert.AlertType.warnig);
                return;
            }

            if (disAction == 0 & disLevel == 0)
            {
                alert.Show("Required info \n Select provided options", alert.AlertType.warnig);
                return;
            }


            //save the current student info
            using (var context = new SmartdbEntities())
            {
                Disciplinary dd = new Disciplinary();
                dd.Regno = currentStudent.Regno;
                dd.Action = dropAction.selectedValue.ToString();
                dd.Details = tbLegalInfo.Text;
                dd.level = disAction;

                try
                {
                    context.Disciplinaries.Add(dd);
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void dropLevels_onItemSelected(object sender, EventArgs e)
        {
            //ensures diffrent  value inputs
            Random r = new Random();
            //extreme   
            if (dropLevels.selectedIndex == 0)
            {
                disLevel = r.Next(80, 100);

                //populate the dropAction
                String[] s = {"Expulsion" , "Detention" };
                dropAction.Items = s;
            }
            //minior
            if (dropLevels.selectedIndex == 1)
            {
                disLevel = r.Next(0, 49);

                //populate the dropAction
                String[] s = { "Counselling", "Restorative Justice" };
                dropAction.Items = s;
            }
            //major
            if (dropLevels.selectedIndex == 2)
            {
                disLevel = r.Next(50, 79);

                //populate the dropAction
                String[] s = { "Suspension", "Punishment" };
                dropAction.Items = s;
            }
        }


        private void dropAction_onItemSelected(object sender, EventArgs e)
        {
            int indexed = dropAction.selectedIndex;

            switch (indexed)
            {
                case 0:
                    {
                        //selection formula
                        disAction = (100 - disLevel) * (2 / 3);
                        break;
                    }
                case 1:
                    {
                        //selection formula
                        disAction = (100 - disLevel) * (1 / 3);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
