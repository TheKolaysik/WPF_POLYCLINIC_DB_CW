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
using WPF_POLYCLINIC_DB_CourseWork.Models;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.Pacient.UI
{
    public partial class WriteToDoctorFormcs : Form
    {
        PacientReport pacientReport = new PacientReport();
        PacientService pacientService = new PacientService();
        private long id, pacientId;
        private Doctor doctor = null;
        public WriteToDoctorFormcs(long pacientId)
        {
            InitializeComponent();
            this.pacientId = pacientId;
            pacientReport.DisplayFreeDoctor(dataGridView: dataGridView1);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
         // Записаться
        private void button1_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value.Date;


            // Получаем строку времени из ComboBox
            string timeString = comboBox1.SelectedItem?.ToString(); 

            if (!string.IsNullOrEmpty(timeString))
            {
                
                if (TimeSpan.TryParse(timeString, out TimeSpan selectedTime))
                {
                    
                    DateTime fullDateTime = selectedDate + selectedTime;
                    pacientService.AddAppointment(pacientId, id, fullDateTime, textBox1.Text);
                    MessageBox.Show($"Итоговая дата и время: {fullDateTime}");
                }
                else
                {
                    MessageBox.Show("Некорректный формат времени в ComboBox.");
                }
            }
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        
        // Выбрать
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                // Получаем доступ к строке
                DataGridViewRow row = dataGridView1.CurrentRow;
                id = Convert.ToInt64(row.Cells["ID"].Value);
                //string doctorName = row.Cells["DoctorName"].Value?.ToString();


                comboBox1.DataSource = pacientService.GetFreeSlots(id, dateTimePicker1.Value);
                
            }
        }

        // Отменить
        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
