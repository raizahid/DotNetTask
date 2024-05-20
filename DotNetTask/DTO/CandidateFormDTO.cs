namespace DotNetTask.DTO
{
    public class CandidateFormDTO
    {
        public int Id { get; set; }
        public int ProgramTemplateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
