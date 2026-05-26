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
    internal class EmployeeService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlDataAdapter adapter;
        private SqlCommand cmd;
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

                // Если кабинет или стаж не указаны (равны 0), записываем DBNull.Value
                cmd.Parameters.Add("@cabinet", SqlDbType.Int).Value = (cabinetNum > 0) ? (object)cabinetNum : DBNull.Value;
                cmd.Parameters.Add("@exp", SqlDbType.Int).Value = (expNum >= 0 && !string.IsNullOrEmpty(experience)) ? (object)expNum : DBNull.Value;

                cmd.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = phone;
                cmd.Parameters.Add("@idSpec", SqlDbType.BigInt).Value = specId;

                cmd.Parameters.Add("@working", SqlDbType.Bit).Value = working;
                cmd.Parameters.Add("@vacation", SqlDbType.Bit).Value = vacation;

                cmd.Parameters.Add("@login", SqlDbType.VarChar, 30).Value = login;

                // Передаем байты хеша в колонку Password (убедитесь, что тип колонки изменен на varbinary(32))
                cmd.Parameters.Add("@passwordHash", SqlDbType.VarBinary, 32).Value = passwordHash;

                // Выполнение запроса
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

            MessageBox.Show("Врач успешно добавлен");
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
                "SELECT * FROM [Пациент] WHERE Login = @Login",
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
    }
}
