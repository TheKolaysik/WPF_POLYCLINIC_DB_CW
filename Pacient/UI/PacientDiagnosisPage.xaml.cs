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

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для PacientDiagnosisPage.xaml
    /// </summary>
    public partial class PacientDiagnosisPage : Page
    {
        private PacientReport pacientReport;
        private PacientService pacientService;
        public PacientDiagnosisPage(PacientReport pacientReport, PacientService pacientService)
        {
            InitializeComponent();
            this.pacientReport = pacientReport;
            this.pacientService = pacientService;
        }

        public void UpdateData(long id)
        {
            pacientReport.DisplayMyDiagnosis(id, dataGridMyDiagnosis);
        }
     }
}
