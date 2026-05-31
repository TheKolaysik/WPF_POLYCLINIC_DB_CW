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
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.GeneralForms
{
    public partial class ExportFormEmployee : Form
    {
        EmployeeReport employeeReport = new EmployeeReport();
        DoctorDto data;
        private long idDoctor;
        public ExportFormEmployee(long idDoctor)
        {
            InitializeComponent();
            this.idDoctor = idDoctor;
            data = employeeReport.GetDoctorDataFromSql(idDoctor);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool pInfo = chkPersonalInfo.Checked;
            bool history = chkHistory.Checked;
            bool prescriptions = chkPrescriptions.Checked;
            if (data == null)
            {
                MessageBox.Show("Доктор не найден в базе данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Открываем SaveFileDialog
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Word Documents (*.docx)|*.docx";
                sfd.FileName = $"Конструктор_Карта_{data.Surname}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    employeeReport.ExportDoctorReportToWord(data, sfd.FileName, pInfo, history, prescriptions);
                    MessageBox.Show("Документ Word сформирован на основе выбранных полей!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool pInfo = chkPersonalInfo.Checked;
            bool history = chkHistory.Checked;
            bool prescriptions = chkPrescriptions.Checked;

            if (data == null)
            {
                MessageBox.Show("Доктор не найден в базе данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Worksheets (*.xlsx)|*.xlsx";
                sfd.FileName = $"Конструктор_Отчет_{data.Surname}";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    employeeReport.ExportDoctorReportToExcel(data, sfd.FileName, pInfo, history, prescriptions);
                    MessageBox.Show("Файл Excel успешно сгенерирован по выбранному шаблону!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
