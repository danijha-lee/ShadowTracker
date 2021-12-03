using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShadowTracker.Models.Enums;

namespace ShadowTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [Required]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [DisplayName(" Description")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile ImageFormFile { get; set; }

        [DisplayName(" File Name")]
        public string ImageFileName { get; set; }

        [DisplayName(" Project Image")]
        public byte[] ImageFileData { get; set; }

        [DisplayName(" File Extension")]
        public string ImageContentType { get; set; }

        [DisplayName(" Archived")]
        public bool Archived { get; set; }

        //Navigation Properties
        public virtual Company Company { get; set; }

        public int ProjectPriorityId { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}