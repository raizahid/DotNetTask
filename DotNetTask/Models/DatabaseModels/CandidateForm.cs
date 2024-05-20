namespace DotNetTask.Models.DatabaseModels
{
    public class CandidateForm
    {
        public int Id { get; set; }
        public int ProgramTemplateId { get; set; }
        public ProgramTemplate? ProgramTemplate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
