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
                    // Привязываем параметр типа BigInt (long)
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
        public void DeleteDoctorAndReassignPatients(long deletingDoctorId)
        {
            // 1. Запрос поиска нового врача: ищем доктора той же специальности, который работает, не в отпуске, 
            // не является удаляемым, и у него МИНИМУМ пациентов на участке (через LEFT JOIN и COUNT).
            string sqlFindNewDoctor = @"
                SELECT TOP 1 d.ID_doctor
                FROM [Врач] d
                LEFT JOIN [Пациент] p ON d.ID_doctor = p.ID_doctor
                WHERE d.ID_specialization = (SELECT ID_specialization FROM [Врач] WHERE ID_doctor = @DeletingId)
                  AND d.ID_doctor <> @DeletingId
                  AND d.Working = 1
                  AND d.Vacation = 0
                GROUP BY d.ID_doctor
                ORDER BY COUNT(p.ID_pacient) ASC";

            // Запрос на перевод пациентов к новому врачу
            string sqlReassignPatients = @"
                UPDATE [Пациент]
                SET ID_doctor = @NewDoctorId
                WHERE ID_doctor = @DeletingId;";

            // Запрос на удаление старого врача
            string sqlDeleteDoctor = @"
                DELETE FROM [Врач]
                WHERE ID_doctor = @DeletingId";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                conn.Open();

                // Открываем транзакцию
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

                        // Если у удаляемого врача были пациенты, но замену мы не нашли (он был единственным специалистом)
                        if (newDoctorId == null)
                        {
                            // Проверяем, есть ли вообще у него пациенты
                            string sqlCheckPatients = "SELECT COUNT(*) FROM [Пациент] WHERE ID_doctor = @DeletingId";
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

                        // Если замена найдена, переводим пациентов
                        if (newDoctorId != null)
                        {
                            using (SqlCommand cmdReassign = new SqlCommand(sqlReassignPatients, conn, transaction))
                            {
                                cmdReassign.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                                cmdReassign.Parameters.Add("@NewDoctorId", SqlDbType.BigInt).Value = newDoctorId.Value;
                                cmdReassign.ExecuteNonQuery();
                            }
                        }

                        // Удаляем врача
                        using (SqlCommand cmdDelete = new SqlCommand(sqlDeleteDoctor, conn, transaction))
                        {
                            cmdDelete.Parameters.Add("@DeletingId", SqlDbType.BigInt).Value = deletingDoctorId;
                            cmdDelete.ExecuteNonQuery();
                        }

                        // Если всё прошло гладко — подтверждаем транзакцию
                        transaction.Commit();
                        System.Windows.Forms.MessageBox.Show("Врач успешно удален, его пациенты переведены к наименее загруженному коллеге.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // При любой ошибке откатываем все изменения назад
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show($"Ошибка при удалении врача: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            //  Удаляем связи рецептов и истории
            string sqlDeleteHistoryPrescriptions = @"
        DELETE hr 
        FROM [История_Рецепт] hr
        INNER JOIN [История] h ON hr.ID_history = h.ID_history
        WHERE h.ID_pacient = @PatientId";

            //  Удаляем сами рецепты пациента
            string sqlDeletePrescriptions = @"
        DELETE r 
        FROM [Рецепт] r
        INNER JOIN [История_Рецепт] hr ON r.ID_prescription = hr.ID_prescription
        INNER JOIN [История] h ON hr.ID_history = h.ID_history
        WHERE h.ID_pacient = @PatientId";

            //  Удаляем записи из истории приемов пациента
            string sqlDeleteHistory = @"
        DELETE FROM [История] 
        WHERE ID_pacient = @PatientId";

            //  Удаляем самого пациента
            string sqlDeletePatient = @"
        DELETE FROM [Пациент] 
        WHERE ID_pacient = @PatientId";

            using (SqlConnection conn = new SqlConnection(connection.ConnectionString))
            {
                conn.Open();

                // Запускаем транзакцию, чтобы гарантировать: либо удалится ВСЁ, либо НИЧЕГО
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Шаг 1: Удаляем лекарства из рецептов
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteMedicaments, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Шаг 2: Удаляем связи История-Рецепт
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistoryPrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Шаг 3: Удаляем рецепты
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePrescriptions, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Шаг 4: Удаляем историю
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteHistory, conn, transaction))
                        {
                            cmd.Parameters.Add("@PatientId", SqlDbType.BigInt).Value = patientId;
                            cmd.ExecuteNonQuery();
                        }

                        // Шаг 5: Удаляем пациента
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
                        System.Windows.Forms.MessageBox.Show("Данные пациента, включая историю и рецепты, успешно удалены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Если что-то пошло не так (например, оборвалось соединение), база вернется в исходное состояние
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show($"Ошибка при удалении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
