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
		   .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
		   .ReverseMap()
		   .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore());
		}
	}
}
