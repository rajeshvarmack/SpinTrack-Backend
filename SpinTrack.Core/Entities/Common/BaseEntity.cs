namespace SpinTrack.Core.Entities.Common
{
    /// <summary>
    /// Base entity with audit fields
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
