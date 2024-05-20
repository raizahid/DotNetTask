using DotNetTask.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace DotNetTask.Models
{
    public class DotNetTaskDbContext : DbContext
    {
        public DotNetTaskDbContext(DbContextOptions<DotNetTaskDbContext> options)
            : base(options)
        {
        }
        public DbSet<Questions> Questions { get; set;}
        public DbSet<ProgramTemplate> ProgramTemplate { get; set; }
        public DbSet<QuestionsMapping> QuestionsMapping { get; set; }
        public DbSet<CandidateForm> CandidateForms { get; set; }
        public DbSet<CandidateAnswer> CandidateAnswers { get; set; }

    }
}
