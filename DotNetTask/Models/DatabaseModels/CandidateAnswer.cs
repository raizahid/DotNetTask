namespace DotNetTask.Models.DatabaseModels
{
    public class CandidateAnswer
    {
        public int Id { get; set; }
        public int CandidateFormId { get; set; }
        public CandidateForm? CandidateForm { get; set; }
        public int QuestionId { get; set; }
        public Questions? Question { get; set; }
        public string Answer { get; set; }
    }
}
