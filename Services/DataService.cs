using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheBlogFinalMVC.Data;
using TheBlogFinalMVC.Enums;
using TheBlogFinalMVC.Models;

namespace TheBlogFinalMVC.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            //Task: Create new database migration programatically
            await _dbContext.Database.MigrateAsync();

            //Task 1: Seeding a few Roles into the system
            await SeedRolesAsync();

            //Task 2: Seed a few users into the system
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //If there are already Roles in the system, do nothing
            if (_dbContext.Roles.Any()) return;

            //Otherwise we want to create a few roles
            foreach(var role in Enum.GetNames(typeof(BlogRole)))
            {
                //I need to use the Role Manager to create roles
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

        }

        private async Task SeedUsersAsync()
        {
            //If there are already Users in the system, do nothing
            if (_dbContext.Users.Any()) return;

            //Step 1: Otherwise create a new instance of BogUser
            var adminUser = new BlogUser()
            {
                Email = "kaylor33@hotmail.co.uk",
                FirstName = "Charlie",
                LastName = "Kaylor",
                UserName = "kaylor33@hotmail.co.uk",
                DisplayName = "Administrator",
                PhoneNumber = "07761942253",
                EmailConfirmed = true
            };

            //Step 2: Use the UserManager to create a new user that is defined by adminUser
            await _userManager.CreateAsync(adminUser, "Abc&123!");

            //Step 3: Add this new user to the Administrator Role
            await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            //Step 1 repeat: Create the moderator user
            var modUser = new BlogUser()
            {
                UserName = "agne1013@gmail.com",
                Email = "agne1013@gmail.com",
                FirstName = "Agne",
                LastName = "Prokopaviciute",
                DisplayName = "Moderator",
                PhoneNumber = "07871964027",
                EmailConfirmed = true
            };

            //Step 2 repeat: Use the UserManager to create a new user that is defined by modUser
            await _userManager.CreateAsync(modUser, "Abc&123!");

            //Step 3 repeat: Add this new user to the Moderator Role
            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }

    }
}
