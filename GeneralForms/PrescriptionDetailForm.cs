using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WPF_POLYCLINIC_DB_CourseWork.Employee.Logic;
using WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic;

namespace WPF_POLYCLINIC_DB_CourseWork.Pacient.UI
{
    public partial class PrescriptionDetailForm : Form
    {
        private bool _editable = false;
        PacientReport pacientReport = new PacientReport();
        public PrescriptionDetailForm(Boolean editable, long id)
        {
            InitializeComponent();
            _editable = editable;
            pacientReport.LoadPrescriptionDetails(id, textBox1, textBox2, textBox3, textBox4, dataGridView1);

        }

        private void PrescriptionDetailForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
