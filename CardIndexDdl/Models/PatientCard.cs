using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardIndexDal.Models
{
   public class PatientCard
   {
      [Required]
      public int Id { get; set; }
      [Required]
      public string Fio { get; set; }
      [Required]
      public Gender Gender { get; set; }
      [Required]
      public DateTime DateOfBirth { get; set; }
      public string Address { get; set; }
      public string Phone { get; set; }

      public virtual List<VisitCard> Visits { get; set; }
   }

   public enum Gender
   {
      Male,
      Female,
      Other,
      Unknown,
   }
}
