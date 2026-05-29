using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Models;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.Logic
{
    public class EmployeeReport
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public void GetPatientsTableByDistrictDoctor(long idDoctor, System.Windows.Controls.DataGrid dataGrid)
        {
            DataTable dt = new DataTable();

            string sql = @"
            SELECT 
                p.ID_pacient AS [ID Пациента],
                p.SurName AS [Фамилия],
                p.FirstName AS [Имя],
                p.NumberPolicy AS [Номер полиса],
                p.Phone AS [Телефон]
            FROM [Пациент] p
            WHERE p.DistrictDoctor = @IdDoctor";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = idDoctor;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    try
                    {
                        adapter.Fill(dt);
                        dataGrid.ItemsSource = dt.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка заполнения таблицы: {ex.Message}", "Ошибка",
                            System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void GetMyPrescription(long id, long pacientId, System.Windows.Controls.DataGrid dataGrid)
        {
            DataTable dt = new DataTable();

            string sql = @"
        SELECT 
            [PrescriptionID] AS [ID Рецепта],
            [IssueDate] AS [Дата выдачи],
            [Instruction] AS [Инструкция по приему],
            [DoctorName] AS [Выписавший врач]
        FROM [View_PatientPrescriptions]
        WHERE [DoctorID] = @IdDoctor AND [PatientID] = @IdPatient
        ORDER BY [IssueDate] DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = id;
                    cmd.Parameters.Add("@IdPatient", SqlDbType.BigInt).Value = pacientId;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке рецептов: {ex.Message}", "Ошибка",
                                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    }
                }
            }
            dataGrid.ItemsSource = dt.DefaultView;
        }

        public void GetMyPrescription(long id, System.Windows.Controls.DataGrid dataGrid)
        {
            DataTable dt = new DataTable();

            string sql = @"
        SELECT 
            [PrescriptionID] AS [ID Рецепта],
            [IssueDate] AS [Дата выдачи],
            [Instruction] AS [Инструкция по приему],
            [DoctorName] AS [Выписавший врач]
        FROM [View_PatientPrescriptions]
        WHERE [DoctorID] = @IdDoctor
        ORDER BY [IssueDate] DESC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = id;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке рецептов: {ex.Message}", "Ошибка",
                                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    }
                }
            }
            dataGrid.ItemsSource = dt.DefaultView;
        }

        public void DisplayHistoryPacient(System.Windows.Controls.DataGrid dataGrid, long idDoctor, long idPacient, List<string> selectedStatuses)
        {
            if (selectedStatuses == null || selectedStatuses.Count == 0) selectedStatuses = new List<string> { "" };


            string statusesString = string.Join(", ", selectedStatuses.Select(s => $"'{s.Replace("'", "''")}'"));

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
          AND ([RecordType] = 'Приём' OR [RecordType] IS NULL)
        ORDER BY [RecordDate] DESC";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdPacient", SqlDbType.BigInt).Value = idPacient;
                    command.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = idDoctor;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            connection.Open();
                            adapter.Fill(dt);
                        }
                        finally
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            }
            dataGrid.ItemsSource = dt.DefaultView;
        }

        public void DisplayHistoryPacient(System.Windows.Controls.DataGrid dataGrid, long idDoctor, List<string> selectedStatuses)
        {
            if (selectedStatuses == null || selectedStatuses.Count == 0) selectedStatuses = new List<string> { "" };
            
            string statusesString = string.Join(", ", selectedStatuses.Select(s => $"'{s.Replace("'", "''")}'"));
            
            string sql = $@"
                SELECT 
                    [RecordNumber] AS [ID записи],
                    [PatientName] AS [Пациент],
                    [Gender] AS [Пол],
                    [NumberPolicy] AS [Номер полиса],
                    [RecordDate] AS [Дата],
                    [RecordStatus] AS [Статус],
                    [Symptoms] AS [Симптомы],
                    [DoctorName] AS [Врач]
                FROM [View_PatientHistoryRecords]
                WHERE [DoctorID] = @IdDoctor
                  AND ([RecordStatus] IN ({statusesString}) OR [RecordStatus] IS NULL)
                  AND ([RecordType] = 'Приём' OR [RecordType] IS NULL)
                ORDER BY [RecordDate] DESC";

            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = idDoctor;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            connection.Open();
                            adapter.Fill(dt);
                        }
                        finally
                        {
                            if (connection.State == ConnectionState.Open)
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            }
            dataGrid.ItemsSource = dt.DefaultView;
        }

        public void GetDiagnosesList(DataGridView dataGridView)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM [Диагноз] ORDER BY Name ASC";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGridView.DataSource = dt;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке справочника диагнозов: {ex.Message}");
                        }
                    }
                }
            }
        }
        public void GetMedicamentsList(DataGridView dataGridView)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM [Лекарство] ORDER BY Name ASC";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGridView.DataSource = dt;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке справочника диагнозов: {ex.Message}");
                        }
                    }
                }
            }
        }

        public Patient AuthenticatePatientById(long id)
        {
            string sql = @"
                SELECT p.* FROM [Пациент] p
                INNER JOIN [История] h ON p.ID_pacient = h.ID_pacient
                WHERE h.ID_history = @Id";
            using (var conn = new SqlConnection(connectionString))
            {
                // Получаем пациента по его ID
                var patient = conn.QueryFirstOrDefault<Patient>(
                    sql, new { Id = id });

                if (patient == null)
                    throw new Exception("Пациент с таким ID не существует!");
                return patient;
                
            }
        }

        public HistoryRecord AuthenticateHistoryById(long id)
        {
            string sql = @"
            SELECT 
                h.ID_history,
                h.ID_pacient,
                h.ID_doctor,
                h.Date
            FROM [История] h
            WHERE h.ID_history = @Id
            ORDER BY h.Date DESC";
            using (var conn = new SqlConnection(connectionString))
            {
                // Получаем пациента по его ID
                var history = conn.QueryFirstOrDefault<HistoryRecord>(
                    sql, new { Id = id });

                if (history == null)
                    throw new Exception("Запись с таким ID не существует!");
                return history;

            }
        }
    }
}
