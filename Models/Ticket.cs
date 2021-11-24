using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(2500)]
        [DisplayName("Description (max 2500 char")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created Date")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated Date")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }

        //=== Foreign Key ==//

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Ticket Type ID")]
        public int TicketTypeId { get; set; }

        [DisplayName("Ticket Priority ID")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Ticket Status ID")]
        public int TicketStatusId { get; set; }

        [DisplayName("Ticket Owner")]
        public string OwnerUserId { get; set; }

        [DisplayName("Ticket Developer")]
        public string DeveloperUserId { get; set; }

        //=== Navigation Properties ==//

        // One to One Relationship
        public virtual Project Project { get; set; }

        [DisplayName("Ticket Type")]
        public virtual TicketType TicketType { get; set; }

        [DisplayName("Ticket Priority")]
        public virtual TicketPriority TicketPriority { get; set; }

        [DisplayName("Ticket Status")]
        public virtual TicketStatus TicketStatus { get; set; }

        public virtual BTUser OwnerUser { get; set; }

        public virtual BTUser DeveloperUser { get; set; }

        // One to Many Relationship
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();

        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();

        public virtual ICollection<TicketTask> Tasks { get; set; } = new HashSet<TicketTask>();
    }
}