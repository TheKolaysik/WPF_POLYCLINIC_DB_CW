using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POLYCLINIC_DB_CourseWork.Models
{
    public class Doctor
    {
        public long ID_doctor { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public int? Cabinet { get; set; }

        public int? Experience { get; set; }

        public string Phone { get; set; }

        public string SpecializationName { get; set; }

        public bool? Working { get; set; }

        public bool? Vacation { get; set; }

        public string Login { get; set; }

        public byte[] Password { get; set; }
    }
}
