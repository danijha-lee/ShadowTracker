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
    }
}