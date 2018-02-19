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
    public partial class FrmLogin1 : Form
    {
        public FrmLogin1()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //Authenticate user
            //perform login

            try
            {
                using (BunifuAnimatorNS.BunifuTransition ss = new BunifuAnimatorNS.BunifuTransition())
                {
                    ss.AnimationType = BunifuAnimatorNS.AnimationType.Rotate;
                    //this.bunifuTransitionUIs
                    this.Visible = false;
                    ss.ShowSync(this);
                    this.Visible = false;
                }
            }
            catch (InvalidOperationException)
            {

                //this exception occurs when the transition is not complete ;(
            }
            catch (System.Reflection.TargetInvocationException exp)
            {
                MessageBox.Show(exp.Message + " \n" + exp.InnerException.Message);
            }
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message, "Try again");
            }

            //Home animation 
            Home hh = new Home();
            try
            {
                using (BunifuAnimatorNS.BunifuTransition ss = new BunifuAnimatorNS.BunifuTransition())
                {
                    ss.AnimationType = BunifuAnimatorNS.AnimationType.ScaleAndRotate;
                    //this.bunifuTransitionUIs
                    ss.ShowSync(hh);
                }
            }
            catch (InvalidOperationException)
            {

                //this exception occurs when the transition is not complete ;(
            }
            catch (System.Reflection.TargetInvocationException exp)
            {
                MessageBox.Show(exp.Message + " \n" + exp.InnerException.Message);
            }
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message, "Try again");
            }
        }
    }
}
