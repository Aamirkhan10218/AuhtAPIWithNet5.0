using APIAuth.Authentication;
using APIAuth.Data;
using APIAuth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace APIAuth.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly AppDBContext appDBContext;


        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, AppDBContext appDBContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;
            this.appDBContext = appDBContext;
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> UpdateProfile(RegisterModel registerModel)
        {
            try
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = registerModel.UserName,
                    Email = registerModel.Email
                };
                await userManager.UpdateAsync(user);
                return Ok("Updated");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = RolesModel.Admin)]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var data = userManager.Users.ToList();
                if (data != null && data.Count > 0)
                {
                    return Ok(data);
                }
                return BadRequest("No data Found");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
