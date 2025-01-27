namespace Ridely.Domain.Abstractions;
public abstract class AuditableEntity : Entity
{
    public long? CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

}
