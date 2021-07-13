using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string cause { get; set; }
        public string phone { get; set; }
        public string description { get; set; }
        public int? user_id { get; set; }
    }
}