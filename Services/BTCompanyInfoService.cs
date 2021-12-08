using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShadowTracker.Data;
using ShadowTracker.Models;
using ShadowTracker.Services.Interfaces;

namespace ShadowTracker.Services
{
    public class BTCompanyInfoService : IBTCompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public BTCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BTUser>> GetAllMembersAsync(int companyId)
        {
            List<BTUser> result = new();
            try
            {
                result = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company result = new();
            try
            {
                if (companyId != null)
                {
                    result = await _context.Companies
                                            .Include(c => c.Members)
                                            .Include(c => c.Invites)
                                            .Include(c => c.Projects)
                                           .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetProjectsAsync(int? companyId)
        {
            List<Project> result = new();
            try
            {
                if (companyId != null)
                {
                    result = await _context.Projects.Where(p => p.CompanyId == companyId)
                                            .Include(p => p.Members)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Comments)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Attachments)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.History)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Notifications)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.OwnerUser)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketPriority)
                                          .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                           .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                           .Include(p => p.ProjectPriority)
                                            .ToListAsync();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsAsync(int? companyId)
        {
            List<Ticket> result = new();

            try
            {
                if (companyId != null)
                {
                    result = await _context.Tickets.Where(t => t.Project.CompanyId == companyId)
                        .Include(t => t.Project)
                        .ToListAsync();
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}