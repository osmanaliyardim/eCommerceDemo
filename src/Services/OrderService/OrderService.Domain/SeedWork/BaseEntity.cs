using MediatR;

namespace OrderService.Domain.SeedWork;

public abstract class BaseEntity
{
    public virtual Guid Id { get; protected set; }

    public DateTime CreateDate { get; set; }


    int? _requestedHasCode;

    private List<INotification> domainEvents;

    public IReadOnlyCollection<INotification> DomainEvents => domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        domainEvents = domainEvents ?? new List<INotification>();
        domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvent()
    {
        domainEvents?.Clear();
    }

    public bool isTransient()
    {
        return Id == default;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || !(obj is BaseEntity))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        BaseEntity item = (BaseEntity)obj;

        if (item.isTransient() || isTransient())
            return false;
        else
            return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!isTransient())
        {
            if (!_requestedHasCode.HasValue)
            {
                _requestedHasCode = Id.GetHashCode() * 31; // XOR for random distribution
            }

            return _requestedHasCode.Value;
        }
        else
            return base.GetHashCode();
    }

    public static bool operator ==(BaseEntity left, BaseEntity right)
    {
        if (Equals(left, null))
            return Equals(right, null) ? true : false;
        else
            return left.Equals(right);
    }

    public static bool operator !=(BaseEntity left, BaseEntity right)
    {
        return !(left == right);
    }
}