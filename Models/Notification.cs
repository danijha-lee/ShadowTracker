using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DisplayName("Had Been Viewed ")]
        public bool Viewed { get; set; }

        [DisplayName("Ticket ")]
        public int? TicketId { get; set; }

        [DisplayName("Project ")]
        public int? ProjectId { get; set; }

        [DisplayName("Recipient")]
        public int? ReciepientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public int? SenderId { get; set; }

        //Navigation Properties
        public virtual NotificationType NotificationType { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual Project Project { get; set; }
        public virtual BTUser Recipient { get; set; }
        public virtual BTUser Sender { get; set; }
    }
}