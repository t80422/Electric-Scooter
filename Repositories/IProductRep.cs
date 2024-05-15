using Electric_Scooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electric_Scooter.Repositories
{
    interface IProductRep:IRepository<Product>
    {
        IEnumerable<string> GetNames();
        IEnumerable<string> GetModels();
        IEnumerable<string> GetColors();
        IEnumerable<Product> GetProductsBySupplier(int supId,string type);
    }
}
