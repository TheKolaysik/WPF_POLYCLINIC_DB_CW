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

            throw new Exception("Неверный пароль!");
        }

        // Получить свободное время у врача
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
        // Запись на приём
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
        // Создать рецепт
        public long CreatePrescription(long historyId, long doctorId, DateTime issueDate, string instruction)
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
            long newPrescriptionId = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {

                            using (SqlCommand cmdPrescription = new SqlCommand(sqlInsertPrescription, conn, transaction))
                            {
                                cmdPrescription.Parameters.Add("@IssueDate", SqlDbType.Date).Value = issueDate.Date;
                                cmdPrescription.Parameters.Add("@Instruction", SqlDbType.NVarChar).Value = instruction;
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
                return newPrescriptionId;
            }
            
        }
        // Добавить лекарство в рецепт
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
                        cmd.ExecuteNonQuery(); 
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
        // Добавить диагноз
        public void AddDiagnosisToHistory(long historyId, long diagnosisId)
        {
            string sql = @"
                INSERT INTO [Диагноз_История] (ID_history, ID_diagnos)
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

        // Завершить приём
        public void CompleteAppointment(long idHistory)
        {
            string sql = @"
                UPDATE [История]
                SET [Status] = N'Завершённый'
                WHERE [ID_history] = @IdHistory";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@IdHistory", SqlDbType.BigInt).Value = idHistory;

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();


                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } 
            } 
        }

        // ----------------------------------------------------
        // МЕТОДЫ АДМИНИСТРИРОВАНИЯ
        // ----------------------------------------------------
        public void DeleteDoctorAndReassignPatients(long deletingDoctorId)
        {
            // Поиск нового наименее загруженного врача той же специальности
            string sqlFindNewDoctor = @"
                SELECT TOP 1 d.ID_doctor
                FROM [Врач] d
                LEFT JOIN [Пациент] p ON d.ID_doctor = p.DistrictDoctor
                WHERE d.ID_specialization = (SELECT ID_specialization FROM [Врач] WHERE ID_doctor = @DeletingId)
                  AND d.ID_doctor <> @DeletingId
                  AND d.Working = 1
                  AND d.Vacation = 0
                GROUP BY d.ID_doctor
                ORDER BY COUNT(p.ID_pacient) ASC";

            // Перевод пациентов к новому врачу
            string sqlReassignPatients = @"
                UPDATE [Пациент]
                SET DistrictDoctor = @NewDoctorId
                WHERE DistrictDoctor = @DeletingId;";

            // Удаляем связи лекарств с рецептами для всех приёмов этого врача
            string sqlDeleteMedicaments = @"
                DELETE lr 
                FROM [Лекарство_Рецепт] lr
                INNER JOIN [История_Рецепт] hr ON lr.ID_prescription = hr.ID_prescription
                INNER JOIN [История] h ON hr.ID_history = h.ID_history
                WHERE h.ID_doctor = @DeletingId";

            // Удаляем связи из таблицы История-Рецепт
            string sqlDeleteHistoryPrescriptions = @"
                DELETE hr 
                FROM [История_Рецепт] hr
                INNER JOIN [История] h ON hr.ID_history = h.ID_history
                WHERE h.ID_doctor = @DeletingId";

            // удаляем рецепты 
            string sqlDeletePrescriptions = @"
                DELETE FROM [Рецепт] 
                WHERE ID_doctor = @DeletingId";

            // Удаляем связи приёмов этого врача с диагнозами
            string sqlDeleteHistoryDiagnoses = @"
                DELETE dh 
                FROM [Диагноз_История] dh
                INNER JOIN [История] h ON dh.ID_history = h.ID_history
                WHERE h.ID_doctor = @DeletingId";

            // Удаляем записи приёмов из истории
            string sqlDeleteHistory = @"
                DELETE FROM [История] 
                WHERE ID_doctor = @DeletingId";

            // Удаление врача
            string sqlDeleteDoctor = @"
                DELETE FROM [Врач]
                WHERE ID_doctor = @DeletingId";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        long? newDoctorId = null;

                        // Ищем нового врача
                        using (SqlCommand cmdFind = new SqlCommand(sqlFindNewDoctor, conn, transaction))
                        {
                            cmdFind.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            object result = cmdFind.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                newDoctorId = Convert.ToInt64(result);
                            }
                        }

                        // Защитная проверка на случай, если врач единственный в своей специальности
                        if (newDoctorId == null)
                        {
                            string sqlCheckPatients = "SELECT COUNT(*) FROM [Пациент] WHERE DistrictDoctor = @DeletingId";
                            int patientCount = 0;

                            using (SqlCommand cmdCheck = new SqlCommand(sqlCheckPatients, conn, transaction))
                            {
                                cmdCheck.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                                patientCount = (int)cmdCheck.ExecuteScalar();
                            }

                            if (patientCount > 0)
                            {
                                throw new Exception("Невозможно удалить врача. В системе нет других активных врачей этой специальности для перевода пациентов!");
                            }
                        }

                        // Переводим пациентов к новому врачу
                        if (newDoctorId != null)
                        {
                            using (SqlCommand cmdReassign = new SqlCommand(sqlReassignPatients, conn, transaction))
                            {
                                cmdReassign.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                                cmdReassign.Parameters.Add("@NewDoctorId", SqlDbType.BigInt).Value = newDoctorId.Value;
                                cmdReassign.ExecuteNonQuery();
                            }
                        }

                        // Удаляем медикаменты из рецептов
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteMedicaments, conn, transaction))
                        {
                            cmd.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем связи История-Рецепт
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryPrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmd.ExecuteNonQuery();
                        }

                        // Теперь удаляем сами рецепты по прямой связи с доктором
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем связи Истории с Диагнозами
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryDiagnoses, conn, transaction))
                        {
                            cmd.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем историю приёмов этого врача
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistory, conn, transaction))
                        {
                            cmd.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmd.ExecuteNonQuery();
                        }

                        // удаляем самого врача
                        using (SqlCommand cmdDelete = new SqlCommand(sqlDeleteDoctor, conn, transaction))
                        {
                            cmdDelete.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmdDelete.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        System.Windows.Forms.MessageBox.Show("Врач успешно удален. Его приёмы, рецепты и диагнозы стерты, а пациенты переведены к новому доктору.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show($"Ошибка при удалении врача и его приёмов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void DeletePatientWithHistoryAndPrescriptions(long patientId)
        {
            // Удаляем связи лекарств с рецептами этого пациента
            string sqlDeleteMedicaments = @"
                DELETE lr 
                FROM [Лекарство_Рецепт] lr
                INNER JOIN [История_Рецепт] hr ON lr.ID_prescription = hr.ID_prescription
                INNER JOIN [История] h ON hr.ID_history = h.ID_history
                WHERE h.ID_pacient = @PatientId";

            // Удаляем рецепты пациента
            string sqlDeletePrescriptions = @"
                DELETE r 
                FROM [Рецепт] r
                INNER JOIN [История_Рецепт] hr ON r.ID_prescription = hr.ID_prescription
                INNER JOIN [История] h ON hr.ID_history = h.ID_history
                WHERE h.ID_pacient = @PatientId";

            // Удаляем связи рецептов и истории
            string sqlDeleteHistoryPrescriptions = @"
                DELETE hr 
                FROM [История_Рецепт] hr
                INNER JOIN [История] h ON hr.ID_history = h.ID_history
                WHERE h.ID_pacient = @PatientId";

            // Удаляем связи истории и диагнозов 
            string sqlDeleteHistoryDiagnoses = @"
                DELETE dh 
                FROM [Диагноз_История] dh
                INNER JOIN [История] h ON dh.ID_history = h.ID_history
                WHERE h.ID_pacient = @PatientId";

            // Удаляем записи из истории приемов пациента
            string sqlDeleteHistory = @"DELETE FROM [История] WHERE ID_pacient = @PatientId";

            // Удаляем самого пациента
            string sqlDeletePatient = @"DELETE FROM [Пациент] WHERE ID_pacient = @PatientId";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                conn.Open();

                // Запускаем транзакцию
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Удаляем лекарства из рецептов
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteMedicaments, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }
                        // Удаляем связи История-Рецепт
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryPrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }
                        // Удаляем рецепты
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем связи приёмов с диагнозами
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryDiagnoses, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем саму историю
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistory, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем пациента
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePatient, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception("Пациент с таким ID не найден в базе данных.");
                            }
                        }

                        // Если все шаги выполнены без ошибок, сохраняем изменения в БД
                        transaction.Commit();
                        System.Windows.Forms.MessageBox.Show("Данные пациента, включая историю, диагнозы и рецепты, успешно удалены.", "Успех", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Если произошла ошибка, откатываем любые изменения обратно
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show($"Ошибка при каскадном удалении: {ex.Message}", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }
        public void DeleteHistoryRecord(long historyId)
        {
            // Удаляем связи лекарств с рецептами, выписанными на этом конкретном приёме
            string sqlDeleteMedicaments = @"
                DELETE lr 
                FROM [Лекарство_Рецепт] lr
                INNER JOIN [История_Рецепт] hr ON lr.ID_prescription = hr.ID_prescription
                WHERE hr.ID_history = @HistoryId";

            // Удаляем рецепты, привязанные к этому приёму
            string sqlDeletePrescriptions = @"
                DELETE r 
                FROM [Рецепт] r
                INNER JOIN [История_Рецепт] hr ON r.ID_prescription = hr.ID_prescription
                WHERE hr.ID_history = @HistoryId";

            // Удаляем записи из связующей таблицы 
            string sqlDeleteHistoryPrescriptions = @"
                DELETE FROM [История_Рецепт] 
                WHERE ID_history = @HistoryId";

            // Удаляем связи этого приёма с диагнозами из таблицы 
            string sqlDeleteHistoryDiagnoses = @"
                DELETE FROM [Диагноз_История] 
                WHERE ID_history = @HistoryId";

            // Удаляем запись приёма из таблицы
            string sqlDeleteHistory = @"
                DELETE FROM [История] 
                WHERE ID_history = @HistoryId";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                conn.Open();

                // Открываем транзакцию для безопасности данных
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Очищаем лекарства в рецептах этого приёма
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteMedicaments, conn, transaction))
                        {
                            cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем связи приёма и рецептов
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryPrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем бланки рецептов этого приёма
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                            cmd.ExecuteNonQuery();
                        }                       

                        // Удаляем связи приёма и диагнозов
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryDiagnoses, conn, transaction))
                        {
                            cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем запись о приёме
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistory, conn, transaction))
                        {
                            cmd.Parameters.Add("@HistoryId", SqlDbType.BigInt).Value = historyId;
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception("Запись в истории с таким ID не найдена.");
                            }
                        }

                        // Если всё прошло успешно  — фиксируем изменения в БД
                        transaction.Commit();
                        System.Windows.Forms.MessageBox.Show("Запись приёма, а также все связанные диагнозы и рецепты успешно удалены.", "Успех", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // При ошибке откатываем базу в исходное состояние
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show($"Ошибка при удалении приёма: {ex.Message}", "Ошибка", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void UpdateDiagnosis(int diagnosisId, string name, string description)
        {
            string sql = @"
                UPDATE [Диагноз] 
                SET Name = @Name, Description = @Description 
                WHERE ID_diagnos = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = diagnosisId;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdatePatient(int patientId, string surname, string firstName, string numberPolicy, DateTime? dateOfBirth)
        {
            string sql = @"
                UPDATE [Пациент] 
                SET Surname = @Surname, FirstName = @FirstName, NumberPolicy = @Policy, DateOfBirth = @BirthDate 
                WHERE ID_pacient = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = patientId;
                    cmd.Parameters.Add("@Surname", SqlDbType.NVarChar).Value = surname;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = firstName;
                    cmd.Parameters.Add("@Policy", SqlDbType.NVarChar).Value = numberPolicy;
                    cmd.Parameters.Add("@BirthDate", SqlDbType.DateTime).Value = dateOfBirth;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateSpecialization(int specId, string name, string description)
        {
            string sql = @"
                UPDATE [Специальность] 
                SET Name = @Name, Responsibility = @Description 
                WHERE ID_specialization = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = specId;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateDoctor(int doctorId, string surname, string firstName, string cabinet)
        {
            string sql = @"
                UPDATE [Врач] 
                SET Surname = @Surname, FirstName = @FirstName, Cabinet = @Cabinet 
                WHERE ID_doctor = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = doctorId;
                    cmd.Parameters.Add("@Surname", SqlDbType.NVarChar).Value = surname;
                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = firstName;
                    cmd.Parameters.Add("@Cabinet", SqlDbType.NVarChar).Value = cabinet;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateDoctorVacation(int doctorId, bool vacation, bool working)
        {
            string sql = @"
                UPDATE [Врач] 
                SET Working = @Working, Vacation = @Vacation 
                WHERE ID_doctor = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = doctorId;
                    cmd.Parameters.Add("@Working", SqlDbType.Bit).Value = working;
                    cmd.Parameters.Add("@Vacation", SqlDbType.Bit).Value = vacation;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateMedicament(int medicamentId, string name, string manufacturer, decimal price, string sideEffects, string description)
        {
            string sql = @"
                UPDATE [Лекарство] 
                SET Name = @Name, Manufacturer = @Manufacturer, Price = @Price, SideEffects = @SideEffects, MethodOfApplication = @Description 
                WHERE ID_medicament = @Id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = medicamentId;
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Manufacturer", SqlDbType.NVarChar).Value = manufacturer;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@SideEffects", SqlDbType.NVarChar).Value = sideEffects;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddSpecialization(string name, string description)
        {
            // Запрос вставляет строку и сразу возвращает сгенерированный ID
            string sql = @"
                INSERT INTO [Специальность] (Name, Responsibility) 
                VALUES (@Name, @Description);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddDiagnosis(string name, string description)
        {
            string sql = @"
                INSERT INTO [Диагноз] (Name, Description) 
                VALUES (@Name, @Description);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void AddMedicament(string name, string manufacturer, decimal price, string sideEffects, string description)
        {
            string sql = @"
                INSERT INTO [Лекарство] (Name, Manufacturer, Price, SideEffects, MethodOfApplication) 
                VALUES (@Name, @Manufacturer, @Price, @SideEffects, @Description);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                    cmd.Parameters.Add("@Manufacturer", SqlDbType.NVarChar).Value = manufacturer;
                    cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = price;
                    cmd.Parameters.Add("@SideEffects", SqlDbType.NVarChar).Value = sideEffects;
                    cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = description;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
