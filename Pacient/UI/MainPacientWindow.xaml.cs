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
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;
using WPF_POLYCLINIC_DB_CourseWork.GeneralForms;
using WPF_POLYCLINIC_DB_CourseWork.Models;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.UI
{
    /// <summary>
    /// Логика взаимодействия для MainPacientWindow.xaml
    /// </summary>
    public partial class MainPacientWindow : Window
    {
        static PacientService pacientService = new PacientService();
        static PacientReport pacientReport = new PacientReport();
        static PacientDiagnosisPage pacientDiagnosisPage = new PacientDiagnosisPage(pacientReport, pacientService);
        static PacientHistoryPage pacientHistoryPage = new PacientHistoryPage(pacientReport, pacientService);
        static PacientPrescrittionPage pacientPrescriptionPage = new PacientPrescrittionPage(pacientReport, pacientService);
        private Patient currentPacient = null;
        public MainPacientWindow()
        {
            InitializeComponent();
            framePages.Navigate(pacientHistoryPage);
        }

        public void AuthPacient(Patient patient)
        {
            currentPacient = patient;
            txtPatientFio.Text = $"Пациент: {currentPacient.FirstName} {currentPacient.SurName}";
            txtNextAppointment.Text = pacientReport.GetNextAppointmentString(patient.ID_pacient);
            pacientHistoryPage.UpdateData(patient.ID_pacient);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void buttonDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            pacientDiagnosisPage.UpdateData(currentPacient.ID_pacient);
            framePages.Navigate(pacientDiagnosisPage);
        }

        private void buttonPrescription_Click(object sender, RoutedEventArgs e)
        {
            pacientPrescriptionPage.UpdateData(currentPacient.ID_pacient);
            framePages.Navigate(pacientPrescriptionPage);
        }

        private void buttonHistory_Click(object sender, RoutedEventArgs e)
        {
            pacientHistoryPage.UpdateData(currentPacient.ID_pacient);
            framePages.Navigate(pacientHistoryPage);
        }

        private void buttonExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var exportForm = new ExportFormPacient(currentPacient.ID_pacient);
            exportForm.ShowDialog();
        }
    }
}
