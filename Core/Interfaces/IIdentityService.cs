using Core.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{

	public interface IIdentityService
	{
		Task<Result<Guid>> RegisterUserAsync(string email, string password, string firstName, string lastName);
		Task<Result<Guid>> LoginUserAsync(string email, string password);
		Task<Result<string>> AssignRoleAsync(Guid userId, string role);
		Task<User> GetUserByIdAsync(Guid userId);
		Task<List<string>> GetUserRolesAsync(User user);
		Task<Result<bool>> UpdateUserAsync(User user);
	}
}
