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
    public partial class CreatePrescription : Form
    {
        private EmployeeService _employeeService = new EmployeeService();
        private EmployeeReport _employeeReport = new EmployeeReport();
        private long idPacient, idDoctor, idHistory;
        public CreatePrescription(long idPacient, long idDoctor, long idHistory)
        {
            InitializeComponent();
            _employeeReport.GetDiagnosesList(dataGridView: dataGridView2);
            _employeeReport.GetMedicamentsList(dataGridView: dataGridView1);
            this.idDoctor = idDoctor;
            this.idPacient = idPacient;
            this.idHistory = idHistory;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        // Создать рецепт
        private void button1_Click_1(object sender, EventArgs e)
        {
            _employeeService.CreatePrescription(idHistory, idDoctor, DateTime.Now, textBox1.Text);
        }

        // Добавить лекарство
        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        // Завершить приём
        private void button3_Click(object sender, EventArgs e)
        {

        }

        // Добавить диагноз
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void CreatePrescription_Load(object sender, EventArgs e)
        {

        }
    }
}
