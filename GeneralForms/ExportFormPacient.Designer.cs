namespace WPF_POLYCLINIC_DB_CourseWork.GeneralForms
{
    partial class ExportFormPacient
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
            this.chkPersonalInfo = new System.Windows.Forms.CheckBox();
            this.chkHistory = new System.Windows.Forms.CheckBox();
            this.chkPrescriptions = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkPersonalInfo
            // 
            this.chkPersonalInfo.AutoSize = true;
            this.chkPersonalInfo.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkPersonalInfo.Location = new System.Drawing.Point(65, 48);
            this.chkPersonalInfo.Name = "chkPersonalInfo";
            this.chkPersonalInfo.Size = new System.Drawing.Size(279, 24);
            this.chkPersonalInfo.TabIndex = 0;
            this.chkPersonalInfo.Text = "Включить личные данные пациента";
            this.chkPersonalInfo.UseVisualStyleBackColor = true;
            this.chkPersonalInfo.CheckedChanged += new System.EventHandler(this.chkPersonalInfo_CheckedChanged);
            // 
            // chkHistory
            // 
            this.chkHistory.AutoSize = true;
            this.chkHistory.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkHistory.Location = new System.Drawing.Point(65, 78);
            this.chkHistory.Name = "chkHistory";
            this.chkHistory.Size = new System.Drawing.Size(227, 24);
            this.chkHistory.TabIndex = 1;
            this.chkHistory.Text = "Включить историю приёмов";
            this.chkHistory.UseVisualStyleBackColor = true;
            this.chkHistory.CheckedChanged += new System.EventHandler(this.chkHistory_CheckedChanged);
            // 
            // chkPrescriptions
            // 
            this.chkPrescriptions.AutoSize = true;
            this.chkPrescriptions.Font = new System.Drawing.Font("Segoe UI", 9.818182F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chkPrescriptions.Location = new System.Drawing.Point(65, 108);
            this.chkPrescriptions.Name = "chkPrescriptions";
            this.chkPrescriptions.Size = new System.Drawing.Size(357, 24);
            this.chkPrescriptions.TabIndex = 2;
            this.chkPrescriptions.Text = "Включить подробные спецификации рецептов";
            this.chkPrescriptions.UseVisualStyleBackColor = true;
            this.chkPrescriptions.CheckedChanged += new System.EventHandler(this.chkPrescriptions_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(65, 224);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 43);
            this.button1.TabIndex = 3;
            this.button1.Text = "Экспорт в Word";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnExportWord_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(269, 224);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(153, 43);
            this.button2.TabIndex = 4;
            this.button2.Text = "Экспорт в Excel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // ExportFormPacient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 297);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkPrescriptions);
            this.Controls.Add(this.chkHistory);
            this.Controls.Add(this.chkPersonalInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportFormPacient";
            this.Text = "Экспорт карты пациента";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkPersonalInfo;
        private System.Windows.Forms.CheckBox chkHistory;
        private System.Windows.Forms.CheckBox chkPrescriptions;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}