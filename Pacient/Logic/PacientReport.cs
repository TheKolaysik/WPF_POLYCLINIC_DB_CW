using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic
{
    public class PacientReport
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlDataAdapter adapter;
        private SqlCommand cmd;

        public void DisplayFreeDoctor(DataGridView dataGridView)
        {
            connection.Open();
            DataTable dt = new DataTable();
            adapter = new SqlDataAdapter(@"SELECT d.ID_doctor, d.FirstName, d.SurName, COUNT(p.NumberPolicy) AS Количество_пациентов FROM [Врач] d
                                            LEFT JOIN[Специальность] spec ON d.ID_specialization = spec.ID_specialization
                                            LEFT JOIN[Пациент] p ON p.DistrictDoctor = d.ID_doctor
                                            GROUP BY d.FirstName, d.SurName, d.ID_doctor", connection);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            connection.Close();
        }
        public bool GetPacientLoginIsExists(string login)
        {

            string sqlExpression = "SELECT COUNT(ID_pacient) FROM Пациент WHERE Login = @login";
            cmd = new SqlCommand(sqlExpression, connection);
            // Используем параметры для защиты от SQL-инъекций
            cmd.Parameters.AddWithValue("@login", login);

            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;

        }

        public bool GetPacientPolicyIsExists(string password)
        {

            string sqlExpression = "SELECT COUNT(ID_pacient) FROM Пациент WHERE NumberPolicy = @policy";
            cmd = new SqlCommand(sqlExpression, connection);
            // Используем параметры для защиты от SQL-инъекций
            cmd.Parameters.AddWithValue("@policy", password);

            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;

        }
        public string GetUniqueOmsNumber()
        {
            string newOms;
            bool exists;

            do
            {
                newOms = OmsGenerator.GenerateValidOmsNumber();
                // Вызываем SQL процедуру
                exists = GetPacientPolicyIsExists(newOms);
            } while (exists);

            return newOms;
        }

        public void DisplayHistoryPacient(System.Windows.Controls.DataGrid dataGrid, long idPacient, List<string> selectedStatuses, List<string> selectedTypes)
        {
            string statusesString = string.Join(", ", selectedStatuses.Select(s => $"'{s.Replace("'", "''")}'"));
            string typesString = string.Join(", ", selectedTypes.Select(s => $"'{s.Replace("'", "''")}'"));
            if (selectedStatuses == null || selectedStatuses.Count == 0) selectedStatuses = new List<string> { "" };
            if (selectedTypes == null || selectedTypes.Count == 0) selectedTypes = new List<string> { "" };

            // Запрос к представлению с красивыми алиасами для DataGrid
            string sql = $@"
                    SELECT 
                        [RecordNumber] AS [Номер записи],
                        [RecordDate] AS [Дата],
                        [RecordStatus] AS [Статус],
                        [Symptoms] AS [Симптомы],
                        [RecordType] AS [Тип],
                        [DoctorName] AS [Врач]
                    FROM [View_PatientHistoryRecords]
                    WHERE [PatientID] = @IdPacient
                      AND ([RecordStatus] IN ({statusesString}) OR [RecordStatus] IS NULL)
                      AND ([RecordType] IN ({typesString}) OR [RecordType] IS NULL)
                    ORDER BY [RecordDate] DESC";

            connection.Open();
            DataTable dt = new DataTable();

            cmd = new SqlCommand(sql, connection);
            // Добавляем параметры
            cmd.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = idPacient;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            dataGrid.ItemsSource = dt.DefaultView;
            connection.Close();
        }

        public void DisplayMyDiagnosis(long id, System.Windows.Controls.DataGrid dataGrid)
        {
            connection.Open();
            DataTable dt = new DataTable();

            string sql = @"
                SELECT 
                    [DiagnosisID] AS [ID Диагноза],
                    [DiagnosisName] AS [Название диагноза],
                    [EstablishmentDate] AS [Дата постановки]
                FROM [View_PatientDiagnoses]
                WHERE [PatientID] = @IdPacient
                ORDER BY [EstablishmentDate] DESC";

            cmd = new SqlCommand(sql, connection);
            // Передаем ID выбранного пациента в параметр
            cmd.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = id;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;

            connection.Close();
        }
        public void DisplayMyPrescription(long id, System.Windows.Controls.DataGrid dataGrid)
        {
            connection.Open();
            DataTable dt = new DataTable();

            string sql = @"
                SELECT 
                    [PrescriptionID] AS [ID Рецепта],
                    [IssueDate] AS [Дата выдачи],
                    [Instruction] AS [Инструкция по приему],
                    [DoctorName] AS [Выписавший врач]
                FROM [View_PatientPrescriptions]
                WHERE [PatientID] = @IdPacient
                ORDER BY [IssueDate] DESC";

            cmd = new SqlCommand(sql, connection);
            // Передаем ID выбранного пациента в параметр
            cmd.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = id;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;
            connection.Close();
        }

        public string GetNextAppointmentString(long idPacient)
        {
            string result = "Ближайших приемов не запланировано";

            string sql = @"
            SELECT TOP 1
                h.Date,
                (d.SurName + ' ' + d.FirstName) AS DoctorName,
                s.Name AS SpecializationName,
                d.Cabinet AS Cabinet,
                h.Type
            FROM [История] h
            INNER JOIN [Врач] d ON h.ID_doctor = d.ID_doctor
            INNER JOIN [Специальность] s ON d.ID_specialization = s.ID_specialization
            WHERE h.ID_pacient = @IdPacient
              AND h.Date >= GETDATE() AND h.Status != 'Завершённый'
            ORDER BY h.Date ASC";

            cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = idPacient;

            connection.Open();

            // Используем SqlDataReader для чтения одной строки
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read()) // Если запись найдена
                {
                    DateTime appointmentDate = reader.GetDateTime(0);
                    string doctor = reader.GetString(1);
                    string specialization = reader.GetString(2);
                    string cabinet = reader.GetString(3);
                    string type = reader.GetString(4);
                    result = $"Ближайший прием: {appointmentDate:dd.MM.yyyy в HH:mm} | {doctor} ({specialization} | Кабинет: {cabinet} | {type})";
                }
            }

            connection.Close();

            return result;
        }

        private void LoadPrescriptionDetails(long prescriptionId, System.Windows.Forms.TextBox txtInstruction, DataGridView dataGrid)
        {
            // В первом запросе убрана лишняя запятая перед FROM и добавлен выбор инструкции
            string sqlInstruction = @"
        SELECT TOP 1
            [IssueDate],
            [Instruction],
            [PatientName],
            [DoctorName]
        FROM [View_FullPrescriptionDetails] 
        WHERE [PrescriptionID] = @Id";

            string sqlMedicaments = @"
        SELECT 
            [MedicamentID] AS [ID],
            [MedicamentName] AS [Название лекарства],
            [IssueDate] AS [Дата выдачи],
            [Dosage] AS [Дозировка] -- Поле дозировки из представления
        FROM [View_FullPrescriptionDetails]
        WHERE [PrescriptionID] = @Id";

            // Оборачиваем подключение в using, чтобы оно автоматически закрывалось (connection.Close() больше не нужен)
            using (var conn = new SqlConnection(connection.ConnectionString))
            {
                try
                {
                    // 1. Получаем общую информацию (Инструкцию, имена и т.д.)
                    // Используем dynamic, чтобы быстро прочитать свойства без создания лишних классов-моделей
                    var headerInfo = conn.QueryFirstOrDefault<dynamic>(sqlInstruction, new { Id = prescriptionId });

                    if (headerInfo != null)
                    {
                        txtInstruction.Text = headerInfo.Instruction?.ToString();

                        
                    }

                    // 2. Получаем список лекарств для таблицы
                    // Dapper возвращает IEnumerable, .ToList() превращает его в понятный для WinForms список
                    var medicamentsList = conn.Query(sqlMedicaments, new { Id = prescriptionId }).ToList();

                    // Привязываем результат напрямую к DataGridView
                    dataGrid.DataSource = medicamentsList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных через Dapper: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
