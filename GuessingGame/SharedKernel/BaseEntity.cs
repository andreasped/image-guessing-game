using GuessingGame.SharedKernel;

namespace GuessingGame.SharedKernel;

public abstract class BaseEntity
{
	public List<BaseDomainEvent> Events = new();
}
