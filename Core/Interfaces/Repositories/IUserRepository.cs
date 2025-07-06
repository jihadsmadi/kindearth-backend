using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories
{
	public interface IUserRepository
	{
		Task<Result<Guid>> RegisterUserAsync(string email, string password, string firstName, string lastName, string phone);
		Task<Result<Guid>> LoginUserAsync(string email, string password);
		Task<Result<string>> AssignRoleAsync(Guid userId, string role);
		Task<Result<string>> ReplaceUserRoleAsync(Guid userId, string newRole);
		Task<UserDto> GetUserByIdAsync(Guid userId);
		Task<List<string>> GetUserRolesAsync(UserDto user);
		Task<Result<bool>> UpdateUserAsync(UserDto user);
		bool VerifyRefreshToken(UserDto user, string rawToken);
		Task<Result<bool>> CreateVendorProfileAsync(Guid userId, string storeName);
	}
}
