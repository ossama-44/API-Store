using APIStore.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIStore.Controllers
{

    public class BasketController : BaseController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper )
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await this.basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto customerBasketDto)
        {
            var basket = this.mapper.Map<CustomerBasket>(customerBasketDto);

            var updateBasket = await this.basketRepository.UpdateBasketAsync(basket);

            return Ok(updateBasket);
        }

        [HttpDelete]
        public async Task DeleteBasketById(string id)
            => await this.basketRepository.DeleteBasketAsync(id);

    }
}
