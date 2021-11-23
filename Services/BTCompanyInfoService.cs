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
    }
}