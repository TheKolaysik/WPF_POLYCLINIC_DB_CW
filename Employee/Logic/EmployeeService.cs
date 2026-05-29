using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Models;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.Logic
{
    public class EmployeeService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
        public bool isVisitPacient = false;
        public List<Doctor> GetDoctorsWithSpecialties(string connectionString)
        {
            string sql = @"
            SELECT d.ID_doctor, d.FirstName, d.SurName, d.Cabinet, s.Name AS SpecializationName
            FROM [Врач] d
            INNER JOIN [Специальность] s ON d.ID_specialization = s.ID_specialization";

            return connection.Query<Doctor>(sql).ToList();
        }
        public void SaveNewDoctor(
            string firstName,
            string surName,
            string cabinet,
            string experience,
            string phone,
            string idSpecialization,
            bool working,
            bool vacation,
            string login,
            string password)
        {
            // Вычисляем хеш пароля в массив байтов
            byte[] passwordHash = HashPasswordToBytes(password);

            string sql = @"INSERT INTO [Врач] 
                       (FirstName, SurName, Cabinet, Experience, Phone, ID_specialization, Working, Vacation, Login, Password) 
                       VALUES 
                       (@name, @lastName, @cabinet, @exp, @phone, @idSpec, @working, @vacation, @login, @passwordHash)";

            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                // Безопасный парсинг числовых значений
                int.TryParse(cabinet, out int cabinetNum);
                int.TryParse(experience, out int expNum);
                long.TryParse(idSpecialization, out long specId);

                // Добавление параметров
                cmd.Parameters.Add("@name", SqlDbType.NVarChar, 50).Value = firstName;
                cmd.Parameters.Add("@lastName", SqlDbType.NVarChar, 50).Value = surName;

              
                cmd.Parameters.Add("@cabinet", SqlDbType.Int).Value = (cabinetNum > 0) ? (object)cabinetNum : DBNull.Value;
                cmd.Parameters.Add("@exp", SqlDbType.Int).Value = (expNum >= 0 && !string.IsNullOrEmpty(experience)) ? (object)expNum : DBNull.Value;

                cmd.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = phone;
                cmd.Parameters.Add("@idSpec", SqlDbType.BigInt).Value = specId;

                cmd.Parameters.Add("@working", SqlDbType.Bit).Value = working;
                cmd.Parameters.Add("@vacation", SqlDbType.Bit).Value = vacation;

                cmd.Parameters.Add("@login", SqlDbType.VarChar, 30).Value = login;
                cmd.Parameters.Add("@passwordHash", SqlDbType.VarBinary, 32).Value = passwordHash;

                // Выполнение запроса
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            System.Windows.Forms.MessageBox.Show("Врач успешно добавлен");
        }

        // Метод хеширования пароля
        private byte[] HashPasswordToBytes(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public Doctor AuthenticateEmployee(string login, string password)
        {

            // Получаем пациента по логину
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
         FROM [Врач] d LEFT JOIN [Специальность] spec ON d.ID_specialization = spec.ID_specialization WHERE Login = @Login",
                new { Login = login }
            );

            if (doctor == null) throw new Exception("Сотрудник с таким логином не существует!");

            // Вычисляем хеш введенного пароля
            byte[] inputHash = HashPasswordToBytes(password);

            // Сравниваем байтовые массивы
            if (StructuralComparisons.StructuralEqualityComparer.Equals(doctor.Password, inputHash))
            {
                return doctor;
            }

            throw new Exception("Неверный пароль!"); // Пароль неверный
        }

        public List<string> GetFreeSlots(long doctorId, DateTime date)
        {
            // Расписание врача 
            TimeSpan startShift = new TimeSpan(8, 0, 0);  // 08:00
            TimeSpan endShift = new TimeSpan(17, 0, 0);  // 17:00
            TimeSpan slotDuration = TimeSpan.FromMinutes(30);

            // Генерируем все потенциальные слоты для записи на этот день
            List<TimeSpan> allSlots = new List<TimeSpan>();
            for (TimeSpan slot = startShift; slot < endShift; slot = slot.Add(slotDuration))
            {
                allSlots.Add(slot);
            }

            string sql = @"
                SELECT CAST([Date] AS TIME) 
                FROM [История] 
                WHERE ID_doctor = @DoctorId 
                  AND CAST([Date] AS DATE) = CAST(@Date AS DATE)";

            using (var conn = new SqlConnection(connectionString))
            {
                // Получаем список занятых 
                var busySlots = conn.Query<TimeSpan>(sql, new { DoctorId = doctorId, Date = date.Date }).ToList();

                // Исключаем занятые слоты из общего расписания
                var freeSlots = allSlots.Where(slot => !busySlots.Contains(slot))
                                         .Select(slot => slot.ToString(@"hh\:mm"))
                                         .ToList();

                return freeSlots;
            }
        }
        public void AddAppointment(long patientId, long doctorId, DateTime appointmentDateTime)
        {
            // Проверяем, что время кратно 30 минутам 
            if (appointmentDateTime.Minute % 30 != 0)
            {
                throw new Exception("Запись возможна только на ровные слоты (например, 10:00 или 10:30).");
            }

            string sqlCheck = @"
                SELECT COUNT(1) 
                FROM [История] 
                WHERE ID_doctor = @DoctorId 
                  AND [Date] = @AppointmentDateTime";

            // Запрос на добавление новой записи в историю
            string sqlInsert = @"
                INSERT INTO [История] (ID_pacient, ID_doctor, [Date]) 
                VALUES (@PatientId, @DoctorId, @AppointmentDateTime)";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Проверяем, занято ли время
                        int isBusy = conn.ExecuteScalar<int>(sqlCheck,
                            new { DoctorId = doctorId, AppointmentDateTime = appointmentDateTime },
                            transaction);

                        if (isBusy > 0)
                        {
                            throw new Exception("Извините, это время уже занято другим пациентом. Обновите расписание.");
                        }

                        // Если свободно — добавляем запись
                        conn.Execute(sqlInsert,
                            new { PatientId = patientId, DoctorId = doctorId, AppointmentDateTime = appointmentDateTime },
                            transaction);

                        // Подтверждаем транзакцию
                        transaction.Commit();
                        System.Windows.MessageBox.Show("Вы успешно записаны на прием!", "Успех",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // В случае ошибки или если слот занят — отменяем операцию
                        transaction.Rollback();
                        System.Windows.MessageBox.Show($"Не удалось записаться: {ex.Message}", "Ошибка",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }
        public void CreatePrescription(long historyId, long doctorId, DateTime issueDate, string instruction)
        {
            // Запрос на создание рецепта. Возвращает ID созданной записи.
            string sqlInsertPrescription = @"
                INSERT INTO [Рецепт] (IssueDate, Instruction, ID_doctor)
                OUTPUT INSERTED.ID_prescription
                VALUES (@IssueDate, @Instruction, @IdDoctor);";

            // Запрос на привязку рецепта к конкретной записи в истории
            string sqlLinkToHistory = @"
                INSERT INTO [История_Рецепт] (ID_history, ID_prescription)
                VALUES (@HistoryId, @PrescriptionId);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            long newPrescriptionId;

                            using (SqlCommand cmdPrescription = new SqlCommand(sqlInsertPrescription, conn, transaction))
                            {
                                cmdPrescription.Parameters.Add("@IssueDate", SqlDbType.Date).Value = issueDate.Date;
                                cmdPrescription.Parameters.Add("@Instruction", SqlDbType.NVarChar).Value = (object)instruction ?? DBNull.Value;
                                cmdPrescription.Parameters.Add("@IdDoctor", SqlDbType.BigInt).Value = doctorId;
                                newPrescriptionId = Convert.ToInt64(cmdPrescription.ExecuteScalar());
                            }

                            using (SqlCommand cmdLink = new SqlCommand(sqlLinkToHistory, conn, transaction))
                            {
                                cmdLink.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                                cmdLink.Parameters.Add("@PrescriptionId", SqlDbType.BigInt).Value = newPrescriptionId;

                                cmdLink.ExecuteNonQuery();
                            }

                            
                            transaction.Commit();
                            System.Windows.Forms.MessageBox.Show($"Рецепт успешно создан и привязан к истории!", "Успех",
                                (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Ошибка внутри транзакции: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"Не удалось создать рецепт: {ex.Message}", "Ошибка",
                        (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                }
            }
        }
        public void AddMedicamentToPrescription(long prescriptionId, long medicamentId)
        {
            string sql = @"
                INSERT INTO [Лекарство_Рецепт] (ID_prescription, ID_medicament)
                VALUES (@PrescriptionId, @MedicamentId)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Явно указываем типы данных для параметров, чтобы избежать ошибок типизации
                    cmd.Parameters.Add("@PrescriptionId", SqlDbType.BigInt).Value = prescriptionId;
                    cmd.Parameters.Add("@MedicamentId", SqlDbType.BigInt).Value = medicamentId;


                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery(); // Выполняем команду без возвращения строк

                        System.Windows.Forms.MessageBox.Show("Лекарство успешно добавлено в рецепт!", "Успех",
                            (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка при добавлении лекарства: {ex.Message}", "Ошибка",
                            (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    }
                }
            }
        }
        public void AddDiagnosisToHistory(long historyId, long diagnosisId)
        {
            string sql = @"
                INSERT INTO [История_Диагноз] (ID_history, ID_diagnosis)
                VALUES (@HistoryId, @DiagnosisId)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                    cmd.Parameters.Add("@DiagnosisId", SqlDbType.BigInt).Value = diagnosisId;

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();

                        System.Windows.Forms.MessageBox.Show("Диагноз успешно добавлен к записи истории!", "Успех",
                            MessageBoxButtons.OK, (MessageBoxIcon)MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка при добавлении диагноза: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, (MessageBoxIcon)MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
