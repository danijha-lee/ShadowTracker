using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShadowTracker.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public bool Deleted { get; set; }
        public BTUser OwnerUser { get; set; }
    }
}