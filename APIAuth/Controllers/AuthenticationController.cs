using APIAuth.Authentication;
using APIAuth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIAuth.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;


        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterModel registerModel)
        {
            try
            {
                var isUserExist = await userManager.FindByNameAsync(registerModel.UserName);
                if (isUserExist != null)
                {
                    return Ok("User Already Exist");
                }
                ApplicationUser applicationUser = new ApplicationUser
                {
                    Email = registerModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerModel.UserName
                };
                var result = await userManager.CreateAsync(applicationUser, registerModel.Password);
                if (!result.Succeeded)
                {
                    return BadRequest("Register User Failed");
                }

                if (!await roleManager.RoleExistsAsync(RolesModel.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(RolesModel.Admin));

                }
                if (!await roleManager.RoleExistsAsync(RolesModel.Customer))
                {
                    await roleManager.CreateAsync(new IdentityRole(RolesModel.Customer));
                    

                }
                if (await roleManager.RoleExistsAsync(RolesModel.Admin))
                {
                    await userManager.AddToRoleAsync(applicationUser, RolesModel.Customer);
                }
                return Ok("User Created Successfully.");
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterModel registerModel)
        {
            try
            {
                var isUserExist = await userManager.FindByNameAsync(registerModel.UserName);
                if (isUserExist != null)
                {
                    return Ok("User Already Exist");
                }
                ApplicationUser applicationUser = new ApplicationUser
                {
                    Email = registerModel.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = registerModel.UserName
                };
                var result = await userManager.CreateAsync(applicationUser, registerModel.Password);
                if (!result.Succeeded)
                {
                    return BadRequest("Register User Failed");
                }

                if (!await roleManager.RoleExistsAsync(RolesModel.Admin))
                {
                    await roleManager.CreateAsync(new IdentityRole(RolesModel.Admin));

                }
                if (!await roleManager.RoleExistsAsync(RolesModel.Customer))
                {
                    await roleManager.CreateAsync(new IdentityRole(RolesModel.Customer));

                }
                if (await roleManager.RoleExistsAsync(RolesModel.Admin))
                {
                    await userManager.AddToRoleAsync(applicationUser, RolesModel.Customer);
                }
                return Ok("User Created Successfully.");
            }
            catch (System.Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var userdata = await userManager.FindByEmailAsync(loginModel.Email);
                if (userdata != null )
                {
                    var userRoles = await userManager.GetRolesAsync(userdata);
                    var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,userdata.Email),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                    };
                    foreach (var Role in userRoles)
                    {
                        Claims.Add(new Claim(ClaimTypes.Role, Role));
                    }
                    var SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                    var AccessToken = new JwtSecurityToken(
                        issuer: configuration["JWT:ValidIssuer"],
                        audience: configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(1),
                        claims: Claims,
                        signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
                        );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(AccessToken),
                        expirydate = AccessToken.ValidTo,
                        issuedate = DateTime.Now,
                        User = loginModel.Email
                    });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

    }
}
