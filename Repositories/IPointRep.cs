using Electric_Scooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electric_Scooter.Repositories
{
    interface IPointRep:IRepository<Points>
    {
        Points GetDataByOrderId(int ordId);
    }
}
