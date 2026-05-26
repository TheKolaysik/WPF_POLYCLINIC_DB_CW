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

                // Передаем байты напрямую в varbinary(32)
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
    }
}
