using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShadowTracker.Models;
using ShadowTracker.Extensions;
using ShadowTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using ShadowTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace ShadowTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            IBTCompanyInfoService companyInfoService,
            IBTProjectService projectService,
            UserManager<BTUser> userManager)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
            _userManager = userManager;
        }

        public IActionResult Landing()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard(string swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;
            DashboardViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;
            model.Tickets = await _companyInfoService.GetTicketsAsync(companyId);
            model.Projects = await _companyInfoService.GetProjectsAsync(companyId);
            model.Members = await _companyInfoService.GetAllMembersAsync(companyId);
            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);

            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}