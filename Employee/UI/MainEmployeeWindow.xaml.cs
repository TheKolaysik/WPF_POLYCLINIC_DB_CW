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
    /// Логика взаимодействия для MainEmployeeWindow.xaml
    /// </summary>
    public partial class MainEmployeeWindow : Window
    {
        static Patient currentPacient = null;
        static HistoryRecord historyRecord = null;
        static EmployeeReport employeeReport = new EmployeeReport();
        static EmployeeService employeeService = new EmployeeService();
        static EmployeeMyPacHistory employeeMyPacHistory = new EmployeeMyPacHistory(
            employeeReport,
            employeeService,
            currentPacient, historyRecord,
            (newPatient) =>
            {
                currentPacient = newPatient;

                // Находим активное окно приложения, чтобы получить доступ к элементам UI
                var mainWindow = Application.Current.Windows.OfType<MainEmployeeWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    if (currentPacient != null)
                    {
                        // Выводим ФИО выбранного пациента в StatusBar
                        mainWindow.txtPacientfio.Text = $"Пациент: {currentPacient.FirstName} {currentPacient.SurName}";
                    }
                    else
                    {
                        mainWindow.txtPacientfio.Text = "Пациент не выбран";
                    }
                }
            }, (newHistory) => { historyRecord = newHistory; } 
        );
        static EmployeeMyPac employeeMyPac = new EmployeeMyPac(employeeReport, employeeService);
        static EmployeePrescriptoinPage employeePrescriptoinPage = new EmployeePrescriptoinPage(employeeReport, employeeService);
        
        Doctor currentDoctor = null;
        
        public MainEmployeeWindow()
        {
            InitializeComponent();
            txtPacientfio.Text = "Пациент не выбран";
        }

        public void AuthPacient(Doctor doctor)
        {
            currentDoctor = doctor;
            txtDoctorfio.Text = $"Врач: {currentDoctor.FirstName} {currentDoctor.SurName}";
            txtDoctorSpec.Text = $"Специальность: {currentDoctor.SpecializationName}";
            //txtNextAppointment.Text = pacientReport.GetNextAppointmentString(patient.ID_pacient);
            //pacientHistoryPage.UpdateData(patient.ID_pacient);
        }

        private void buttonPrescription_Click(object sender, RoutedEventArgs e)
        {
            
            framePages.Navigate(employeeMyPacHistory);
            if (currentPacient != null)
            {
                employeeMyPacHistory.UpdateData(currentDoctor.ID_doctor, currentPacient.ID_pacient);
            }
            else
            {
                employeeMyPacHistory.UpdateData(currentDoctor.ID_doctor);                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show();
        }

        private void buttonDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            framePages.Navigate(employeePrescriptoinPage);
            if (currentPacient != null)
            {
                employeePrescriptoinPage.UpdateData(currentDoctor.ID_doctor, currentPacient.ID_pacient, historyRecord.ID_history);
            }
            else
            {
                employeePrescriptoinPage.UpdateData(currentDoctor.ID_doctor);
            }
        }

        private void buttonMyPacients_Click(object sender, RoutedEventArgs e)
        {
            employeeMyPac.UpdateData(currentDoctor.ID_doctor);            
            framePages.Navigate(employeeMyPac);
        }
    }
}
