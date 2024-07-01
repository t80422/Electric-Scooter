using Electric_Scooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Electric_Scooter.Repositories
{
    public class PointRep : Repository<Points>, IPointRep
    {
        public PointRep(I_ChiEntities db) : base(db) { }

        public Points GetDataByOrderId(int ordId)
        {
            return _dbSet.FirstOrDefault(x => x.po_o_Id == ordId);
        }
    }
}