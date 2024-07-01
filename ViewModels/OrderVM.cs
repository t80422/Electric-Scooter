using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Electric_Scooter.Models
{
    public class OrderVM
    {
        public Orders Orders { get; set; }
        public string CusName { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public Points Points { get; set; }
    }
}