using AutoMapper;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs.Response;

public class MappingCode : Profile
{
    public MappingCode()
    {
        CreateMap<CreateServiceOrderRequestDto, ServiceOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateServiceOrderRequestDto, ServiceOrder>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<ServiceOrder, OrderResponseDto>();
        CreateMap<IEnumerable<ServiceOrder>, OrderListResponseDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src));

    }
}
