﻿using System.Linq;
using System.Threading.Tasks;
using trifenix.agro.db.interfaces;
using trifenix.agro.db.interfaces.agro.ext;
using trifenix.agro.db.model.agro;

namespace trifenix.agro.db.applicationsReference.agro.ext {
    public class ProductRepository : IProductRepository {

        private readonly IMainDb<Product> _db;
        public ProductRepository(IMainDb<Product> db) {
            _db = db;
        }
        public async Task<string> CreateUpdateProduct(Product product) {
            return await _db.CreateUpdate(product);
        }

        public async Task<Product> GetProduct(string id) {
            return await _db.GetEntity(id);
        }

        public IQueryable<Product> GetProducts() {
            return _db.GetEntities();
        }

        public async Task<long> Total(string season) {
            return await _db.Store.QuerySingleAsync<long>($"SELECT value count(1) FROM c where c.SeasonId = '{season}'");
        }
    }
}
