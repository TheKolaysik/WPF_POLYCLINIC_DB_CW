using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
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
            WHERE d.Working = '1' AND d.Vacation = '0' AND spec.Name != 'Администратор'
            GROUP BY 
                d.ID_doctor, 
                d.FirstName, 
                d.SurName, 
                spec.Name, 
                d.Experience, 
                d.Cabinet
            HAVING COUNT(p.NumberPolicy) < 50;", connection);
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

        public void DisplayHistoryPacient(System.Windows.Controls.DataGrid dataGrid, long idPacient, List<string> selectedStatuses)
        {
            string statusesString = string.Join(", ", selectedStatuses.Select(s => $"'{s.Replace("'", "''")}'"));
            if (selectedStatuses == null || selectedStatuses.Count == 0) selectedStatuses = new List<string> { "" };


            // Запрос к представлению с красивыми алиасами для DataGrid
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

        public void LoadPrescriptionDetails(long prescriptionId, System.Windows.Forms.TextBox pacient, System.Windows.Forms.TextBox doctor, System.Windows.Forms.TextBox date, System.Windows.Forms.TextBox txtInstruction, DataGridView dataGrid)
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
            [SideEffects] AS [Побочные эффекты],
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
                                pacient.Text = reader["PatientName"] != DBNull.Value
                                    ? reader["PatientName"].ToString()
                                    : string.Empty;
                                doctor.Text = reader["DoctorName"] != DBNull.Value
                                    ? reader["DoctorName"].ToString()
                                    : string.Empty;
                                date.Text = reader["IssueDate"] != DBNull.Value
                                    ? Convert.ToDateTime(reader["IssueDate"]).Date.ToString()
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
        public PatientDto GetPatientDataFromSql(long patientId)
        {
            PatientDto patient = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Запрос на личные данные пациента
                string patientSql = "SELECT ID_pacient, Surname, FirstName, NumberPolicy, DateOfBirth FROM [Пациент] WHERE ID_pacient = @PatientId";
                using (SqlCommand cmd = new SqlCommand(patientSql, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patient = new PatientDto
                            {
                                Id = Convert.ToInt32(reader["ID_pacient"]),
                                Surname = reader["Surname"].ToString(),
                                FirstName = reader["FirstName"].ToString(),
                                NumberPolicy = reader["NumberPolicy"]?.ToString(),
                                DateOfBirth = reader["DateOfBirth"] as DateTime?
                            };
                        }
                    }
                }
                if (patient == null) return null;

                string historySql = @"
            SELECT 
                h.ID_history, 
                h.Date, 
                h.Symptoms, 
                h.Status, 
                (d.Surname + ' ' + d.FirstName) AS DocName 
            FROM [История] h 
            LEFT JOIN [Врач] d ON h.ID_doctor = d.ID_doctor 
            WHERE h.ID_pacient = @PatientId
            ORDER BY h.Date";

                using (SqlCommand cmd = new SqlCommand(historySql, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patient.Histories.Add(new HistoryDto
                            {
                                Id = Convert.ToInt32(reader["ID_history"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Symptoms = reader["Symptoms"].ToString(),
                                Status = reader["Status"].ToString(),
                                // Если врач не указан (LEFT JOIN), запишем пустую строку вместо NULL
                                DoctorName = reader["DocName"] == DBNull.Value ? "Не указан" : reader["DocName"].ToString()
                            });
                        }
                    }
                }
                string prescriptionsSql = @"
                    SELECT 
                        hp.ID_history, 
                        p.ID_prescription, 
                        p.IssueDate, 
                        p.Instruction,
                        m.Name AS MedName, 
                        m.Manufacturer, 
                        m.Price, 
                        m.SideEffects
                    FROM [История] h
                    JOIN [История_Рецепт] hp ON h.ID_history = hp.ID_history
                    JOIN [Рецепт] p ON hp.ID_prescription = p.ID_prescription
                    JOIN [Лекарство_Рецепт] mp ON p.ID_prescription = mp.ID_prescription
                    JOIN [Лекарство] m ON mp.ID_medicament = m.ID_medicament
                    WHERE h.ID_pacient = @PatientId";

                using (SqlCommand cmd = new SqlCommand(prescriptionsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@PatientId", patientId);
                    

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int historyId = Convert.ToInt32(reader["ID_history"]);
                            int prescriptionId = Convert.ToInt32(reader["ID_prescription"]);

                            // Ищем, к какому из уже загруженных приемов привязать этот рецепт
                            var history = patient.Histories.Find(h => h.Id == historyId);
                            if (history != null)
                            {
                                var prescription = history.Prescriptions.Find(p => p.Id == prescriptionId);
                                if (prescription == null)
                                {
                                    prescription = new PrescriptionDto
                                    {
                                        Id = prescriptionId,
                                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                                        Instruction = reader["Instruction"].ToString()
                                    };
                                    history.Prescriptions.Add(prescription);
                                }

                                prescription.Medicaments.Add(new MedicamentDto
                                {
                                    Name = reader["MedName"].ToString(),
                                    Manufacturer = reader["Manufacturer"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    SideEffects = reader["SideEffects"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            return patient;
        }

        public void ExportToExcelFromSql(PatientDto patient, string filePath, bool includePersonalInfo, bool includeHistory, bool includePrescriptions)
        {
            if (patient == null) return;

            Excel.Application excelApp = null;
            Excel.Workbooks workbooks = null;
            Excel.Workbook workbook = null;
            Excel.Sheets sheets = null;
            Excel.Worksheet worksheet = null;

            try
            {
                excelApp = new Excel.Application();
                workbooks = excelApp.Workbooks;
                workbook = workbooks.Add(Type.Missing);
                sheets = workbook.Sheets;
                worksheet = (Excel.Worksheet)sheets[1];
                worksheet.Name = "Отчет SQL";

                int currentRow = 1;

                // Заголовок периода
                worksheet.Cells[currentRow, 1] = "МЕДИЦИНСКИЙ ОТЧЕТ";
                ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                
                currentRow += 2;

                // Личные данные
                if (includePersonalInfo)
                {
                    worksheet.Cells[currentRow, 1] = "Пациент (ФИО):";
                    worksheet.Cells[currentRow, 2] = $"{patient.Surname} {patient.FirstName}";
                    currentRow++;
                    worksheet.Cells[currentRow, 1] = "Полис ОМС:";
                    worksheet.Cells[currentRow, 2] = patient.NumberPolicy;
                    currentRow += 2;
                }

                // Табличная история приёмов
                if (includeHistory)
                {
                    worksheet.Cells[currentRow, 1] = "История приёмов и назначенное лечение";
                    ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                    currentRow++;

                    List<string> headers = new List<string> { "Дата приёма", "Врач", "Симптомы", "Статус" };
                    if (includePrescriptions) headers.Add("Назначенные лекарства (Рецепты)");

                    for (int i = 0; i < headers.Count; i++)
                    {
                        Excel.Range cell = (Excel.Range)worksheet.Cells[currentRow, i + 1];
                        cell.Value2 = headers[i];
                        cell.Font.Bold = true;
                        cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    }
                    currentRow++;

                    foreach (var h in patient.Histories)
                    {
                        worksheet.Cells[currentRow, 1] = h.Date.ToShortDateString();
                        worksheet.Cells[currentRow, 2] = string.IsNullOrEmpty(h.DoctorName) ? "Не указан" : h.DoctorName;
                        worksheet.Cells[currentRow, 3] = h.Symptoms;
                        worksheet.Cells[currentRow, 4] = h.Status;

                        if (includePrescriptions)
                        {
                            List<string> prescriptionsText = new List<string>();
                            foreach (var pr in h.Prescriptions)
                            {
                                string info = $"Рецепт №{pr.Id} ({pr.Instruction}):\n";
                                info += string.Join("\n", pr.Medicaments.Select(m => $"- {m.Name} [{m.Manufacturer}] — {m.Price} руб."));
                                prescriptionsText.Add(info);
                            }
                            Excel.Range targetCell = (Excel.Range)worksheet.Cells[currentRow, 5];
                            targetCell.Value2 = string.Join("\n\n", prescriptionsText);
                            targetCell.WrapText = true;
                        }
                        currentRow++;
                    }
                }
                else if (includePrescriptions)
                {
                    // Сценарий конструктора: историю отключили, но рецепты выгрузить надо
                    worksheet.Cells[currentRow, 1] = "Сводный реестр рецептов за период:";
                    ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                    currentRow += 2;

                    foreach (var h in patient.Histories)
                    {
                        foreach (var pr in h.Prescriptions)
                        {
                            worksheet.Cells[currentRow, 1] = $"Рецепт №{pr.Id} от {pr.IssueDate.ToShortDateString()}";
                            ((Excel.Range)worksheet.Cells[currentRow, 1]).Font.Bold = true;
                            currentRow++;
                            worksheet.Cells[currentRow, 1] = $"Инструкция: {pr.Instruction}";
                            currentRow++;

                            foreach (var m in pr.Medicaments)
                            {
                                worksheet.Cells[currentRow, 2] = $"• {m.Name} ({m.Manufacturer})";
                                currentRow++;
                            }
                            currentRow++;
                        }
                    }
                }

                Excel.Range usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();
                if (includeHistory && includePrescriptions) ((Excel.Range)worksheet.Columns[5]).ColumnWidth = 55;

                workbook.SaveAs(filePath);
            }
            finally
            {
                if (workbook != null) { workbook.Close(false); Marshal.ReleaseComObject(workbook); }
                if (workbooks != null) Marshal.ReleaseComObject(workbooks);
                if (worksheet != null) Marshal.ReleaseComObject(worksheet);
                if (sheets != null) Marshal.ReleaseComObject(sheets);
                if (excelApp != null) { excelApp.Quit(); Marshal.ReleaseComObject(excelApp); }
                GC.Collect();
            }
        }

        public void ExportToWordFromSql(PatientDto patient, string filePath, bool includePersonalInfo, bool includeHistory, bool includePrescriptions)
        {
            if (patient == null) return;

            Word.Application wordApp = null;
            Word.Documents documents = null;
            Word.Document document = null;

            try
            {
                wordApp = new Word.Application();
                documents = wordApp.Documents;
                document = documents.Add();

                // Главный заголовок — ЗАГОВОЛОК (Жирный, Левый край)
                Word.Paragraph titlePara = document.Paragraphs.Add();
                titlePara.Range.Text = "Выписка из медицинской карты";
                titlePara.Range.Font.Size = 16;
                titlePara.Range.Font.Bold = 1; // Жирный
                titlePara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft; // Левый край
                titlePara.Range.InsertParagraphAfter();

                // Период — Обычный текст (НЕ жирный, Левый край)
                Word.Paragraph periodPara = document.Paragraphs.Add();
                periodPara.Range.Text = $"";
                periodPara.Range.Font.Size = 11;
                periodPara.Range.Font.Bold = 0; // Обычный
                periodPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft; // Левый край
                periodPara.Range.InsertParagraphAfter();

                // Чекбокс: Личные данные
                if (includePersonalInfo)
                {
                    Word.Paragraph infoPara = document.Paragraphs.Add();
                    infoPara.Range.Text = $"Пациент: {patient.Surname} {patient.FirstName}\n" +
                                          $"Номер полиса: {patient.NumberPolicy}\n" +
                                          $"Дата рождения: {patient.DateOfBirth?.ToShortDateString()}\n";
                    infoPara.Range.Font.Size = 11;
                    infoPara.Range.Font.Bold = 0; // Обычный
                    infoPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    infoPara.Range.InsertParagraphAfter();
                }

                // Чекбокс: История приёмов
                if (includeHistory)
                {
                    // Заголовок раздела — ЗАГОВОЛОК (Жирный)
                    Word.Paragraph historyHeader = document.Paragraphs.Add();
                    historyHeader.Range.Text = "Раздел: Сведения о посещениях врача";
                    historyHeader.Range.Font.Size = 13;
                    historyHeader.Range.Font.Bold = 1; // Жирный
                    historyHeader.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    historyHeader.Range.InsertParagraphAfter();

                    int index = 1;
                    foreach (var h in patient.Histories)
                    {
                        Word.Paragraph hPara = document.Paragraphs.Add();
                        hPara.Range.Text = $"{index}. Приём от {h.Date.ToShortDateString()}\n" +
                                           $"   Врач: {h.DoctorName}\n" +
                                           $"   Жалобы: {h.Symptoms}\n" +
                                           $"   Статус приёма: {h.Status}";
                        hPara.Range.Font.Size = 11;
                        hPara.Range.Font.Bold = 0; // Обычный
                        hPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        hPara.Range.InsertParagraphAfter();

                        // Вложенный чекбокс рецептов внутри приёма
                        if (includePrescriptions && h.Prescriptions.Any())
                        {
                            foreach (var pr in h.Prescriptions)
                            {
                                Word.Paragraph rPara = document.Paragraphs.Add();
                                rPara.Range.Text = $"      => Выписан Рецепт №{pr.Id} (Инструкция: {pr.Instruction}):\n" +
                                                   $"         Препараты:";
                                rPara.Range.Font.Size = 11;
                                rPara.Range.Font.Bold = 0; // Обычный
                                rPara.Range.Font.Italic = 1; // Курсив для наглядности структуры (не жирный)
                                rPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                rPara.Range.InsertParagraphAfter();

                                foreach (var m in pr.Medicaments)
                                {
                                    Word.Paragraph mPara = document.Paragraphs.Add();
                                    mPara.Range.Text = $"           • {m.Name} | Производитель: {m.Manufacturer} | {m.Price} руб.";
                                    mPara.Range.Font.Size = 11;
                                    mPara.Range.Font.Bold = 0; // Обычный
                                    mPara.Range.Font.Italic = 0;
                                    mPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                    mPara.Range.InsertParagraphAfter();
                                }
                            }
                        }
                        index++;
                    }
                }
                else if (includePrescriptions)
                {
                    // Заголовок раздела — ЗАГОВОЛОК (Жирный)
                    Word.Paragraph prescriptionHeader = document.Paragraphs.Add();
                    prescriptionHeader.Range.Text = "Раздел: Реестр выписанных рецептов за период";
                    prescriptionHeader.Range.Font.Size = 13;
                    prescriptionHeader.Range.Font.Bold = 1; // Жирный
                    prescriptionHeader.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    prescriptionHeader.Range.InsertParagraphAfter();

                    foreach (var h in patient.Histories)
                    {
                        foreach (var pr in h.Prescriptions)
                        {
                            Word.Paragraph rPara = document.Paragraphs.Add();
                            rPara.Range.Text = $"• Рецепт №{pr.Id} от {pr.IssueDate.ToShortDateString()}\n" +
                                               $"  Применение: {pr.Instruction}\n" +
                                               $"  Состав лекарств:";
                            rPara.Range.Font.Size = 11;
                            rPara.Range.Font.Bold = 0; // Обычный
                            rPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                            rPara.Range.InsertParagraphAfter();

                            foreach (var m in pr.Medicaments)
                            {
                                Word.Paragraph mPara = document.Paragraphs.Add();
                                mPara.Range.Text = $"    - {m.Name} ({m.Manufacturer})";
                                mPara.Range.Font.Size = 11;
                                mPara.Range.Font.Bold = 0; // Обычный
                                mPara.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                                mPara.Range.InsertParagraphAfter();
                            }
                        }
                    }
                }

                object filename = filePath;
                document.SaveAs2(ref filename);
            }
            finally
            {
                if (document != null) { document.Close(false); Marshal.ReleaseComObject(document); }
                if (documents != null) Marshal.ReleaseComObject(documents);
                if (wordApp != null) { wordApp.Quit(); Marshal.ReleaseComObject(wordApp); }
                GC.Collect();
            }
        }
    }
}
