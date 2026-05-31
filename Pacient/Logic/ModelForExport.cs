using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POLYCLINIC_DB_CourseWork.Pacient.Logic
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Cabinet { get; set; }

        public List<HistoryDto> Histories { get; set; } = new List<HistoryDto>();
    }
    public class PatientDto
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string NumberPolicy { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<HistoryDto> Histories { get; set; } = new List<HistoryDto>();
    }

    public class HistoryDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Symptoms { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; } = new List<PrescriptionDto>();
        public List<DiagnosisDto> Diagnoses { get; set; } = new List<DiagnosisDto>();

        public int PatientId { get; set;  }
        public string PatientName { get; set; }
        public string PatientPolicy { get; set; }
    }

    public class PrescriptionDto
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string Instruction { get; set; }
        public List<MedicamentDto> Medicaments { get; set; } = new List<MedicamentDto>();
    }

    public class MedicamentDto
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public string SideEffects { get; set; }
    }

    public class DiagnosisDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
