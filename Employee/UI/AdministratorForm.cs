using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    
    public partial class AdministratorForm : Form
    {
        EmployeeReport employeeReport = new EmployeeReport();
        EmployeeService employeeeService = new EmployeeService();
        private string id_pacient, id_history, id_doctor, id_medicament, id_diagnos, id_spec;
        public AdministratorForm()
        {
            InitializeComponent();
            employeeReport.GetAllEmployeesList(dataGridViewEmployee);
            employeeReport.GetAllPacientsList(dataGridView1);
            employeeReport.GetAllHistoryesList(dataGridHistory);
            employeeReport.GetAllSpecList(dataGridView2);
            employeeReport.GetDiagnosesList(dataGridViewDiagnosis);
            employeeReport.GetMedicamentsList(dataGridView3);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void AdministratorForm_Load(object sender, EventArgs e)
        {

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    employeeReport.GetAllEmployeesList(dataGridViewEmployee);
                    break;
                case 1:
                    employeeReport.GetMedicamentsList(dataGridView3);
                    break;
                case 2:
                    employeeReport.GetAllSpecList(dataGridView2);
                    break;
                case 3:
                    employeeReport.GetDiagnosesList(dataGridViewDiagnosis);
                    break;
                case 4:
                    employeeReport.GetAllHistoryesList(dataGridHistory);
                    break;
                case 5:
                    employeeReport.GetAllPacientsList(dataGridView1);
                    break;
                default: 
                    break;
            } 
        }
    }
}
