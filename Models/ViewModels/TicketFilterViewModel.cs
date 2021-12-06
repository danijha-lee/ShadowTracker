using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShadowTracker.Models.ViewModels
{
    public class TicketFilterViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public Ticket Ticket { get; set; }
        public SelectList TicketPrioritySelectList { get; set; }
        public SelectList TicketTypeSelectList { get; set; }
        public int PriorityId { get; set; }
        public int TypeId { get; set; }
    }
}