﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class TicketPriority
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; }
    }
}