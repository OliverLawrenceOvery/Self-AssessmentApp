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
using System.Windows;
using System.Windows.Input;

namespace SelfAssessmentService_WPF.ViewModels
{
    public class ResourceViewModel : BaseViewModel
    {
        public SelfAssessmentDbContext Context => new SelfAssessmentDbContext();

        private IList<MainTopic> _mainTopics;
        public IList<MainTopic> MainTopics
        {
            get { return _mainTopics; }
            set { _mainTopics = value; OnPropertyChanged(nameof(MainTopics)); }
        }


        private string _createOrUpdate;
        public string CreateOrUpdate
        {
            get { return _createOrUpdate; }
            set { _createOrUpdate = value; OnPropertyChanged(nameof(CreateOrUpdate)); }
        }
        public ResourceViewModel()
        {
            CreateOrUpdate = "Create";
            MainTopics = Context.MainTopics.ToList();
        }


        private MainTopic _selectedMainTopic;
        public MainTopic SelectedMainTopic
        {
            get { return _selectedMainTopic; }
            set
            {
                _selectedMainTopic = value;
                if (SelectedMainTopic != null)
                {
                    SubTopics = Context.SubTopics
                         .Where(r => r.MainTopic.Id == SelectedMainTopic.Id)
                         .ToList();
                    CreatedMainTopicTitle = value.Title;
                }
            }
        }

        private IList<SubTopic> _subTopics;
        public IList<SubTopic> SubTopics
        {
            get { return _subTopics; }
            set { _subTopics = value; OnPropertyChanged(nameof(SubTopics)); }
        }
        public string TopicIntroduction { get; set; }
        public string TopicContent { get; set; }
        public string TopicSummary { get; set; }

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
                }
                else
                {
                    TopicIntroduction = "";
                    TopicContent = "";
                    TopicSummary = "";
                }
                OnPropertyChanged(nameof(TopicIntroduction));
                OnPropertyChanged(nameof(TopicContent));
                OnPropertyChanged(nameof(TopicSummary));
            }
        }

        private string _createdMainTopicTitle;
        public string CreatedMainTopicTitle
        {
            get { return _createdMainTopicTitle; }
            set { _createdMainTopicTitle = value; OnPropertyChanged(nameof(CreatedMainTopicTitle)); }
        }

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



        public ICommand CreateMainTopic => new DelegateCommand<object>(FuncToCall4);
        private void FuncToCall4(object context)
        {
            if (CreatedMainTopicTitle == "") { MessageBox.Show("You must enter a name in order to create a main topic."); }
            else
            {
                using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
                {
                    MainTopic checkMainTopic = db.MainTopics.Where(m => m.Title == CreatedMainTopicTitle).FirstOrDefault();
                    if (checkMainTopic == null)
                    {
                        db.MainTopics.Add(new MainTopic() { Title = CreatedMainTopicTitle });
                        db.SaveChanges();
                    }
                    else { MessageBox.Show("This main topic already exists!"); }
                }
                MainTopics = Context.MainTopics.ToList();
                CreatedMainTopicTitle = "";
            }
        }

        public ICommand UpdateMainTopic => new DelegateCommand<object>(FuncToCall5);
        private void FuncToCall5(object context)
        {
            if (CreatedMainTopicTitle == "") { MessageBox.Show("You must enter a valid name in order to update a main topic."); }
            else
            {
                using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
                {
                    MainTopic mainTopic = db.MainTopics.Where(m => m.Title == SelectedMainTopic.Title).FirstOrDefault();
                    mainTopic.Title = CreatedMainTopicTitle;
                    db.Set<MainTopic>().Update(mainTopic);
                    db.SaveChanges();
                }
                MainTopics = Context.MainTopics.ToList();
                CreatedMainTopicTitle = "";
            }
        }

        public ICommand DeleteMainTopic => new DelegateCommand<object>(FuncToCall6);
        private void FuncToCall6(object context)
        {
            if (CreatedMainTopicTitle == "") { MessageBox.Show("You must enter a valid name in order to delete a main topic."); }
            else
            {
                using (SelfAssessmentDbContext db = new SelfAssessmentDbContext())
                {
                    List<SubTopic> subTopics = db.SubTopics
                        .Include(s => s.MainTopic)
                        .Where(m => m.MainTopic.Title == SelectedMainTopic.Title)
                        .ToList();
                    foreach (SubTopic subTopic in subTopics)
                    {
                        db.SubTopics.Remove(subTopic);
                    }
                    MainTopic mainTopic = db.MainTopics.Where(m => m.Title == SelectedMainTopic.Title).FirstOrDefault();
                    db.Set<MainTopic>().Remove(mainTopic);
                    db.SaveChanges();
                }
                SelectedMainTopic = null;
                MainTopics = Context.MainTopics.ToList();
                CreatedMainTopicTitle = "";
                SubTopics = null;
            }
        }


        public ICommand CreateOrUpdateSubTopic => new DelegateCommand<object>(FuncToCall);
        private async void FuncToCall(object context)
        {
            if(CreatedSubTopicTitle == "") { MessageBox.Show("You must define a title for this sub-topic."); }
            else if (CreateOrUpdate == "Create")
            {
                ISubTopicService service = new SubTopicDataService();
                await service.CreateNewSubTopic(SelectedMainTopic.Title, CreatedSubTopicTitle, CreatedSubTopicIntro, CreatedSubTopicSummary, CreatedSubTopicContent);
                SubTopics = Context.SubTopics
                        .Where(r => r.MainTopic.Id == SelectedMainTopic.Id)
                        .ToList();
            }
            else if (CreateOrUpdate == "Update")
            {
                ISubTopicService service = new SubTopicDataService();
                SubTopic updatedSubTopic = await service.UpdateSubTopic(SelectedSubTopic.Id, CreatedSubTopicTitle, CreatedSubTopicIntro, CreatedSubTopicContent, CreatedSubTopicSummary);
                SelectedSubTopic = updatedSubTopic;
                CreateOrUpdate = "Create";
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
        }

    }
}
