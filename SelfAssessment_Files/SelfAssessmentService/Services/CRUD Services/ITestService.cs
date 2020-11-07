using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.CRUD_Services
{
    public interface ITestService : IDataService<Test>
    {
        Task<Test> CreateNewTest(Test test, string seriesName);
    }
}
