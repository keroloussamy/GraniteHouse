using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models
{
    public class Appointments
    {
        public int Id { get; set; }
        [NotMapped]//will not add to database but apper in Views
        public DateTime AppointmentTime { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsConfirmed { get; set; }

    }
}
