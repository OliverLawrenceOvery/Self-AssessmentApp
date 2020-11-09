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
    public class QuestionDataService : GenericDataService<Question>, IQuestionService
    {
        public async Task<Question> CreateNewQuestion(Question question, string testName, string option1, string option2, string option3, string option4)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                question.Test = context.Tests.Where(q => q.TestName == testName)
                    .FirstOrDefault();
                
                EntityEntry<Question> createdResult = await context.Set<Question>().AddAsync(question);
                await context.Set<QuestionOption>().AddAsync(new QuestionOption { OptionText = option1, Question = question });
                await context.Set<QuestionOption>().AddAsync(new QuestionOption { OptionText = option2, Question = question });
                await context.Set<QuestionOption>().AddAsync(new QuestionOption { OptionText = option3, Question = question });
                await context.Set<QuestionOption>().AddAsync(new QuestionOption { OptionText = option4, Question = question });
                await context.SaveChangesAsync();
                return createdResult.Entity;
            }
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsForGivenTestName(string testName)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                return await context.Questions.Include(q => q.Test).Where(t => t.Test.TestName == testName).ToListAsync();
            }
        }
    }
}
