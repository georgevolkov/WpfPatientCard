using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardIndexDal.Models
{
    public class VisitCard
    {
        [Required] public int Id { get; set; }

        [Required] public DateTime VisitDate { get; set; }

        [Required] public VisitType VisitType { get; set; }

        [Required] public string Diagnosis { get; set; }

        [ForeignKey("PatientCardId")] public virtual PatientCard PatientCard { get; set; }
    }

    public enum VisitType
    {
        First,
        Second,
    }
}
