using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using SelfAssessmentService_Domain.Models;
using SelfAssessmentService_Domain.Services.CRUD_Services;
using SelfAssessmentService_EntityFramework;
using SelfAssessmentService_EntityFramework.CRUD_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class ResourceViewModel : BaseViewModel
    {
        public SelfAssessmentDbContext Context => new SelfAssessmentDbContext();
        public ResourceViewModel()
        {
            MainTopics = Context.MainTopics.ToList();
            CreateOrUpdate = "Create";
        }

        private string _createOrUpdate;
        public string CreateOrUpdate
        {
            get { return _createOrUpdate; }
            set { _createOrUpdate = value; OnPropertyChanged(nameof(CreateOrUpdate)); }
        }

        public IList<MainTopic> MainTopics { get; set; }

        private MainTopic _selectedMainTopic;
        public MainTopic SelectedMainTopic
        {
            get { return _selectedMainTopic; }
            set
            {
                _selectedMainTopic = value;
                SubTopics = Context.SubTopics
                    .Where(r => r.MainTopic.Id == SelectedMainTopic.Id)
                    .ToList();
                OnPropertyChanged(nameof(SubTopics));
            }
        }
        public IList<SubTopic> SubTopics { get; set; }

        private SubTopic _selectedSubTopic;
        public SubTopic SelectedSubTopic
        {
            get { return _selectedSubTopic; }
            set
            {
                _selectedSubTopic = value;
                if (SelectedSubTopic != null)
                {
                    TopicIntroduction = _selectedSubTopic.Introduction;
                    TopicContent = _selectedSubTopic.Content;
                    TopicSummary = _selectedSubTopic.Summary;
                    OnPropertyChanged(nameof(TopicIntroduction));
                    OnPropertyChanged(nameof(TopicContent));
                    OnPropertyChanged(nameof(TopicSummary));
                }
            }
        }

        public string TopicIntroduction { get; set; }
        public string TopicContent { get; set; }
        public string TopicSummary { get; set; }



        private string _createdSubTopicTitle;
        public string CreatedSubTopicTitle
        {
            get { return _createdSubTopicTitle; }
            set { _createdSubTopicTitle = value; OnPropertyChanged(nameof(CreatedSubTopicTitle)); }
        }

        private string _createdSubTopicIntro;
        public string CreatedSubTopicIntro
        {
            get { return _createdSubTopicIntro; }
            set { _createdSubTopicIntro = value; OnPropertyChanged(nameof(CreatedSubTopicIntro)); }
        }

        private string _createdSubTopicContent;
        public string CreatedSubTopicContent
        {
            get { return _createdSubTopicContent; }
            set { _createdSubTopicContent = value; OnPropertyChanged(nameof(CreatedSubTopicContent)); }
        }

        private string _createdSubTopicSummary;
        public string CreatedSubTopicSummary
        {
            get { return _createdSubTopicSummary; }
            set { _createdSubTopicSummary = value; OnPropertyChanged(nameof(CreatedSubTopicSummary)); }
        }

        public ICommand CreateOrUpdateSubTopic => new DelegateCommand<object>(FuncToCall);
        private async void FuncToCall(object context)
        {
            if (CreateOrUpdate == "Create")
            {
                ISubTopicService service = new SubTopicDataService();
                await service.CreateNewSubTopic(SelectedMainTopic.Title, CreatedSubTopicTitle, CreatedSubTopicIntro, CreatedSubTopicSummary, CreatedSubTopicContent);
                SubTopics = Context.SubTopics
                        .Where(r => r.MainTopic.Id == SelectedMainTopic.Id)
                        .ToList();
                OnPropertyChanged(nameof(SubTopics));
            }
            else if (CreateOrUpdate == "Update")
            {
                ISubTopicService service = new SubTopicDataService();
                await service.UpdateSubTopic(SelectedSubTopic.Id, CreatedSubTopicTitle, CreatedSubTopicIntro, CreatedSubTopicContent, CreatedSubTopicSummary);
                OnPropertyChanged(nameof(CreatedSubTopicTitle));
                OnPropertyChanged(nameof(CreatedSubTopicIntro));
                OnPropertyChanged(nameof(CreatedSubTopicContent));
                OnPropertyChanged(nameof(CreatedSubTopicSummary));
            }
        }


        public ICommand UpdateCommand => new DelegateCommand<object>(FuncToCall2);
        private void FuncToCall2(object context)
        {
            CreatedSubTopicTitle = SelectedSubTopic.Title;
            CreatedSubTopicIntro = TopicIntroduction;
            CreatedSubTopicContent = TopicContent;
            CreatedSubTopicSummary = TopicSummary;

            CreateOrUpdate = "Update";
        }


        public ICommand DeleteCommand => new DelegateCommand<object>(FuncToCall3);
        private void FuncToCall3(object context)
        {
            using (SelfAssessmentDbContext newContext = new SelfAssessmentDbContext())
            {
                SubTopic retrievedSubTopic = newContext.SubTopics.Where(st => st.Id == SelectedSubTopic.Id).FirstOrDefault();
                newContext.SubTopics.Remove(retrievedSubTopic);
                newContext.SaveChanges();
            }
            SubTopics = Context.SubTopics.Include(m => m.MainTopic)
                    .Where(r => r.MainTopic.Id == SelectedMainTopic.Id)
                    .ToList();
            SelectedSubTopic = null;
            TopicIntroduction = "";
            TopicContent = "";
            TopicSummary = "";
            OnPropertyChanged(nameof(SubTopics));
        }

    }
}
