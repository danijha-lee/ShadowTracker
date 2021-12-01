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
    public class BTLookupService : IBTLookupService
    {
        #region Properties

        private readonly ApplicationDbContext _context;

        #endregion Properties

        #region Constructor

        public BTLookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion Constructor

        #region Get Project Priorities

        public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
        {
            try
            {
                return await _context.ProjectPriorities.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Get Project Priorities

        #region Get Ticket Priorities

        public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
        {
            try
            {
                return await _context.TicketPriorities.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Priorities

        #region Get Ticket Statuses

        public async Task<List<TicketStatus>> GetTicketStatusesAsync()
        {
            try
            {
                return await _context.TicketStatuses.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Statuses

        #region Get Ticket Types

        public async Task<List<TicketType>> GetTicketTypesAsync()
        {
            try
            {
                return await _context.TicketTypes.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Get Ticket Types

    }
}