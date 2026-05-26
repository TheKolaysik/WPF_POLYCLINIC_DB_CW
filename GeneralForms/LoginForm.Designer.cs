namespace WPF_POLYCLINIC_DB_CourseWork
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAuthPacient = new System.Windows.Forms.Button();
            this.btnAuthEmployee = new System.Windows.Forms.Button();
            this.login = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRegistration = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.854546F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(49, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Логин";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.854546F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(49, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Пароль";
            // 
            // btnAuthPacient
            // 
            this.btnAuthPacient.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnAuthPacient.FlatAppearance.BorderSize = 0;
            this.btnAuthPacient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuthPacient.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.854546F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAuthPacient.Location = new System.Drawing.Point(179, 223);
            this.btnAuthPacient.Name = "btnAuthPacient";
            this.btnAuthPacient.Size = new System.Drawing.Size(129, 35);
            this.btnAuthPacient.TabIndex = 2;
            this.btnAuthPacient.Text = "Вход (пациент)";
            this.btnAuthPacient.UseVisualStyleBackColor = false;
            this.btnAuthPacient.Click += new System.EventHandler(this.btnAuthPacient_Click);
            // 
            // btnAuthEmployee
            // 
            this.btnAuthEmployee.BackColor = System.Drawing.Color.LimeGreen;
            this.btnAuthEmployee.FlatAppearance.BorderSize = 0;
            this.btnAuthEmployee.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuthEmployee.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.854546F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAuthEmployee.Location = new System.Drawing.Point(337, 223);
            this.btnAuthEmployee.Name = "btnAuthEmployee";
            this.btnAuthEmployee.Size = new System.Drawing.Size(143, 35);
            this.btnAuthEmployee.TabIndex = 3;
            this.btnAuthEmployee.Text = "Вход (сотрудник)";
            this.btnAuthEmployee.UseVisualStyleBackColor = true;
            this.btnAuthEmployee.Click += new System.EventHandler(this.btnAuthEmployee_Click);
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(149, 89);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(228, 20);
            this.login.TabIndex = 4;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(149, 149);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(228, 20);
            this.password.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 15.70909F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(101, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(276, 32);
            this.label3.TabIndex = 6;
            this.label3.Text = "ИС \"ПОЛИКЛИНИКА\"";
            // 
            // btnRegistration
            // 
            this.btnRegistration.BackColor = System.Drawing.Color.LemonChiffon;
            this.btnRegistration.FlatAppearance.BorderSize = 0;
            this.btnRegistration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistration.Font = new System.Drawing.Font("Microsoft YaHei UI", 7.854546F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRegistration.Location = new System.Drawing.Point(12, 223);
            this.btnRegistration.Name = "btnRegistration";
            this.btnRegistration.Size = new System.Drawing.Size(131, 35);
            this.btnRegistration.TabIndex = 7;
            this.btnRegistration.Text = "Регистрация";
            this.btnRegistration.UseVisualStyleBackColor = false;
            this.btnRegistration.Click += new System.EventHandler(this.btnRegistration_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PeachPuff;
            this.ClientSize = new System.Drawing.Size(492, 280);
            this.Controls.Add(this.btnRegistration);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.password);
            this.Controls.Add(this.login);
            this.Controls.Add(this.btnAuthEmployee);
            this.Controls.Add(this.btnAuthPacient);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "Авторизация";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAuthPacient;
        private System.Windows.Forms.Button btnAuthEmployee;
        private System.Windows.Forms.TextBox login;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRegistration;
    }
}