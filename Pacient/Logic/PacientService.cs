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
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Models;

namespace WPF_POLYCLINIC_DB_CourseWork.Employee.Logic
{
    public class PacientService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlDataAdapter adapter;
        private SqlCommand cmd;

        public void SaveNewPacient(string firstName, string surName, string gender, DateTime dateTime, string placeOfLiving, string phone, string districtDoctor, string numberPolicy, string login, string password)
        {
            // Сначала вычисляем хеш 
            byte[] passwordHash = HashPasswordToBytes(password);

            string sql = @"INSERT INTO [Пациент] 
                   (FirstName, SurName, PlaceOfLiving, Gender, NumberPolicy, DateOfBirth, DistrictDoctor, Login, PasswordHash, Phone) 
                   VALUES 
                   (@name, @lastName, @place, @gender, @number, @date, @disDoctor, @login, @passwordHash, @phone)";

            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                // Безопасно парсим ID врача
                int.TryParse(districtDoctor, out int doctorId);

                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = firstName;
                cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = surName;
                cmd.Parameters.Add("@place", SqlDbType.NVarChar).Value = placeOfLiving;
                cmd.Parameters.Add("@gender", SqlDbType.NVarChar).Value = gender;
                cmd.Parameters.Add("@number", SqlDbType.NVarChar).Value = numberPolicy;
                cmd.Parameters.Add("@date", SqlDbType.Date).Value = dateTime;
                cmd.Parameters.Add("@disDoctor", SqlDbType.BigInt).Value = (doctorId > 0) ? (object)doctorId : DBNull.Value;
                cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = login;

                cmd.Parameters.Add("@passwordHash", SqlDbType.VarBinary, 32).Value = passwordHash;

                cmd.Parameters.Add("@phone", SqlDbType.VarChar).Value = phone;

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            MessageBox.Show("Пациент добавлен");
        }
        private byte[] HashPasswordToBytes(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public Patient AuthenticatePatient(string login, string password)
        {

            // Получаем пациента по логину
            var patient = connection.QueryFirstOrDefault<Patient>(
                "SELECT * FROM [Пациент] WHERE Login = @Login",
                new { Login = login }
            );

            if (patient == null) throw new Exception("Пациент с таким логином не существует!");

            // Вычисляем хеш введенного пароля
            byte[] inputHash = HashPasswordToBytes(password); 

            // Сравниваем байтовые массивы
            if (StructuralComparisons.StructuralEqualityComparer.Equals(patient.PasswordHash, inputHash))
            {
                return patient;
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
                // Получаем список занятых TimeSpan
                var busySlots = conn.Query<TimeSpan>(sql, new { DoctorId = doctorId, Date = date.Date }).ToList();

                // Исключаем занятые слоты из общего расписания
                var freeSlots = allSlots.Where(slot => !busySlots.Contains(slot))
                                         .Select(slot => slot.ToString(@"hh\:mm"))
                                         .ToList();

                return freeSlots;
            }
        }
        public void AddAppointment(long patientId, long doctorId, DateTime appointmentDateTime, string Symptoms)
        {
            // Проверяем, что время кратно 30 минутам 
            if (appointmentDateTime.Minute % 30 != 0)
            {
                throw new Exception("Запись возможна только на ровные слоты (например, 10:00 или 10:30).");
            }

            // Запрос, который проверяет наличие записи к этому врачу именно на это время
            string sqlCheck = @"
                SELECT COUNT(1) 
                FROM [История] 
                WHERE ID_doctor = @DoctorId 
                  AND [Date] = @AppointmentDateTime";

            // Запрос на добавление новой записи в историю
            string sqlInsert = @"
                INSERT INTO [История] (ID_pacient, ID_doctor, [Date], Status, Symptoms) 
                VALUES (@PatientId, @DoctorId, @AppointmentDateTime, 'Предстоящий', @Symptoms)";



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
                            throw new Exception("Извините, это время уже занято другим пациентом");
                        }                      
                        conn.Execute(sqlInsert,
                            new { PatientId = patientId, DoctorId = doctorId, AppointmentDateTime = appointmentDateTime, Symptoms = Symptoms },
                            transaction);

                        // Подтверждаем транзакцию
                        transaction.Commit();
                        System.Windows.MessageBox.Show("Вы успешно записаны на прием!", "Успех",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // В случае ошибки отменяем операцию
                        transaction.Rollback();
                        System.Windows.MessageBox.Show($"Не удалось записаться: {ex.Message}", "Ошибка",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
