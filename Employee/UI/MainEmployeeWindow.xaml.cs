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
using System.Windows.Shapes;
using WPF_POLYCLINIC_DB_CourseWork.GeneralForms;
using WPF_POLYCLINIC_DB_CourseWork.Models;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для MainEmployeeWindow.xaml
    /// </summary>
    public partial class MainEmployeeWindow : Window
    {
        EmployeeMyPacHistory employeeMyPacHistory = new EmployeeMyPacHistory();
        Doctor currentDoctor = null;
        public MainEmployeeWindow()
        {
            InitializeComponent();
        }

        public void AuthPacient(Doctor doctor)
        {
            currentDoctor = doctor;
            //txtPatientFio.Text = $"Пациент: {currentPacient.FirstName} {currentPacient.SurName}";
            //txtNextAppointment.Text = pacientReport.GetNextAppointmentString(patient.ID_pacient);
            //pacientHistoryPage.UpdateData(patient.ID_pacient);
        }

        private void buttonPrescription_Click(object sender, RoutedEventArgs e)
        {
            framePages.Navigate(employeeMyPacHistory);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }
    }
}
