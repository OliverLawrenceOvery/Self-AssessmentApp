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
    public class SubTopicDataService : GenericDataService<SubTopic>, ISubTopicService
    {
        public async Task<SubTopic> CreateNewSubTopic(string mainTopicTitle, string subTopicTitle, string subTopicIntro, string subTopicContent, string subTopicSummary)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                MainTopic mainTopic = context.MainTopics.Where(m => m.Title == mainTopicTitle).FirstOrDefault();
                SubTopic newSubTopic = new SubTopic()
                {
                    Title = subTopicTitle,
                    Introduction = subTopicIntro,
                    Content = subTopicContent,
                    Summary = subTopicSummary,
                    MainTopic = mainTopic
                };
                EntityEntry<SubTopic> createdResult = await context.Set<SubTopic>().AddAsync(newSubTopic);
                await context.SaveChangesAsync();
                return createdResult.Entity;
            }
        }

        public async Task<SubTopic> UpdateSubTopic(int subTopicId, string subTopicTitle, string subTopicIntro, string subTopicContent, string subTopicSummary)
        {
            using (SelfAssessmentDbContext context = new SelfAssessmentDbContext())
            {
                SubTopic retrievedSubTopic = context.SubTopics.Where(st => st.Id == subTopicId).FirstOrDefault();
                retrievedSubTopic.Title = subTopicTitle;
                retrievedSubTopic.Introduction = subTopicIntro;
                retrievedSubTopic.Content = subTopicContent;
                retrievedSubTopic.Summary = subTopicSummary;

                context.Set<SubTopic>().Update(retrievedSubTopic);
                await context.SaveChangesAsync();

                return retrievedSubTopic;
            }
        }
    }
}
