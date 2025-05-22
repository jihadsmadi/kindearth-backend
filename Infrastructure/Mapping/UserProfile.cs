using AutoMapper;
using Core.Entities;
using Infrastructure.Identity.Models;


namespace Infrastructure.Mapping
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<AppUser, User>()
				.ReverseMap()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
		}
	}
}
