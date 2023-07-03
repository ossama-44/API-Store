using APIStore.Dtos;
using APIStore.Helpers;
using APIStore.ResponseModule;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIStore.Controllers
{

    public class ProductController : BaseController
    {
        private readonly IGenericRepository<Product> productRepository;
        private readonly IGenericRepository<ProductBrand> productBrandRepository;
        private readonly IGenericRepository<ProductType> productTypeRepository;
        private readonly IMapper mapper;

        //private readonly IProductRepository productRepository;

        public ProductController(/*IProductRepository productRepository*/
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductBrand> productBrandRepository,
            IGenericRepository<ProductType> productTypeRepository,
            IMapper mapper)
        {
            this.productRepository = productRepository;
            this.productBrandRepository = productBrandRepository;
            this.productTypeRepository = productTypeRepository;
            this.mapper = mapper;
            //this.productRepository = productRepository;
        }

        [HttpGet("GetProducts")]
        public async Task<ActionResult<Pagination<ProductDto>>> GetProducts([FromQuery]ProductSpecParams productSpec)
        {
            var spec = new ProductWithTypeAndBrandSpecifications(productSpec);

            var countSpec = new ProductWithFiltersForCountSpecifications(productSpec);

            var totalItems = await this.productRepository.CountAsync(countSpec);

            var products = await this.productRepository.ListAsync(spec);

            var mappedProducts = this.mapper.Map<IReadOnlyList<ProductDto>>(products);

            var paginationData = new Pagination<ProductDto>(productSpec.PageIndex, productSpec.PageSize, totalItems, mappedProducts);
             
            return Ok(paginationData);
        } 
        
        [HttpGet("GetProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductWithTypeAndBrandSpecifications(id);

            var product = await this.productRepository.GetEntityWithSpecifications(spec);

            if (product == null) 
                return NotFound(new ApiResponse(404));

            var mappedProduct = this.mapper.Map<ProductDto>(product);

            return mappedProduct;
        }

        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProductBrands()
        {
            return Ok(await this.productBrandRepository.ListAllAsync());
        }
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProductTypes()
            => Ok(await this.productTypeRepository.ListAllAsync());
        

    }
}
