using AutoMapper;
using CrudPark.API.DTOs;
using CrudPark.API.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        
        // --- REGLAS PARA MEMBERSHIP ---
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
        
        //--- REGLAS PARA RATE ---
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
        
        // --- REGLAS PARA OPERATOR ---
        CreateMap<OperatorCreateDto, Operator>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore());

        CreateMap<Operator, OperatorResponseDto>();


        CreateMap<Operator, OperatorResponseDto>();
        
        // --- REGLAS PARA TICKET ---
        CreateMap<TicketEntryDto, Ticket>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Folio, opt => opt.Ignore()) 
            .ForMember(dest => dest.EntryDateTime, opt => opt.Ignore()) 
            .ForMember(dest => dest.ExitDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.EntryType, opt => opt.Ignore()) 
            .ForMember(dest => dest.ExitOperatorId, opt => opt.Ignore())
            .ForMember(dest => dest.TotalMinutes, opt => opt.Ignore())
            .ForMember(dest => dest.MembershipId, opt => opt.Ignore())
            .ForMember(dest => dest.QRCode, opt => opt.Ignore()) 
            .ForMember(dest => dest.RateApplied, opt => opt.Ignore())
            .ForMember(dest => dest.TotalCost, opt => opt.Ignore());
        
        CreateMap<Ticket, TicketResponseDto>()
            // Conversión de Enum a String para la respuesta
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType.ToString()))
            .ForMember(dest => dest.EntryType, opt => opt.MapFrom(src => src.EntryType.ToString()));
        
        CreateMap<Operator, OperatorResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore()); 


    }
}