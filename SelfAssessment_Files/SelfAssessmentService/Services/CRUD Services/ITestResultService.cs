using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.CRUD_Services
{
    public interface ITestResultService : IDataService<TestResult>
    {
        Task<List<TestResult>> GetPersonalTestResults(Account currentAccount, string selectedSeries);
    }
}
