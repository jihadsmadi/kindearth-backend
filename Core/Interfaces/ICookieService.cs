using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface ICookieService
	{
		void SetJwtCookies(string accessToken, string refreshToken);
		bool RemoveJwtCookies();
	}
}
