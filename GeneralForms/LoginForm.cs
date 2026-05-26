using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;
using WPF_POLYCLINIC_DB_CourseWork.Employee.UI;
using WPF_POLYCLINIC_DB_CourseWork.Models;

namespace WPF_POLYCLINIC_DB_CourseWork
{
    public partial class LoginForm : Form
    {
        static MainPacientWindow mainPacientWindow = new MainPacientWindow();
        static MainEmployeeWindow mainEmployeeWindow = new MainEmployeeWindow();
        PacientService pacientService = new PacientService();
        EmployeeService employeeService = new EmployeeService();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnRegistration_Click(object sender, EventArgs e)
        {
            var regForm = new RegistrationForm();
            regForm.ShowDialog();
        }

        private void btnAuthPacient_Click(object sender, EventArgs e)
        {
            
            try
            {
                Patient currentPatient = pacientService.AuthenticatePatient(login.Text.Trim(), password.Text.Trim());
                this.Hide();
                mainPacientWindow.AuthPacient(currentPatient);
                mainPacientWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            


        }

        private void btnAuthEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                Doctor currentDoctor = employeeService.AuthenticateEmployee(login.Text.Trim(), password.Text.Trim());
                this.Hide();
                mainEmployeeWindow.AuthPacient(currentDoctor);
                mainPacientWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
