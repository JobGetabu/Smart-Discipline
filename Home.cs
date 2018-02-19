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
        public Home()
        {
            InitializeComponent();
        }

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
            Student fst = studs.FirstOrDefault();
            UIset(fst);
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

        private async void UIset(Student fst)
        {
            lblName.Text = fst.FullName;
            lblSchoolAndNo.Text = $"{fst.School} - {fst.SchoolCode}" ;//EschoolKe Secondary - 20154245
            lblMOL.Text = $"Mode of Learning - {fst.LearningMode}";//Mode of Learning - Bording
            lblRegno.Text = $"Reg NO - {fst.Regno}";//Reg NO - 17052

            //look for data on discpline

            Disciplinary mDis = null;

            List<Disciplinary> studdis = await Task.Factory.StartNew(() =>
            {
                using (var context = new SmartdbEntities())
                {
                    return context.Disciplinaries
                    .Where(x=>x.Regno == fst.Regno)
                    .ToList();
                }
            });

            //latest case
            mDis = studdis.OrderBy(x => x.Id).FirstOrDefault();

            if (mDis != null)
            {
                tbLegalInfo.Text = mDis.Details;
            }

            //charts consume this data


        }

        private async Task<Student> StudFoundAsync(string regno)
        {
            var studentList =  await Task.Factory.StartNew(() =>
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //do search
        }
    }
}
