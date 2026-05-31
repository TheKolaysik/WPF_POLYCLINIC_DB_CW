namespace WPF_POLYCLINIC_DB_CourseWork.GeneralForms
{
    partial class ExportFormEmployee
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
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.chkPrescriptions = new System.Windows.Forms.CheckBox();
            this.chkHistory = new System.Windows.Forms.CheckBox();
            this.chkPersonalInfo = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(251, 215);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(153, 43);
            this.button2.TabIndex = 9;
            this.button2.Text = "Экспорт в Excel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(47, 215);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 43);
            this.button1.TabIndex = 8;
            this.button1.Text = "Экспорт в Word";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkPrescriptions
            // 
            this.chkPrescriptions.AutoSize = true;
            this.chkPrescriptions.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkPrescriptions.Location = new System.Drawing.Point(47, 99);
            this.chkPrescriptions.Name = "chkPrescriptions";
            this.chkPrescriptions.Size = new System.Drawing.Size(357, 24);
            this.chkPrescriptions.TabIndex = 7;
            this.chkPrescriptions.Text = "Включить подробные спецификации рецептов";
            this.chkPrescriptions.UseVisualStyleBackColor = true;
            // 
            // chkHistory
            // 
            this.chkHistory.AutoSize = true;
            this.chkHistory.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkHistory.Location = new System.Drawing.Point(47, 69);
            this.chkHistory.Name = "chkHistory";
            this.chkHistory.Size = new System.Drawing.Size(227, 24);
            this.chkHistory.TabIndex = 6;
            this.chkHistory.Text = "Включить историю приёмов";
            this.chkHistory.UseVisualStyleBackColor = true;
            // 
            // chkPersonalInfo
            // 
            this.chkPersonalInfo.AutoSize = true;
            this.chkPersonalInfo.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkPersonalInfo.Location = new System.Drawing.Point(47, 39);
            this.chkPersonalInfo.Name = "chkPersonalInfo";
            this.chkPersonalInfo.Size = new System.Drawing.Size(254, 24);
            this.chkPersonalInfo.TabIndex = 5;
            this.chkPersonalInfo.Text = "Включить личные данные врача";
            this.chkPersonalInfo.UseVisualStyleBackColor = true;
            // 
            // ExportFormEmployee
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 300);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkPrescriptions);
            this.Controls.Add(this.chkHistory);
            this.Controls.Add(this.chkPersonalInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportFormEmployee";
            this.Text = "Экспорт данных врача";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkPrescriptions;
        private System.Windows.Forms.CheckBox chkHistory;
        private System.Windows.Forms.CheckBox chkPersonalInfo;
    }
}