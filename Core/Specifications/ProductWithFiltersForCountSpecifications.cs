using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductWithFiltersForCountSpecifications : BaseSpecifications<Product>
    {
        public ProductWithFiltersForCountSpecifications(ProductSpecParams productSpecParams)
    : base(product => 
          (string.IsNullOrEmpty(productSpecParams.Search) || product.Name.ToLower().Contains(productSpecParams.Search)) &&
          (!productSpecParams.BrandId.HasValue || product.ProductBrandId == productSpecParams.BrandId) &&
          (!productSpecParams.TypeId.HasValue || product.ProductTypeId == productSpecParams.TypeId)
          )
        {

        }
    }
}
