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

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для EmployeeMyPac.xaml
    /// </summary>
    public partial class EmployeeMyPac : Page
    {
        private EmployeeService _employeeService;
        private EmployeeReport _employeeReport;
        public EmployeeMyPac(EmployeeReport employeeReport, EmployeeService employeeService)
        {
            InitializeComponent();
            _employeeReport = employeeReport;
            _employeeService = employeeService;
        }

        public void UpdateData(long IdDoctor)
        {
            _employeeReport.GetPatientsTableByDistrictDoctor(IdDoctor, dataGridMyPacients);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
