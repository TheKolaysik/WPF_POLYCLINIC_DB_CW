namespace WPF_POLYCLINIC_DB_CourseWork
{
    partial class RegistrationForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.firstName = new System.Windows.Forms.TextBox();
            this.surName = new System.Windows.Forms.TextBox();
            this.placeOfLiving = new System.Windows.Forms.TextBox();
            this.phone = new System.Windows.Forms.TextBox();
            this.birthDay = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.login = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.password = new System.Windows.Forms.TextBox();
            this.passwordConfirm = new System.Windows.Forms.TextBox();
            this.genderBox = new System.Windows.Forms.ComboBox();
            this.btnRegistration = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Фамилия";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(61, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Пол";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Дата рождения";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Место жительства";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(61, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Телефон";
            // 
            // firstName
            // 
            this.firstName.Location = new System.Drawing.Point(211, 45);
            this.firstName.Name = "firstName";
            this.firstName.Size = new System.Drawing.Size(281, 20);
            this.firstName.TabIndex = 8;
            // 
            // surName
            // 
            this.surName.Location = new System.Drawing.Point(211, 80);
            this.surName.Name = "surName";
            this.surName.Size = new System.Drawing.Size(281, 20);
            this.surName.TabIndex = 9;
            // 
            // placeOfLiving
            // 
            this.placeOfLiving.Location = new System.Drawing.Point(211, 198);
            this.placeOfLiving.Name = "placeOfLiving";
            this.placeOfLiving.Size = new System.Drawing.Size(281, 20);
            this.placeOfLiving.TabIndex = 10;
            // 
            // phone
            // 
            this.phone.Location = new System.Drawing.Point(211, 238);
            this.phone.Name = "phone";
            this.phone.Size = new System.Drawing.Size(281, 20);
            this.phone.TabIndex = 11;
            // 
            // birthDay
            // 
            this.birthDay.Location = new System.Drawing.Point(211, 157);
            this.birthDay.Name = "birthDay";
            this.birthDay.Size = new System.Drawing.Size(281, 20);
            this.birthDay.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(64, 281);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Логин";
            // 
            // login
            // 
            this.login.Location = new System.Drawing.Point(211, 278);
            this.login.Name = "login";
            this.login.Size = new System.Drawing.Size(281, 20);
            this.login.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(64, 319);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Пароль";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(64, 355);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Повтор пароля";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(67, 391);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 47;
            this.dataGridView.Size = new System.Drawing.Size(571, 187);
            this.dataGridView.TabIndex = 17;
            this.dataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_RowHeaderMouseClick);
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(211, 316);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(281, 20);
            this.password.TabIndex = 18;
            // 
            // passwordConfirm
            // 
            this.passwordConfirm.Location = new System.Drawing.Point(211, 352);
            this.passwordConfirm.Name = "passwordConfirm";
            this.passwordConfirm.Size = new System.Drawing.Size(281, 20);
            this.passwordConfirm.TabIndex = 19;
            // 
            // genderBox
            // 
            this.genderBox.FormattingEnabled = true;
            this.genderBox.Items.AddRange(new object[] {
            "Мужской",
            "Женский"});
            this.genderBox.Location = new System.Drawing.Point(211, 116);
            this.genderBox.Name = "genderBox";
            this.genderBox.Size = new System.Drawing.Size(281, 21);
            this.genderBox.TabIndex = 20;
            // 
            // btnRegistration
            // 
            this.btnRegistration.Location = new System.Drawing.Point(301, 596);
            this.btnRegistration.Name = "btnRegistration";
            this.btnRegistration.Size = new System.Drawing.Size(191, 40);
            this.btnRegistration.TabIndex = 21;
            this.btnRegistration.Text = "Зарегистрироваться";
            this.btnRegistration.UseVisualStyleBackColor = true;
            this.btnRegistration.Click += new System.EventHandler(this.button1_Click);
            // 
            // RegistrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 648);
            this.Controls.Add(this.btnRegistration);
            this.Controls.Add(this.genderBox);
            this.Controls.Add(this.passwordConfirm);
            this.Controls.Add(this.password);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.login);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.birthDay);
            this.Controls.Add(this.phone);
            this.Controls.Add(this.placeOfLiving);
            this.Controls.Add(this.surName);
            this.Controls.Add(this.firstName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegistrationForm";
            this.Text = "Регистрация пациента";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox firstName;
        private System.Windows.Forms.TextBox surName;
        private System.Windows.Forms.TextBox placeOfLiving;
        private System.Windows.Forms.TextBox phone;
        private System.Windows.Forms.DateTimePicker birthDay;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox login;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox passwordConfirm;
        private System.Windows.Forms.ComboBox genderBox;
        private System.Windows.Forms.Button btnRegistration;
    }
}