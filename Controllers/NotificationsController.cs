using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShadowTracker.Data;
using ShadowTracker.Models;
using ShadowTracker.Models.Enums;
using ShadowTracker.Services.Interfaces;
using ShadowTracker.Extensions;

namespace ShadowTracker.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTNotificationService _notificationService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTCompanyInfoService _infoService;

        public NotificationsController(ApplicationDbContext context,
                                        IBTNotificationService notificationService,
                                        IBTProjectService projectService,
                                        IBTTicketService ticketService,
                                        IBTLookupService lookupService,
                                        UserManager<BTUser> userManager,
                                        IBTCompanyInfoService infoService)
        {
            _context = context;
            _notificationService = notificationService;
            _projectService = projectService;
            _ticketService = ticketService;
            _lookupService = lookupService;
            _userManager = userManager;
            _infoService = infoService;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);
            List<Notification> notifications = await _notificationService.GetReceivedNotificationsAsync(userId);
            return View(notifications);
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.NotificationType)
                .Include(n => n.Project)
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public async Task<IActionResult> Create()
        {
            BTUser user = await _userManager.GetUserAsync(User);
            int companyId = User.Identity.GetCompanyId().Value;
            if (User.IsInRole(nameof(BTRoles.Admin)))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(user.Id), "Id", "Name");
            }
            //ViewData["NotificationTypeId"] = new SelectList(await _lookupService.LookupNotificationTypeId(), "Id", "Name");
            ViewData["RecipientId"] = new SelectList(await _infoService.GetAllMembersAsync(companyId), "Id", "FullName");
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description");
            return View();
        }

        //POST : Notifications/ SendNotificaiton
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNotification(string RecipientId, string Message, string Title)
        {
            Notification notification = new();
            notification.RecipientId = RecipientId;
            notification.Message = Message;
            notification.Title = Title;
            notification.Created = DateTime.Now;
            BTUser user = await _userManager.GetUserAsync(User);
            notification.SenderId = user.Id;
            notification.NotificationTypeId = 1;
            await _notificationService.AddNotificationAsync(notification);
            await _notificationService.SendEmailNotificationAsync(notification, Title);

            return RedirectToAction("Index", "Notifications");
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Message,NotificationTypeId,Viewed,TicketId,ProjectId,RecipientId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                BTUser user = await _userManager.GetUserAsync(User);
                notification.SenderId = user.Id;
                notification.Created = DateTime.Now;
                await _notificationService.AddNotificationAsync(notification);
                return RedirectToAction(nameof(Index));
            }
            ViewData["NotificationTypeId"] = new SelectList(_context.NotificationTypes, "Id", "Name", notification.NotificationTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", notification.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["NotificationTypeId"] = new SelectList(_context.NotificationTypes, "Id", "Name", notification.NotificationTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", notification.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Message,Created,NotificationTypeId,Viewed,TicketId,ProjectId,RecipientId,SenderId")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NotificationTypeId"] = new SelectList(_context.NotificationTypes, "Id", "Name", notification.NotificationTypeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", notification.ProjectId);
            ViewData["RecipientId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipientId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.NotificationType)
                .Include(n => n.Project)
                .Include(n => n.Recipient)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}