namespace MLSCore.Models
{
    public class TbTask
    {public int Id { get; set; }
        string Title { get; set; }
        string URL { get; set; }
        public DateTime StartDate {get;set; }
        public DateTime EndDate {get;set; }
        public string? CreatedBy { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public int CurrentState { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("SubContent")]
        public int SubContentId { get; set; }
        public TbSubContent SubContent { get; set; }


    }
}
