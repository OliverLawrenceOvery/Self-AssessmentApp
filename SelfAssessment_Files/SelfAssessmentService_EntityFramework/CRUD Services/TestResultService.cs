using Microsoft.EntityFrameworkCore;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_EntityFramework.CRUD_Services
{
    public class TestResultService : GenericDataService<TestResult>, ITestResultService
    {
        public async Task<List<TestResult>> GetPersonalTestResults(Account currentAccount, string selectedSeries)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                TestSeries series = context.TestSeries
                    .Where(s => s.TestSeriesName == selectedSeries).FirstOrDefault();
                return await context.TestResults
                    .Include(q => q.Test)
                    .ThenInclude(qr => qr.TestSeries)
                    .Where(r => r.Account.Id == currentAccount.Id)
                    .Where(e => e.Test.TestSeries.Id == series.Id).ToListAsync();
            }
        }
    }
}
