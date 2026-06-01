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
        private long id_prescription, id_medicament, id_diagnos; 
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
            id_prescription = _employeeService.CreatePrescription(idHistory, idDoctor, DateTime.Now, textBox1.Text);
            button1.Hide();
            button2.Show();
        }

        private void dataGrid_RowHeaderMouseClick(object srnder, DataGridViewCellMouseEventArgs e)
        {
            id_medicament = Convert.ToInt64(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
        }

        private void dataGrid1_RowHeaderMouseClick(object srnder, DataGridViewCellMouseEventArgs e)
        {
            id_diagnos = Convert.ToInt64(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
        }
        // Добавить лекарство
        private void button2_Click_1(object sender, EventArgs e)
        {
            _employeeService.AddMedicamentToPrescription(id_prescription, id_medicament);
        }

        // Завершить приём
        private void button3_Click(object sender, EventArgs e)
        {
            _employeeService.CompleteAppointment(idHistory);
            WPF_POLYCLINIC_DB_CourseWork.Employee.UI.MainEmployeeWindow.ResetCurrentSession();

            // Выводим уведомление и закрываем текущую форму рецепта
            MessageBox.Show("Приём успешно завершён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        // Добавить диагноз
        private void button4_Click(object sender, EventArgs e)
        {
            if (!_employeeReport.IsDiagnosisInHistory(idHistory, id_diagnos))
            {
                _employeeService.AddDiagnosisToHistory(idHistory, id_diagnos);
            }
            else
            {
                MessageBox.Show("Диагноз уже был добавлен!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CreatePrescription_Load(object sender, EventArgs e)
        {

        }
    }
}
