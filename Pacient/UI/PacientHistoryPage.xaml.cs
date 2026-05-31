using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для PacientHistoryPage.xaml
    /// </summary>
    public partial class PacientHistoryPage : Page
    {
        private PacientReport pacientReport;
        private PacientService pacientService;
        private long id;
        public PacientHistoryPage(PacientReport pacientReport, PacientService pacientService)
        {
            InitializeComponent();
            this.pacientReport = pacientReport;
            this.pacientService = pacientService;
            
        }


        public void UpdateData(long id)
        {
            this.id = id;
            List<string> status = null;
            switch (statusFilter.SelectedIndex)
            {
                case 0:
                    status = new List<string>() { "Завершённый" };
                    break;
                case 1:
                    status = new List<string>() { "Предстоящий", "Плановый" };
                    break;
                case 2:
                    status = new List<string>() { "Завершённый", "Предстоящий", "Плановый" };
                    break;
                default:
                    break;

            }
            
            pacientReport.DisplayHistoryPacient(dataGridMyHistory, id, status);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateData(id);
        }

        // Запись на приём
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var detailForm = new WriteToDoctorFormcs(id);
            detailForm.ShowDialog();
        }
    }
}
