using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPF_POLYCLINIC_DB_CourseWork
{
    public partial class RegistrationForm : Form
    {
        PacientReport pacientReport = new PacientReport();
        Regex rgx1 = new Regex("[^a-zA-Za-яё]");
        String id_doctor;
        public RegistrationForm()
        {
            InitializeComponent();
            genderBox.SelectedIndex = 0;
            pacientReport.DisplayFreeDoctor(dataGridView: dataGridView);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String errorMessage;
            string fName = firstName.Text.Trim();
            string lName = surName.Text.Trim();
            string plOfLive = placeOfLiving.Text;
            string ph = phone.Text;
            string log = login.Text.Trim();
            string pass = password.Text.Trim();
            string passC = passwordConfirm.Text.Trim();
            formsBackColorWhite();

            if (dataGridView == null)
            {
                MessageBox.Show("Нет возмозности выбора участкового врача для регистрации!", "Данные отстутствуют", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            errorMessage = "";
            // Проверка имени
            if (string.IsNullOrEmpty(fName))
            {
                errorMessage += "Введите имя\n";
                firstName.BackColor = Color.Yellow;
            }
            else
            {
                if (fName.Length != rgx1.Replace(fName, "").Length)
                {
                    errorMessage += "Имя должно быть только из букв\n";
                    firstName.BackColor = Color.Red;
                }
            }

            // Проверка фамилии
            if (string.IsNullOrEmpty(lName))
            {
                errorMessage += "Введите фамилию\n";
                surName.BackColor = Color.Yellow;
            }
            else
            {
                if (lName.Length != rgx1.Replace(lName, "").Length)
                {
                    errorMessage += "Фамилия должна быть только из букв\n";
                    surName.BackColor = Color.Red;
                }
            }

            // Проверка места жительства
            if (string.IsNullOrEmpty(plOfLive))
            {
                errorMessage += "Введите место жительства\n";
                placeOfLiving.BackColor = Color.Yellow;
            }
            
            // Проверка номера телефона 
            if (string.IsNullOrEmpty(ph))
            {
                errorMessage += "Введите номер телефона\n";
                phone.BackColor = Color.Yellow;
            }
            else
            {
                string pattern = @"^\+?[\d\s\-\(\)]{7,15}$";
                if (!Regex.IsMatch(ph, pattern))
                {
                    errorMessage += "Номер имеет неправильный формат\n";
                    phone.BackColor= Color.Red;
                }
            }
            // Проверка логина
            if (string.IsNullOrEmpty(log))
            {
                errorMessage += "Введите логин\n";
                login.BackColor = Color.Yellow;
            }
            else
            {
                string pattern = @"^[a-zA-Z0-9_]{4,20}$";
                if (!Regex.IsMatch(log, pattern))
                {
                    errorMessage += "Неверный формат логина\n";
                    login.BackColor= Color.Red;
                }
                else
                {
                    if (pacientReport.GetPacientLoginIsExists(log))
                    {
                        errorMessage += "Такой логин уже существует\n";
                        login.BackColor = Color.Red;
                    }
                }
            }
            // Проверка пароля 
            if (string.IsNullOrEmpty(pass))
            {
                errorMessage += "Введите пароль\n";
                password.BackColor = Color.Yellow;
            }
            else
            {
                string pattern = @"^[a-zA-Z0-9_]{4,20}$";
                if (!Regex.IsMatch(pass, pattern))
                {
                    errorMessage += "Неверный формат пароля\n";
                    password.BackColor = Color.Red;
                }
                else
                {
                    if (!pass.Equals(passC)) 
                    {
                        errorMessage += "Пароли не совпадают\n";
                        passwordConfirm.BackColor = Color.Red;

                    }
                }
            }
            
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            PacientService pacientService = new PacientService();

            pacientService.SaveNewPacient(fName, lName, genderBox.Text, birthDay.Value, plOfLive, ph, id_doctor, pacientReport.GetUniqueOmsNumber(), log, pass);

        }
     
        private void dataGrid_RowHeaderMouseClick(object srnder, DataGridViewCellMouseEventArgs e)
        {
            id_doctor = dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
        }
        private void formsBackColorWhite()
        {
            firstName.BackColor = Color.White;
            surName.BackColor = Color.White;
            placeOfLiving.BackColor = Color.White;
            phone.BackColor = Color.White;
            login.BackColor = Color.White;
            password.BackColor = Color.White;
            passwordConfirm.BackColor = Color.White;
        }
    }

    // Генерация номера полиса ОМС
    public class OmsGenerator
    {
        private static Random _random = new Random();

        public static string GenerateValidOmsNumber()
        {
            // Генерируем первые 15 цифр
            int[] digits = new int[16];
            for (int i = 0; i < 15; i++)
            {
                digits[i] = _random.Next(0, 10);
            }

            // Вычисляем контрольную цифру
            digits[15] = CalculateLuhnCheckDigit(digits.Take(15).ToArray());

            return string.Concat(digits);
        }

        private static int CalculateLuhnCheckDigit(int[] digits)
        {
            int sum = 0;
            bool doubleDigit = true;

            // Идем с конца к началу
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int val = digits[i];
                if (doubleDigit)
                {
                    val *= 2;
                    if (val > 9) val -= 9;
                }
                sum += val;
                doubleDigit = !doubleDigit;
            }

            int remainder = sum % 10;
            return (remainder == 0) ? 0 : 10 - remainder;
        }
    }
}
