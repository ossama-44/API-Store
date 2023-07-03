using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class ProductWithTypeAndBrandSpecifications : BaseSpecifications<Product>
    {
        public ProductWithTypeAndBrandSpecifications(ProductSpecParams productSpecParams) 
            : base(product =>
                  (string.IsNullOrEmpty(productSpecParams.Search) || product.Name.ToLower().Contains(productSpecParams.Search)) &&
                  (!productSpecParams.BrandId.HasValue || product.ProductBrandId == productSpecParams.BrandId) && 
                  (!productSpecParams.TypeId.HasValue || product.ProductTypeId == productSpecParams.TypeId )
                  )
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddOrderBy(product => product.Name);
            ApplyPaging(productSpecParams.PageSize * (productSpecParams.PageIndex - 1), productSpecParams.PageSize);

            if (!string.IsNullOrEmpty(productSpecParams.Sort))
            {
                switch (productSpecParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(product => product.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(product => product.Price);
                        break;
                    default:
                        AddOrderBy(product => product.Name);
                        break;
                }
            }
        }

        public ProductWithTypeAndBrandSpecifications(int id)
             : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }
    }
}
