using custom_alert_notifications;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace SmartDiscipline
{
    public partial class Home : Form
    {

        private decimal disLevel;
        private decimal disAction;

        private double angValue = 0;
        private ChartValues<decimal> mPieValue;
        private ChartValues<decimal> pieValueM;
        private ChartValues<decimal> pieValueE;

        public Home()
        {
            InitializeComponent();
            SetUpAngular();
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
            //chart updates auto

            UIset(currentStudent);


        }

        private void UpdateCharts(List<Disciplinary> lDis)
        {
            //different values of discipline
            //minor major extreme
            //check drop values

            decimal minor = 0; decimal major = 0; decimal extreme = 0;

            foreach (var dsss in lDis)
            {
                if (dsss.level > 79)
                {
                    // dropLevels.selectedIndex = 0;
                    extreme += dsss.level;
                }
                if (dsss.level > 50 & dsss.level < 80)
                {
                    //dropLevels.selectedIndex = 2;
                    major += dsss.level;
                }
                if (dsss.level >= 0 & dsss.level < 50)
                {
                    //dropLevels.selectedIndex = 1;
                    minor += dsss.level;
                }
            }

            // pieValueE.Clear() ;pieValueM.Clear(); mPieValue.Clear();

            // pieValueE.Add(extreme);pieValueM.Add(major);mPieValue.Add(minor);

            SetUpPieChart(minor, major, extreme);

            decimal average = ((extreme + minor + major) / 100) * 100;

            // angValue.Clear();
            angValue = (double)average;
            angularGauge1.Value = angValue;

        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            SetToolTip(tbLegalInfo, $"Info: \n Keywords like Good, bad, improving, indisciplined, positive, questionable \n are being tracked to determine overall discipline");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
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
            if (dsss.level > 50 & dsss.level < 80)
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
            mDis = studdis.OrderBy(x => x.Id).LastOrDefault();

            if (mDis != null)
            {
                tbLegalInfo.Text = mDis.Details;
                SetUpDrops(mDis);
            }
            UpdateCharts(studdis);

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
                    alert.Show($"{currentStudent.FullName} found ", alert.AlertType.success);
                }
                else
                {
                    alert.Show("No student found", alert.AlertType.info);
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
                dd.level = (disAction + disLevel);

                try
                {
                    context.Disciplinaries.Add(dd);
                    context.SaveChanges();
                    alert.Show("Saved Successfully", alert.AlertType.success);

                    //set ui again
                    UIset(currentStudent);
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
                String[] s = { "Expulsion", "Detention" };
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

        private async void tab2_Click(object sender, EventArgs e)
        {
            //prompt add student
            FrmAddStud ff = new FrmAddStud();
            if (ff.ShowDialog() == DialogResult.OK)
            {
                List<Student> studs = await Task.Factory.StartNew(() =>
                {
                    using (var context = new SmartdbEntities())
                    {
                        return context.Students.OrderBy(c => c.Regno)
                        .ToList();
                    }
                });

                //first student in db default
                currentStudent = studs.LastOrDefault();
                //chart updates auto

                UIset(currentStudent);
            }
        }

        //pie chart
        private async void SetUpPieChart(decimal i, decimal j, decimal k)
        {
            pieChart1.InnerRadius = 70;
            pieChart1.LegendLocation = LegendLocation.Right;

            pieChart1.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Minor Issues",
                    Values = new ChartValues<decimal> {i },
                    PushOut = 15,
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Major Issues",
                    Values = new ChartValues<decimal> {j },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Extreme Cases",
                    Values = new ChartValues<decimal> {k },
                    DataLabels = true
                }
            };

        }


        private void SetUpAngular()
        {
            angularGauge1.Value = angValue;
            angularGauge1.FromValue = 0;
            angularGauge1.ToValue = 100;

            angularGauge1.TicksForeground = Brushes.White;
            angularGauge1.Base.Foreground = Brushes.White;
            angularGauge1.Base.FontWeight = FontWeights.Bold;
            angularGauge1.Base.FontSize = 16;
            angularGauge1.SectionsInnerRadius = 0.5;

            angularGauge1.Sections.Add(new AngularSection
            {
                FromValue = 0,
                ToValue = 49,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(123, 141, 57))
            });
            angularGauge1.Sections.Add(new AngularSection
            {
                FromValue = 50,
                ToValue = 79,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(221, 69, 0))
            });
            angularGauge1.Sections.Add(new AngularSection
            {
                FromValue = 80,
                ToValue = 100,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 187, 0))
            });
            angularGauge1.Sections.Add(new AngularSection
            {
                FromValue = 200,
                ToValue = 250,
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(201, 0, 0))
            });
        }
    }
}
