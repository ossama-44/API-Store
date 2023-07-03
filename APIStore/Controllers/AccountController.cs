using APIStore.Dtos;
using APIStore.ResponseModule;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APIStore.Controllers
{

    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            //var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type  == ClaimTypes.Email)?.Value;
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await this.userManager.FindByEmailAsync(email);

            if (user == null) 
                return NotFound(new ApiResponse(404));

            return new UserDto 
            {
                Email = email,
                DispalyName = user.DisplayName,
                Token = this.tokenService.CreateToken(user)
            };
        }
        [HttpPost("Login")]

        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await this.userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Unauthorized(new ApiResponse(401));
            var result = await this.signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            return new UserDto
            {
                Email = user.Email,
                DispalyName = user.DisplayName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[]
                    {
                        "Email Address Already in Use !!"
                    }
                });
            }

            var user = new AppUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email,
            };

            var result = await this.userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            return new UserDto
            {
                Email = user.Email,
                DispalyName = user.DisplayName,
                Token = this.tokenService.CreateToken(user)
            };
        }

        [HttpGet("CheckEmail")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
            => await this.userManager.FindByEmailAsync(email) != null;

        [Authorize]
        [HttpGet("Address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await this.userManager.Users.Include(x => x.Address)
                                                    .SingleOrDefaultAsync(x => x.Email == email);

            var mappedAddress = this.mapper.Map<AddressDto>(user.Address);
            return Ok(mappedAddress);
        }
        [Authorize]
        [HttpPost("UpdataAddress")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await this.userManager.Users.Include(x => x.Address)
                                                    .SingleOrDefaultAsync(x => x.Email == email);

            user.Address = this.mapper.Map<Address>(addressDto);

            var result = await this.userManager.UpdateAsync(user);
            var mappedAddress = this.mapper.Map<AddressDto>(user.Address);

            if (result.Succeeded) 
            return Ok(mappedAddress);

            return BadRequest(new ApiResponse(400, "Problem Updating the User Address"));
        }
    }
}
