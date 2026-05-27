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
    /// Логика взаимодействия для EmployeeMyPacHistory.xaml
    /// </summary>
    public partial class EmployeeMyPacHistory : Page
    {
        private EmployeeService _employeeService;
        private EmployeeReport _employeeReport;
        public EmployeeMyPacHistory(EmployeeReport employeeReport, EmployeeService employeeService, ref Patient patient)
        {
            InitializeComponent();
            _employeeReport = employeeReport;
            _employeeService = employeeService;
        }
        public void UpdateData(long IdDoctor)
        {
            _employeeReport.DisplayHistoryPacient(dataGridMyPacHistory, IdDoctor, null);
        }
        public void UpdateData(long IdDoctor, long IdPatient)
        {
            _employeeReport.DisplayHistoryPacient(dataGridMyPacHistory, IdDoctor, IdPatient, null);
        }
    }
}
