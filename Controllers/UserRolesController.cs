using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShadowTracker.Models;
using ShadowTracker.Models.ViewModels;
using ShadowTracker.Services.Interfaces;

namespace ShadowTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller

    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<BTUser> _userManager;

        public UserRolesController(IBTCompanyInfoService companyInfoService,
                                                IBTRolesService rolesService,
                                                UserManager<BTUser> userManager)
        {
            _companyInfoService = companyInfoService;
            _rolesService = rolesService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new ManageUserRolesViewModel();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);
                model.Add(viewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.BTUser.Id);

            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                if (await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}