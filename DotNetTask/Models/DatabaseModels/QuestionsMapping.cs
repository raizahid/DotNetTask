namespace DotNetTask.Models.DatabaseModels
{
    public class QuestionsMapping
    {
        public int Id { get; set; }
        public int QuestionsId { get; set; }
        public Questions? Questions { get; set; }
        public int ProgramTemplateId { get; set; }
        public ProgramTemplate? ProgramTemplate { get; set; }
    }
}
