using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.CRUD_Services
{
    public interface IQuestionService : IDataService<Question>
    {
        Task<Question> CreateNewQuestion(Question question, string testName, string option1, string option2, string option3, string option4);

        Task<IEnumerable<Question>> GetAllQuestionsForGivenTestName(string testName);
    }
}
