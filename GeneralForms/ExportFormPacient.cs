using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.GeneralForms
{
    public partial class ExportFormPacient : Form
    {
        PacientReport pacientReport =  new PacientReport();
        PatientDto data;
        private long idPatient;
        public ExportFormPacient(long idPatient)
        {
            InitializeComponent();
            this.idPatient = idPatient;
            data = pacientReport.GetPatientDataFromSql(idPatient);
        }
        
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            
            bool pInfo = chkPersonalInfo.Checked;
            bool history = chkHistory.Checked;
            bool prescriptions = chkPrescriptions.Checked;

            if (data == null)
            {
                MessageBox.Show("Пациент не найден в базе данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Worksheets (*.xlsx)|*.xlsx";
                sfd.FileName = $"Конструктор_Отчет_{data.Surname}";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pacientReport.ExportToExcelFromSql(data, sfd.FileName, pInfo, history, prescriptions);
                    MessageBox.Show("Файл Excel успешно сгенерирован по выбранному шаблону!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnExportWord_Click(object sender, EventArgs e)
        {
            bool pInfo = chkPersonalInfo.Checked;
            bool history = chkHistory.Checked;
            bool prescriptions = chkPrescriptions.Checked;
            if (data == null)
            {
                MessageBox.Show("Пациент не найден в базе данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Открываем SaveFileDialog
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Word Documents (*.docx)|*.docx";
                sfd.FileName = $"Конструктор_Карта_{data.Surname}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pacientReport.ExportToWordFromSql(data, sfd.FileName, pInfo, history, prescriptions);
                    MessageBox.Show("Документ Word сформирован на основе выбранных полей!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void chkPrescriptions_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkHistory_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkPersonalInfo_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
