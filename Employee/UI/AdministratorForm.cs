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
        // Удалить приём
        private void button1_Click(object sender, EventArgs e)
        {
            employeeeService.DeleteHistoryRecord(Convert.ToInt64(id_history));
            employeeReport.GetAllHistoryesList(dataGridHistory);
        }
        // Отправить в отпуск
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateDoctorVacation(Convert.ToInt32(id_doctor), true, false);
                MessageBox.Show("Данные доктора обновлены (отправлен в отпуск!");
                employeeReport.GetAllEmployeesList(dataGridViewEmployee);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        private void pacientData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_pacient = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            if (dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString().Trim().ToLower() == "мужской")
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex = 1;
            }
            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            textBox4.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            textBox5.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
            textBox6.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();


        }
        // Добавить врача
        private void buttonAddEmployee_Click(object sender, EventArgs e)
        {
            employeeeService.SaveNewDoctor(firstNameField.Text, surNameField.Text, cabinetField.Text, experienceField.Text, "88888888888", idSpecField.Text, true, false, loginField.Text, passwordField.Text);
            //MessageBox.Show("Доктор успешно добавлен");
            employeeReport.GetAllEmployeesList(dataGridViewEmployee);
        }
        // Изменить данные врача
        private void buttonEditEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateDoctor(Convert.ToInt32(id_doctor), surNameField.Text, firstNameField.Text, cabinetField.Text);
                MessageBox.Show("Данные доктора обновлены!");
                employeeReport.GetAllEmployeesList(dataGridViewEmployee);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        // Удалить врача
        private void buttonDeleteEmployee_Click(object sender, EventArgs e)
        {
            employeeeService.DeleteDoctorAndReassignPatients(Convert.ToInt64(id_doctor));
            employeeReport.GetAllEmployeesList(dataGridViewEmployee);
        }
        // Вернуть из отпуска
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateDoctorVacation(Convert.ToInt32(id_doctor), false, true);
                MessageBox.Show("Данные доктора обновлены (возвращён из отпуска)!");
                employeeReport.GetAllEmployeesList(dataGridViewEmployee);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Добавить лекарство
        private void buttonAddMedicament_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.AddMedicament(nameMedic.Text, manufacturer.Text, Convert.ToDecimal(price.Text), sideEffects.Text, descriptionMedic.Text);
                MessageBox.Show("Лекарство успешно добавлено");
                employeeReport.GetMedicamentsList(dataGridView3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Изменить данные лекарства
        private void buttonEditMedicament_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateMedicament(Convert.ToInt32(id_medicament), nameMedic.Text, manufacturer.Text, Convert.ToDecimal(price.Text), sideEffects.Text, descriptionMedic.Text);
                MessageBox.Show("Данные лекарства обновлены!");
                employeeReport.GetMedicamentsList(dataGridView3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Добавить специальность
        private void buttonAddSpec_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.AddSpecialization(nameSpec.Text, descriptionSpec.Text);
                MessageBox.Show("Специальность успешно добавлена");
                employeeReport.GetAllSpecList(dataGridView2);
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.Message);
            }
        }
        // Изменить данные специальности
        private void buttonEditSpec_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateSpecialization(Convert.ToInt32(id_spec), nameSpec.Text, descriptionSpec.Text);
                MessageBox.Show("Данные специальности обновлены!");
                employeeReport.GetAllSpecList(dataGridView2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Добавить диагноз
        private void buttonAddDiagnos_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.AddDiagnosis(textBoxDName.Text, textBoxDDescription.Text);
                MessageBox.Show("Диагноз успешно добавлен");
                employeeReport.GetDiagnosesList(dataGridViewDiagnosis);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Изменить данные диагнозы
        private void buttonEditDiagnos_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdateDiagnosis(Convert.ToInt32(id_diagnos), textBoxDName.Text, textBoxDDescription.Text);
                MessageBox.Show("Данные диагноза обновлены!");
                employeeReport.GetDiagnosesList(dataGridViewDiagnosis);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Изменить данные пациента
        private void buttonEditPac_Click(object sender, EventArgs e)
        {
            try
            {
                employeeeService.UpdatePatient(Convert.ToInt32(id_pacient), textBox2.Text, textBox1.Text, textBox4.Text, dateTimePicker1.Value);
                MessageBox.Show("Данные пациента обновлены!");
                employeeReport.GetAllPacientsList(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        // Удалить пациента
        private void buttonDeletePac_Click(object sender, EventArgs e)
        {
            employeeeService.DeletePatientWithHistoryAndPrescriptions(Convert.ToInt64(id_pacient));
            employeeReport.GetAllPacientsList(dataGridView1);
        }

        private void employeeData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_doctor = dataGridViewEmployee.Rows[e.RowIndex].Cells[0].Value.ToString();
            firstNameField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[1].Value.ToString();
            surNameField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[2].Value.ToString();
            cabinetField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[6].Value.ToString();
            experienceField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[5].Value.ToString();
            idSpecField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[7].Value.ToString();
            loginField.Text = dataGridViewEmployee.Rows[e.RowIndex].Cells[10].Value.ToString();
        }
        private void historyData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_history = dataGridHistory.Rows[e.RowIndex].Cells[2].Value.ToString();
            
        }
        private void medicamentData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_medicament = dataGridView3.Rows[e.RowIndex].Cells[0].Value.ToString();
            nameMedic.Text = dataGridView3.Rows[e.RowIndex].Cells[1].Value.ToString();
            descriptionMedic.Text = dataGridView3.Rows[e.RowIndex].Cells[2].Value.ToString();
            sideEffects.Text = dataGridView3.Rows[e.RowIndex].Cells[3].Value.ToString();
            manufacturer.Text = dataGridView3.Rows[e.RowIndex].Cells[4].Value.ToString();
            price.Text = dataGridView3.Rows[e.RowIndex].Cells[5].Value.ToString();

        }
        private void diagnosData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_diagnos = dataGridViewDiagnosis.Rows[e.RowIndex].Cells[0].Value.ToString();
            textBoxDName.Text = dataGridViewDiagnosis.Rows[e.RowIndex].Cells[1].Value.ToString();
            textBoxDDescription.Text = dataGridViewDiagnosis.Rows[e.RowIndex].Cells[2].Value.ToString();
            
        }
        private void specData_RowHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            id_spec = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            nameSpec.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
            descriptionSpec.Text = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
            
        }
    }
}
