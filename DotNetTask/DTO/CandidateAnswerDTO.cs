namespace DotNetTask.DTO
{
    public class CandidateAnswerDTO
    {
        public int Id { get; set; }
        public int CandidateFormId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
