using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShadowTracker.Models;

namespace ShadowTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {
        public Task<List<BTUser>> GetAllMembersAsync(int companyId);

        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        public Task<List<Project>> GetProjectsAsync(int? companyId);

        public Task<List<Ticket>> GetTicketsAsync(int? companyId);

        public Task<IQueryable<BTUser>> GetPagedMembersAsync(int companyId);
    }
}