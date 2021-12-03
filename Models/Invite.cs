using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Sent ")]
        public DateTime InviteDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Join Date")]
        public DateTime JoinDate { get; set; }

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        public string InvitorId { get; set; }
        public string InviteeId { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("Invitee Email ")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name ")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Lase Name ")]
        public string InviteeLastName { get; set; }

        public bool IsValid { get; set; }

        [DisplayName("Invite Message")]
        public string Message { get; set; }

        //Navigation Properties
        public virtual Company Company { get; set; }

        public virtual BTUser Invitor { get; set; }
        public virtual BTUser Invitee { get; set; }
        public virtual Project Project { get; set; }
    }
}