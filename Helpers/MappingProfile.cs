using AutoMapper;
using CrudPark.API.DTOs;
using CrudPark.API.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MembershipCreateDto, Membership>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Membership, MembershipResponseDto>()
            .ForMember(dest => dest.VehicleType, 
                opt => opt.MapFrom(src => src.VehicleType.ToString())); 
            
        CreateMap<MembershipUpdateDto, Membership>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); 
        
        CreateMap<RateCreateDto, Rate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()); 
        
        CreateMap<Rate, RateResponseDto>()
            .ForMember(dest => dest.VehicleType, 
                opt => opt.MapFrom(src => src.VehicleType.ToString()));
        
        CreateMap<RateUpdateDto, Rate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}