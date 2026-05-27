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
using WPF_POLYCLINIC_DB_CourseWork.Models;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для EmployeePrescriptoinPage.xaml
    /// </summary>
    public partial class EmployeePrescriptoinPage : Page
    {
        private EmployeeService _employeeService;
        private EmployeeReport _employeeReport;
        public EmployeePrescriptoinPage(EmployeeReport employeeReport, EmployeeService employeeService)
        {
            InitializeComponent();
            _employeeReport = employeeReport;
            _employeeService = employeeService;
        }

        public void UpdateData(long IdDoctor)
        {
            buttonPrescription.Visibility = Visibility.Hidden;
            _employeeReport.GetMyPrescription(IdDoctor, dataGridPrescriptions);
        }
        public void UpdateData(long IdDoctor, long IdPatient)
        {
            buttonPrescription.Visibility=Visibility.Visible;
            _employeeReport.GetMyPrescription(IdDoctor, IdPatient, dataGridPrescriptions);
        }

        private void buttonPrescription_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
