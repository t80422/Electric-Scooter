using Electric_Scooter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Electric_Scooter.Repositories
{
    public class ProductRep : Repository<Product>, IProductRep
    {
        public ProductRep(I_ChiEntities db) : base(db) { }

        public IEnumerable<string> GetColors()
        {
            return _dbSet.Select(x => x.p_Color)
                         .Distinct().ToList();
        }

        public IEnumerable<string> GetModels()
        {
            return _dbSet.Select(x => x.p_Model)
                         .Distinct().ToList();
        }

        public IEnumerable<string> GetNames()
        {
            return _dbSet.Select(x => x.p_Name)
                         .Distinct().ToList();
        }

        public IEnumerable<Product> GetProductsBySupplier(int supId,string type)
        {
            return _dbSet.Where(x => x.p_s_Id == supId && x.p_Type==type).ToList();
        }
    }
}