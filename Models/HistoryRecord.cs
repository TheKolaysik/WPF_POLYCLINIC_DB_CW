using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POLYCLINIC_DB_CourseWork.Models
{
    public class HistoryRecord
    {
        public long ID_history { get; set; }
        public long ID_pacient { get; set; }
        public long ID_doctor { get; set; }
        public DateTime Date { get; set; }

    }
}
