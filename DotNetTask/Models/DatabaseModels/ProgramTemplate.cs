using DotNetTask.Models.Enum;

namespace DotNetTask.Models.DatabaseModels
{
    public class ProgramTemplate
    {
        public int Id { get; set; }
        public required string ProgramTitle { get; set; }
        public required string ProgramDescription { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public string? Nationality { get; set; }
        public string? IDCard { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

    }
}
