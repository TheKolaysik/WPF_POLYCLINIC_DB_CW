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
using System.Windows.Forms;
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
        private Action<Patient> _onPatientChanged;
        private Action<HistoryRecord> _onHistoryRecordChanged;
        Patient patient = null;
        HistoryRecord historyRecord = null;
        long id;
        public EmployeeMyPacHistory(EmployeeReport employeeReport, EmployeeService employeeService, Patient currentPatient, HistoryRecord history, Action<Patient> onPatientChanged, Action<HistoryRecord> onHistoryRecordChanged)
        {
            InitializeComponent();
            _employeeReport = employeeReport;
            _employeeService = employeeService;
            this.patient = currentPatient;
            historyRecord = history;
            _onPatientChanged = onPatientChanged; // Сохраняем ссылку на действие
            _onHistoryRecordChanged = onHistoryRecordChanged;
        }
        public void UpdateData(long IdDoctor)
        {
            id = IdDoctor;
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
            _employeeReport.DisplayHistoryPacient(dataGridMyPacHistory, IdDoctor, status);
            if (patient != null)
            {
                button_INPUT.Visibility = Visibility.Hidden;
            }
            else
            {
                button_INPUT.Visibility = Visibility.Visible;
            }
        }

        public void InvokePatientChanged(Patient newPatient)
        {
            this.patient = newPatient;
            _onPatientChanged?.Invoke(newPatient);
        }

        public void InvokeHistoryChanged(HistoryRecord newHistory)
        {
            this.historyRecord = newHistory;
            _onHistoryRecordChanged?.Invoke(newHistory);
        }
        public void UpdateData(long IdDoctor, long IdPatient)
        {
            id = IdDoctor;
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
            _employeeReport.DisplayHistoryPacient(dataGridMyPacHistory, IdDoctor, IdPatient, status);
            if (patient != null)
            {
                button_INPUT.Visibility = Visibility.Hidden;
            } else
            {
                button_INPUT.Visibility = Visibility.Visible;
            }
        }

        // Осуществить приём
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли вообще строка
            if (dataGridMyPacHistory.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dataGridMyPacHistory.SelectedItem;
                long historyId = Convert.ToInt64(row["ID записи"]); 
                DateTime appointmentDate = Convert.ToDateTime(row["Дата"]);
                string patientName = row["Пациент"]?.ToString();

                // Использование данных
                System.Windows.Forms.MessageBox.Show($"Выбрана запись №{historyId} от {appointmentDate:dd.MM.yyyy}. Пациент: {patientName}");
                patient = _employeeReport.AuthenticatePatientById(historyId);
                historyRecord = _employeeReport.AuthenticateHistoryById(historyId);
                _onPatientChanged?.Invoke(patient);
                _onHistoryRecordChanged?.Invoke(historyRecord);
                if (patient != null)
                {
                    UpdateData(id, patient.ID_pacient);
                }
                else
                {
                    UpdateData(id);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Пожалуйста, выберите строку в таблице!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (patient != null)
            {
                UpdateData(id, patient.ID_pacient);
            }
            else
            {
                UpdateData(id);
            }
        }
    }
}
