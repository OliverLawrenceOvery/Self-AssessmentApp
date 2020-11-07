using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SelfAssessmentService_Domain.Services.CRUD_Services
{
    public interface ISubTopicService : IDataService<SubTopic>
    {
        Task<SubTopic> CreateNewSubTopic(string mainTopicTitle, string subTopicTitle, string subTopicIntro, string subTopicContent, string subTopicSummary);
        Task<SubTopic> UpdateSubTopic(int mainTopicTitle, string subTopicTitle, string subTopicIntro, string subTopicContent, string subTopicSummary);
    }
}
