using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POLYCLINIC_DB_CourseWork.Models
{
    public class Patient
    {
        public long ID_pacient { get; set; }

        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string PlaceOfLiving { get; set; }
        public string Gender { get; set; }
        public string NumberPolicy { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public long? DistrictDoctor { get; set; }

        public string Login { get; set; }

        public byte[] PasswordHash { get; set; }

        public string Phone { get; set; }
    }
}
