using DotNetTask.Models.Enum;

namespace DotNetTask.DTO
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public QuestionType QuestionType { get; set; }
        public string? QuestionString { get; set; }
    }
}
