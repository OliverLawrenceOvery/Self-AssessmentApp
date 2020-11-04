using Microsoft.EntityFrameworkCore;
using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_EntityFramework.CRUD_Services
{
    public class TestDataService : GenericDataService<Test>
    {
        public override async Task<Test> Get(int id)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Test entity = await context.Set<Test>()
                    .Include(t => t.Questions)
                    .ThenInclude(q => q.QuestionOptions)
                    .FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }
    }
}
