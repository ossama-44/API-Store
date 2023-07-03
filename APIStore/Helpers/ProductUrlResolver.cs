using APIStore.Dtos;
using AutoMapper;
using Core.Entities;

namespace APIStore.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration configuration;

        public ProductUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PictureUrl))
                return this.configuration["ApiUrl"] + source.PictureUrl;

            return null;
        }
    }
}
