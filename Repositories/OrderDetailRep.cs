using Electric_Scooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Electric_Scooter.Repositories
{
    public class OrderDetailRep : Repository<OrderDetail>, IOrderDetailRep
    {
        public OrderDetailRep(I_ChiEntities db) : base(db) { }

        public void DeleteByOrderId(int ordId)
        {
            var data = _dbSet.Where(x => x.od_o_Id == ordId).ToList();

            foreach (var detail in data)
            {
                _dbSet.Remove(detail);
            }
            _db.SaveChanges();
        }
    }
}