using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShadowTracker.Data;
using ShadowTracker.Extensions;
using ShadowTracker.Models;
using ShadowTracker.Models.Enums;
using ShadowTracker.Models.ViewModels;
using ShadowTracker.Services.Interfaces;

namespace ShadowTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly IBTTicketService _ticketService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTLookupService _lookupService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTFileService _fileService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTTicketHistoryService _ticketHistoryService;
        private readonly IBTNotificationService _notificationService;

        public TicketsController(
                                IBTTicketService ticketService,
                                UserManager<BTUser> userManager,
                                IBTLookupService lookupService,
                                IBTProjectService projectService,
                                IBTCompanyInfoService companyInfoService,
                                IBTFileService fileService,
                                IBTRolesService rolesService,
                                IBTTicketHistoryService ticketHistoryService,
                                IBTNotificationService notificationService)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _lookupService = lookupService;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _fileService = fileService;
            _rolesService = rolesService;
            _ticketHistoryService = ticketHistoryService;
            _notificationService = notificationService;
        }

        //GET: All Tickets
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            TicketFilterViewModel model = new();

            model.Tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
            model.TicketPrioritySelectList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.TicketTypeSelectList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            string userId = _userManager.GetUserId(User);

            if (User.IsInRole(nameof(BTRoles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(userId), "Id", "Name");
            }

            return View(model);
        }

        //GET: My Tickets
        public async Task<IActionResult> MyTickets()
        {
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId().Value;

            TicketFilterViewModel model = new();

            model.Tickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);
            model.TicketPrioritySelectList = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            model.TicketTypeSelectList = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            if (User.IsInRole(nameof(BTRoles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(userId), "Id", "Name");
            }

            return View(model);
        }

        //GET: Archived Tickets
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser user = await _userManager.GetUserAsync(User);
            List<Ticket> model = await _ticketService.GetArchivedTicketsAsync(companyId);
            ViewData["TicketPriority"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketType"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            if (User.IsInRole(nameof(BTRoles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(user.Id), "Id", "Name");
            }

            return View(model);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public async Task<IActionResult> ShowFile(int id)
        {
            TicketAttachment ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }

        //POST : Tickets/ AddTicketComment
        [HttpPost]
        public async Task<IActionResult> AddTicketComment([Bind("TicketId,Comment,Id")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);
                try
                {
                    //ticketComment.TicketId = ticketComment.TicketId;
                    ticketComment.Created = DateTime.Now;
                    ticketComment.UserId = userId;
                    await _ticketService.AddTicketCommentAsync(ticketComment);
                    await _ticketHistoryService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment), ticketComment.UserId);

                    return RedirectToAction(nameof(Details), new { id = ticketComment.TicketId });
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View(ticketComment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment([Bind("Id,FormFile,Description,TicketId,")] TicketAttachment ticketAttachment)
        {
            string statusMessage;

            if (ModelState.IsValid && ticketAttachment.FormFile != null)
            {
                ticketAttachment.FileData = await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.FormFile);
                ticketAttachment.FileName = ticketAttachment.FormFile.FileName;
                ticketAttachment.FileContentType = ticketAttachment.FormFile.ContentType;

                ticketAttachment.Created = DateTimeOffset.Now;
                ticketAttachment.UserId = _userManager.GetUserId(User);

                await _ticketService.AddTicketAttachmentAsync(ticketAttachment);
                statusMessage = "Success: New attachment added to Ticket.";
            }
            else
            {
                statusMessage = "Error: Invalid data.";
            }

            return RedirectToAction("Details", new { id = ticketAttachment.TicketId, message = statusMessage });
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            string userId = _userManager.GetUserId(User);
            //int projectId =
            int companyId = User.Identity.GetCompanyId().Value;

            if (User.IsInRole(nameof(BTRoles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(userId), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");

            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            string userId = _userManager.GetUserId(User);
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser btUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Created = DateTime.Now;
                    ticket.OwnerUserId = userId;
                    ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New))).Value;
                    await _ticketService.AddNewTicketAsync(ticket);
                    Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                    await _ticketHistoryService.AddHistoryAsync(null, newTicket, userId);

                    BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                    Notification notification = new()
                    {
                        TicketId = ticket.Id,
                        Title = "New Ticket Added",
                        Message = $"New Ticket: {ticket.Title}, was created by {btUser.FullName} for the Project : {newTicket.Project.Name}",
                        Created = DateTime.Now,
                        SenderId = userId,
                        RecipientId = projectManager?.Id,
                    };
                    await _notificationService.AddNotificationAsync(notification);
                    if (projectManager != null)
                    {
                        await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
                    }
                    else
                    {
                        await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, "Admin");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string userId = _userManager.GetUserId(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                try
                {
                    ticket.Updated = DateTime.Now;
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, userId);
                return RedirectToAction(nameof(AllTickets));
            }
            int companyId = User.Identity.GetCompanyId().Value;

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetTicketStatusesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        //GET: Assign Developer
        [HttpGet]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }
            AssignDeveloperViewModel model = new();
            model.Ticket = await _ticketService.GetTicketByIdAsync(ticketId.Value);
            model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, nameof(BTRoles.Developer)), "Id", "FullName");

            return View(model);
        }

        //POST: Assign Developer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);
            if (model.DeveloperId != null)
            {
                string userId = _userManager.GetUserId(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                }
                catch (Exception)
                {
                    throw;
                }

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, userId);

                Notification notification = new()
                {
                    TicketId = model.Ticket.Id,
                    NotificationTypeId = (await _lookupService.LookupNotificationTypeId(nameof(BTNotificationTypes.Ticket))).Value,
                    Title = "Ticket Assigned",
                    Message = $"Ticket : {model.Ticket.Title}, was assigned by {btUser.FullName}",
                    SenderId = userId,
                    RecipientId = model.Ticket.DeveloperUserId
                };
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
                return RedirectToAction(nameof(Details), new { id = model.Ticket.Id });
            }
            return RedirectToAction(nameof(AssignDeveloper), new { ticketId = model.Ticket.Id });
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = true;
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(AllTickets));
        }

        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}