using Microsoft.EntityFrameworkCore;
using SelfAssessmentService_Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SelfAssessmentService_EntityFramework
{
    public class SelfAssessmentDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<MainTopic> MainTopics { get; set; }
        public DbSet<SubTopic> SubTopics { get; set; }
        public DbSet<TestSeries> TestSeries { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SelfAssessmentDb;Trusted_Connection=True");
        }
    }
}
