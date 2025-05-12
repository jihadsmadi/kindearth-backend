namespace Infrastructure.Services
{
	public interface ICurrentUserService
	{
		Guid? UserId { get; }
		bool IsAdmin { get; }
	}
}
