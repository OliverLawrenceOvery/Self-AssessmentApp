using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_EntityFramework.CRUD_Services
{
    public class AccountDataService : GenericDataService<Account>, IAccountService
    {
        public override async Task<Account> Get(int id)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Account entity = await context.Accounts
                    .Include(a => a.User)
                    .FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }
       
        public override async Task<IEnumerable<Account>> GetAll()
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                IEnumerable<Account> entities = await context.Accounts
                    .Include(a => a.User)
                    .ToListAsync();
                return entities;
            }
        }

        public async Task<Account> GetByUsername(string username)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                return await context.Accounts
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.User.Username == username);
            }
        }
    }
}
