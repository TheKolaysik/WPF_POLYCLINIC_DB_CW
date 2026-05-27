using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.UI;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для PacientPrescrittionPage.xaml
    /// </summary>
    public partial class PacientPrescrittionPage : Page
    {
        private PacientReport pacientReport;
        private PacientService pacientService;
        public PacientPrescrittionPage(PacientReport pacientReport, PacientService pacientService)
        {
            InitializeComponent();
            this.pacientReport = pacientReport;
            this.pacientService = pacientService;
        }

        public void UpdateData(long id)
        {
            pacientReport.DisplayMyPrescription(id, dataGridMyPrescriptions);
        }

        private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Убеждаемся, что кликнули именно по строке, а не по пустому месту
            var row = sender as DataGridRow;
            if (row != null && row.Item is DataRowView rowView)
            {
                // Достаем ID рецепта из кликнутой строки DataTable
                // Имя "ID Рецепта" должно совпадать с тем, что вы указали в SQL-запросе через AS
                long prescriptionId = Convert.ToInt64(rowView["ID Рецепта"]);

                // Создаем и открываем окно Windows Forms, передавая туда ID
                // (Убедитесь, что у вас подключена ссылка на System.Windows.Forms)
                var detailForm = new PrescriptionDetailForm(false ,prescriptionId);
                detailForm.ShowDialog(); // Открывает модально поверх WPF-окна
            }
        }
    }
}
