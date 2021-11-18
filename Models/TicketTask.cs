using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class TicketTask
    {
        public int Id { get; set; }

        [DisplayName("Task Name")]
        public string Name { get; set; }

        [DisplayName("Task Description")]
        public string Description { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Task Status")]
        public int TaskStatusId { get; set; }

        [DisplayName("Task Type")]
        public int TaskTypeId { get; set; }

        //Navigation Properties
        public virtual Ticket Ticket { get; set; }

        public virtual TicketStatus TaskStatus { get; set; }
        public virtual TicketType TaskType { get; set; }
        public virtual ICollection<TicketAttachment> TaskAttachments { get; set; } = new HashSet<TicketAttachment>();
    }
}