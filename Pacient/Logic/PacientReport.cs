using Dapper;
using System;
using System.Collections;
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
using WPF_POLYCLINIC_DB_CourseWork.Models;

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
            adapter = new SqlDataAdapter(@"SELECT 
                d.ID_doctor AS [ID], 
                d.FirstName AS [Имя], 
                d.SurName AS [Фамилия], 
                spec.Name AS [Специальность], 
                COUNT(p.NumberPolicy) AS [Количество_пациентов], 
                d.Experience AS [Стаж], 
                d.Cabinet AS [Кабинет] 
            FROM [Врач] d
            LEFT JOIN [Специальность] spec ON d.ID_specialization = spec.ID_specialization
            LEFT JOIN [Пациент] p ON p.DistrictDoctor = d.ID_doctor
            WHERE d.Working = '1' AND d.Vacation = '0'
            GROUP BY 
                d.ID_doctor, 
                d.FirstName, 
                d.SurName, 
                spec.Name, 
                d.Experience, 
                d.Cabinet;", connection);
            adapter.Fill(dt);
            dataGridView.DataSource = dt;
            connection.Close();
        }
        public bool GetPacientLoginIsExists(string login)
        {

            string sqlExpression = "SELECT COUNT(ID_pacient) FROM Пациент WHERE Login = @login";
            cmd = new SqlCommand(sqlExpression, connection);
            cmd.Parameters.AddWithValue("@login", login);

            connection.Open();
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;

        }

        public bool GetPacientPolicyIsExists(string password)
        {

            string sqlExpression = "SELECT COUNT(ID_pacient) FROM Пациент WHERE NumberPolicy = @policy";
            cmd = new SqlCommand(sqlExpression, connection);
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
                d.Cabinet AS Cabinet
            FROM [История] h
            INNER JOIN [Врач] d ON h.ID_doctor = d.ID_doctor
            INNER JOIN [Специальность] s ON d.ID_specialization = s.ID_specialization
            WHERE h.ID_pacient = @IdPacient
              AND h.Date >= GETDATE() AND h.Status != 'Завершённый'
            ORDER BY h.Date ASC";

            cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = idPacient;

            connection.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    DateTime appointmentDate = reader["Date"] != DBNull.Value
                            ? Convert.ToDateTime(reader["Date"])
                            : DateTime.MinValue;

                    string doctor = reader["DoctorName"]?.ToString() ?? "Не указан";
                    string specialization = reader["SpecializationName"]?.ToString() ?? "Не указана";
                    string cabinet = reader["Cabinet"]?.ToString() ?? "—";
                    result = $"Ближайший прием: {appointmentDate:dd.MM.yyyy в HH:mm} | {doctor} ({specialization} | Кабинет: {cabinet})";
                }
            }

            connection.Close();

            return result;
        }

        public void LoadPrescriptionDetails(long prescriptionId, System.Windows.Forms.TextBox txtInstruction, DataGridView dataGrid)
        {
            string sqlInstruction = @"
        SELECT TOP 1 
            [IssueDate], 
            [Instruction], 
            [PatientName], 
            [DoctorName]
        FROM [View_PatientPrescriptions] 
        WHERE [PrescriptionID] = @Id";

            string sqlMedicaments = @"
        SELECT 
            [MedicamentName] AS [Название лекарства],
            [MethodOfApplication] AS [Способ применения],
            [SideEffects] AS [Подочные эффекты],
            [Manufacturer] AS [Производитель]
        FROM [View_FullPrescriptionDetails]
        WHERE [PrescriptionID] = @Id";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmdInstruction = new SqlCommand(sqlInstruction, conn))
                    {
                        cmdInstruction.Parameters.Add("@Id", SqlDbType.BigInt).Value = prescriptionId;

                        using (SqlDataReader reader = cmdInstruction.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {                                
                                txtInstruction.Text = reader["Instruction"] != DBNull.Value
                                    ? reader["Instruction"].ToString()
                                    : string.Empty;
                            }
                        } 
                    }


                    using (SqlCommand cmdMedicaments = new SqlCommand(sqlMedicaments, conn))
                    {
                        cmdMedicaments.Parameters.Add("@Id", SqlDbType.BigInt).Value = prescriptionId;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdMedicaments))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt); 
                            dataGrid.DataSource = dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public Doctor GetOneEmployee(long id)
        {

            // Получаем врача по ID
            var doctor = connection.QueryFirstOrDefault<Doctor>(
                @"SELECT d.ID_doctor AS [ID_doctor], 
        d.FirstName AS [FirstName], 
        d.SurName AS [SurName], 
        d.Cabinet AS [Cabinet],
        d.Experience AS [Experience],
        d.Phone AS [Phone],
        d.Working AS [Working],
        d.Vacation AS [Vacation],
        d.Login AS [Login],
        d.Password AS [Password],
        spec.Name AS [SpecializationName]
         FROM [Врач] d LEFT JOIN [Специальность] spec ON d.ID_specialization = spec.ID_specialization WHERE ID_doctor = @ID",
                new { ID = id }
            );

            if (doctor == null) throw new Exception("Сотрудник с таким ID не существует!");
            return doctor;
        }
    }
}
