using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Models
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1,25,ErrorMessage = "Enter a correct number of days")]
        [Display(Name = "Defaul Number of Days")]
        public int DefaultDays { get; set; }

        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }

    }
}
