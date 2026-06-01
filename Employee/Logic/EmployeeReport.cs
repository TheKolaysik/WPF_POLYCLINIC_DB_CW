using Dapper;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using WPF_POLYCLINIC_DB_CourseWork.Models;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;
using DataTable = System.Data.DataTable;

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
            [DoctorName] AS [Врач]
        FROM [View_PatientHistoryRecords]
        WHERE [PatientID] = @IdPacient
          AND ([RecordStatus] IN ({statusesString}) OR [RecordStatus] IS NULL)
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
            string sql = "SELECT " +
                "ID_diagnos AS [ID]," +
                "Name AS [Название]," +
                "Description AS [Описание] FROM [Диагноз] ORDER BY Name ASC";

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
            string sql = "SELECT " +
                "ID_medicament AS [ID]," +
                "Name AS [Название]," +
                "MethodOfApplication AS [Способ применения]," +
                "SideEffects AS [Побочные эффекты]," +
                "Manufacturer AS [Производитель]," +
                "Price AS [Цена] FROM [Лекарство] ORDER BY Name ASC";

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
                            System.Windows.MessageBox.Show($"Ошибка при загрузке справочника лекарств: {ex.Message}");
                        }
                    }
                }
            }
        }
        public void GetAllEmployeesList(DataGridView dataGrid)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                d.ID_doctor AS [ID], 
                d.FirstName AS [Имя], 
                d.SurName AS [Фамилия], 
                spec.Name AS [Специальность], 
                COUNT(p.NumberPolicy) AS [Количество_пациентов], 
                d.Experience AS [Стаж], 
                d.Cabinet AS [Кабинет],
                d.ID_specialization,
                d.Working,
                d.Vacation,
                d.Login
            FROM [Врач] d
            LEFT JOIN [Специальность] spec ON d.ID_specialization = spec.ID_specialization
            LEFT JOIN [Пациент] p ON p.DistrictDoctor = d.ID_doctor
            GROUP BY 
                d.ID_doctor, 
                d.FirstName, 
                d.SurName, 
                spec.Name, 
                d.Experience, 
                d.Cabinet,
                d.ID_specialization,
                d.Working,
                d.Vacation,
                d.Login";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGrid.DataSource = dt;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке списка сотрудников: {ex.Message}");
                        }
                    }
                }
            }
        }
        public void GetAllSpecList(DataGridView dataGrid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM [Специальность] ORDER BY Name ASC";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGrid.DataSource = dt;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке справочника диагнозов: {ex.Message}");
                        }
                    }
                }
            }
        }
        public void GetAllHistoryesList(DataGridView dataGrid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM [View_PatientHistoryRecords]";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGrid.DataSource = dt;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show($"Ошибка при загрузке справочника диагнозов: {ex.Message}");
                        }
                    }
                }
            }
        }
        public void GetAllPacientsList(DataGridView dataGrid)
        {
            DataTable dt = new DataTable();
            string sql = @"
            SELECT 
                p.ID_pacient AS [ID Пациента],
                p.SurName AS [Фамилия],
                p.FirstName AS [Имя],
                p.Gender AS [Пол],
                p.DateOfBirth AS [Дата рождения],
                p.PlaceOfLiving AS [Место проживания],
                p.NumberPolicy AS [Номер полиса],                
                p.Phone AS [Телефон],
                p.DistrictDoctor,
                (d.SurName + ' ' + d.FirstName) AS [DoctorName]
            FROM [Пациент] p 
            LEFT JOIN [Врач] d ON p.DistrictDoctor = d.ID_doctor";

            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        try
                        {
                            adapter.Fill(dt);
                            dataGrid.DataSource = dt;
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
        public bool IsDiagnosisInHistory(long historyId, long diagnosisId)
        {
            
            string sql = @"
                SELECT CASE 
                    WHEN EXISTS (
                        SELECT 1 
                        FROM [Диагноз_История] 
                        WHERE [ID_history] = @HistoryId AND [ID_diagnos] = @DiagnosisId
                    ) THEN 1 
                    ELSE 0 
                END";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                    cmd.Parameters.Add("@DiagnosisId", SqlDbType.BigInt).Value = diagnosisId;

                    try
                    {
                        conn.Open();
                        int result = Convert.ToInt32(cmd.ExecuteScalar());
                        return result == 1;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка при проверке наличия диагноза: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return false;
                    }
                }
            } 
        }

        public DoctorDto GetDoctorDataFromSql(long doctorId)
        {
            DoctorDto doctor = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                
                string doctorSql = @"
                    SELECT ID_doctor, Surname, FirstName, Cabinet 
                    FROM [Врач] 
                    WHERE ID_doctor = @DoctorId";

                using (SqlCommand cmd = new SqlCommand(doctorSql, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            doctor = new DoctorDto
                            {
                                Id = Convert.ToInt32(reader["ID_doctor"]),
                                Surname = reader["Surname"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                Cabinet = reader["Cabinet"]?.ToString()
                            };
                        }
                    }
                }

                if (doctor == null) return null;

                // Все приёмы данного врача с данными пациентов 
                string historySql = @"
                    SELECT 
                        h.ID_history, 
                        h.Date, 
                        h.Symptoms, 
                        h.Status,
                        p.ID_pacient,
                        (p.Surname + ' ' + p.FirstName) AS PatientName,
                        p.NumberPolicy
                    FROM [История] h 
                    INNER JOIN [Пациент] p ON h.ID_pacient = p.ID_pacient 
                    WHERE h.ID_doctor = @DoctorId
                    ORDER BY h.Date DESC";

                using (SqlCommand cmd = new SqlCommand(historySql, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctor.Histories.Add(new HistoryDto
                            {
                                Id = Convert.ToInt32(reader["ID_history"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Symptoms = reader["Symptoms"].ToString(),
                                Status = reader["Status"].ToString(),
                                PatientId = Convert.ToInt32(reader["ID_pacient"]),
                                PatientName = reader["PatientName"].ToString(),
                                PatientPolicy = reader["NumberPolicy"]?.ToString()
                            });
                        }
                    }
                }

                // Все диагнозы, поставленные этим врачом на его приёмах
                string diagnosesSql = @"
                    SELECT 
                        dh.ID_history,
                        d.ID_diagnos,
                        d.Name,
                        d.Description
                    FROM [Диагноз_История] dh
                    INNER JOIN [Диагноз] d ON dh.ID_diagnos = d.ID_diagnos
                    INNER JOIN [История] h ON dh.ID_history = h.ID_history
                    WHERE h.ID_doctor = @DoctorId";

                using (SqlCommand cmd = new SqlCommand(diagnosesSql, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int historyId = Convert.ToInt32(reader["ID_history"]);
                            var history = doctor.Histories.Find(h => h.Id == historyId);
                            if (history != null)
                            {
                                history.Diagnoses.Add(new DiagnosisDto
                                {
                                    Id = Convert.ToInt32(reader["ID_diagnos"]),
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"]?.ToString()
                                });
                            }
                        }
                    }
                }

                // Все рецепты и лекарства, выписанные врачом
                string prescriptionsSql = @"
                    SELECT 
                        hp.ID_history, 
                        p.ID_prescription, 
                        p.IssueDate, 
                        p.Instruction,
                        m.Name AS MedName, 
                        m.Manufacturer, 
                        m.Price
                    FROM [История] h
                    INNER JOIN [История_Рецепт] hp ON h.ID_history = hp.ID_history
                    INNER JOIN [Рецепт] p ON hp.ID_prescription = p.ID_prescription
                    INNER JOIN [Лекарство_Рецепт] mp ON p.ID_prescription = mp.ID_prescription
                    INNER JOIN [Лекарство] m ON mp.ID_medicament = m.ID_medicament
                    WHERE h.ID_doctor = @DoctorId";

                using (SqlCommand cmd = new SqlCommand(prescriptionsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int historyId = Convert.ToInt32(reader["ID_history"]);
                            int prescriptionId = Convert.ToInt32(reader["ID_prescription"]);

                            var history = doctor.Histories.Find(h => h.Id == historyId);
                            if (history != null)
                            {
                                var prescription = history.Prescriptions.Find(p => p.Id == prescriptionId);
                                if (prescription == null)
                                {
                                    prescription = new PrescriptionDto
                                    {
                                        Id = prescriptionId,
                                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                        Instruction = reader["Instruction"]?.ToString()
                                    };
                                    history.Prescriptions.Add(prescription);
                                }

                                prescription.Medicaments.Add(new MedicamentDto
                                {
                                    Name = reader["MedName"].ToString(),
                                    Manufacturer = reader["Manufacturer"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"])
                                });
                            }
                        }
                    }
                }
            }
            return doctor;
        }
        public void ExportDoctorReportToWord(DoctorDto doctor, string filePath, bool includePersonalInfo, bool includeHistory, bool includePrescriptions)
        {
            if (doctor == null) return;

            Word.Application wordApp = null;
            Word.Document document = null;

            try
            {
                wordApp = new Word.Application();
                document = wordApp.Documents.Add();
                if (includePersonalInfo)
                {
                    Word.Paragraph infoPara = document.Paragraphs.Add();
                    infoPara.Range.Text = $"Доктор: {doctor.Surname} {doctor.FirstName}\n" +
                                          $"Кабинет: {doctor.Cabinet}\n";
                    infoPara.Range.Font.Size = 11;
                    infoPara.Range.Font.Bold = 0;
                    infoPara.Range.InsertParagraphAfter();
                }

                if (includeHistory)
                {
                    Word.Paragraph historyHeader = document.Paragraphs.Add();
                    historyHeader.Range.Text = "Реестр оказанной медицинской помощи:";
                    historyHeader.Range.Font.Size = 13;
                    historyHeader.Range.Font.Bold = 1;
                    historyHeader.Range.InsertParagraphAfter();

                    int index = 1;
                    foreach (var h in doctor.Histories)
                    {
                        Word.Paragraph hPara = document.Paragraphs.Add();
                        hPara.Range.Text = $"{index}. Приём от {h.Date.ToShortDateString()} — Пациент: {h.PatientName} (ОМС: {h.PatientPolicy})\n" +
                                           $"   Жалобы: {h.Symptoms} | Статус: {h.Status}";
                        hPara.Range.Font.Size = 11;
                        hPara.Range.Font.Bold = 0;
                        hPara.Range.InsertParagraphAfter();

                        if (h.Diagnoses.Any())
                        {
                            Word.Paragraph dPara = document.Paragraphs.Add();
                            dPara.Range.Text = $"      Поставленные диагнозы:\n" +
                                               string.Join("\n", h.Diagnoses.Select(d => $"         • {d.Name} ({d.Description})"));
                            dPara.Range.Font.Size = 11;
                            dPara.Range.Font.Bold = 1;
                            dPara.Range.InsertParagraphAfter();
                        }

                        if (includePrescriptions && h.Prescriptions.Any())
                        {
                            foreach (var pr in h.Prescriptions)
                            {
                                Word.Paragraph rPara = document.Paragraphs.Add();
                                rPara.Range.Text = $"      => Выписан Рецепт №{pr.Id} (Описание: {pr.Instruction}):\n" +
                                                   string.Join("\n", pr.Medicaments.Select(m => $"         - {m.Name} [{m.Manufacturer}]"));
                                rPara.Range.Font.Size = 11;
                                rPara.Range.Font.Italic = 1;
                                rPara.Range.InsertParagraphAfter();
                            }
                        }
                        index++;
                    }
                }

                object filename = filePath;
                document.SaveAs2(ref filename);
            }
            finally
            {
                if (document != null) { document.Close(false); Marshal.ReleaseComObject(document); }
                if (wordApp != null) { wordApp.Quit(); Marshal.ReleaseComObject(wordApp); }
                GC.Collect();
            }
        }
        public void ExportDoctorReportToExcel(DoctorDto doctor, string filePath, bool includePersonalInfo, bool includeHistory, bool includePrescriptions)
        {
            if (doctor == null) return;

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbook = excelApp.Workbooks.Add(Type.Missing);
                worksheet = (Excel.Worksheet)workbook.Sheets[1];
                worksheet.Name = "Отчет доктора";

                int currentRow = 1;

                worksheet.Cells[currentRow, 1] = "СВОДНЫЙ ОТЧЕТ О РАБОТЕ ВРАЧА";
                ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                currentRow += 2;

                if (includePersonalInfo)
                {
                    worksheet.Cells[currentRow, 1] = "Врач:";
                    worksheet.Cells[currentRow, 2] = $"{doctor.Surname} {doctor.FirstName}";
                    currentRow++;
                    worksheet.Cells[currentRow, 1] = "Кабинет:";
                    worksheet.Cells[currentRow, 2] = doctor.Cabinet;
                    currentRow += 2;
                }

                if (includeHistory)
                {
                    worksheet.Cells[currentRow, 1] = "Журнал приёмов пациентов";
                    ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                    currentRow++;

                    List<string> headers = new List<string> { "Дата приёма", "Пациент", "Полис ОМС", "Симптомы", "Статус", "Поставленные диагнозы" };
                    if (includePrescriptions) headers.Add("Выписанные рецепты");

                    for (int i = 0; i < headers.Count; i++)
                    {
                        Excel.Range cell = (Excel.Range)worksheet.Cells[currentRow, i + 1];
                        cell.Value2 = headers[i];
                        cell.Font.Bold = true;
                        cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                    }
                    currentRow++;

                    foreach (var h in doctor.Histories)
                    {
                        worksheet.Cells[currentRow, 1] = h.Date.ToShortDateString();
                        worksheet.Cells[currentRow, 2] = h.PatientName;
                        worksheet.Cells[currentRow, 3] = h.PatientPolicy;
                        worksheet.Cells[currentRow, 4] = h.Symptoms;
                        worksheet.Cells[currentRow, 5] = h.Status;

                        string diagnosesText = string.Join("\n", h.Diagnoses.Select(d => $"• {d.Name}"));
                        Excel.Range diagCell = (Excel.Range)worksheet.Cells[currentRow, 6];
                        diagCell.Value2 = string.IsNullOrEmpty(diagnosesText) ? "Нет" : diagnosesText;
                        diagCell.WrapText = true;

                        if (includePrescriptions)
                        {
                            List<string> prescriptionsText = new List<string>();
                            foreach (var pr in h.Prescriptions)
                            {
                                string info = $"Рецепт №{pr.Id}:\n" + string.Join("\n", pr.Medicaments.Select(m => $"- {m.Name}"));
                                prescriptionsText.Add(info);
                            }
                            Excel.Range targetCell = (Excel.Range)worksheet.Cells[currentRow, 7];
                            targetCell.Value2 = string.Join("\n\n", prescriptionsText);
                            targetCell.WrapText = true;
                        }
                        currentRow++;
                    }
                }

                Excel.Range usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                ((Excel.Range)worksheet.Columns[6]).ColumnWidth = 30;
                if (includePrescriptions) ((Excel.Range)worksheet.Columns[7]).ColumnWidth = 40;

                workbook.SaveAs(filePath);
            }
            finally
            {
                if (workbook != null) { workbook.Close(false); Marshal.ReleaseComObject(workbook); }
                if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }
                GC.Collect();
            }
        }
    }
}
