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
    public class TestResultService : GenericDataService<TestResult>, ITestResultService
    {
        public async Task<TestResult> CreatePersonalTestResult(int id, string testName, TestResult testResult)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                TestResult testResultCheck = context.TestResults.Where(t => t.Test.TestName == testName).FirstOrDefault(); //return already existing test result for that test
                if (testResultCheck == null)
                {
                    Account account = context.Accounts.Where(a => a.Id == id).FirstOrDefault();
                    Test test = context.Tests.Where(e => e.TestName == testName).FirstOrDefault();
                    testResult.Account = account;
                    testResult.Test = test;
                    EntityEntry<TestResult> createdResult = await context.Set<TestResult>().AddAsync(testResult);
                    await context.SaveChangesAsync();
                    return createdResult.Entity;
                }
                else
                {
                    if(testResultCheck.Mark < testResult.Mark)  //if user scores a higher mark
                    {
                        testResultCheck.Mark = testResult.Mark;
                        EntityEntry<TestResult> createdResult = await context.Set<TestResult>().AddAsync(testResultCheck);
                        await context.SaveChangesAsync();
                        return createdResult.Entity;
                    }
                }
            }
        }

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
