namespace BiometricoZTK.Domain.Entities
{
    public class EntityBase
    {
        public long Id { get; set; }
        public bool Activo { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
