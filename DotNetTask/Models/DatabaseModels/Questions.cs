using DotNetTask.Models.Enum;

namespace DotNetTask.Models.DatabaseModels
{
    public class Questions
    {
        public int Id { get; set; }
        public required string Question { get; set; }
        public required QuestionType QuestionType { get; set; }
        public string? QuestionString { get; set; }
    }
}
