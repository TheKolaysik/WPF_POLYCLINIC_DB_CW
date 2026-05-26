using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_POLYCLINIC_DB_CourseWork
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем и запускаем форму WinForms
            LoginForm form = new LoginForm();
            form.ShowDialog();

            // Закрываем приложение, когда форма закрывается
            this.Shutdown();
        }
    }
}
