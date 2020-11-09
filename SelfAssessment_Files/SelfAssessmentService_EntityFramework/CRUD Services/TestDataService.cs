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
    public class TestDataService : GenericDataService<Test>, ITestService
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

        public async Task<Test> CreateNewTest(Test test, string seriesName)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                Test checkTest = context.Tests.Where(t => t.TestName == test.TestName).FirstOrDefault();
                if (checkTest == null)
                {
                    test.TestSeries = context.TestSeries
                        .Where(q => q.TestSeriesName == seriesName)
                        .FirstOrDefault();
                    EntityEntry<Test> createdResult = await context.Set<Test>().AddAsync(test);
                    await context.SaveChangesAsync();
                    return createdResult.Entity;
                }
                return null;
            }
        }

    }
}
