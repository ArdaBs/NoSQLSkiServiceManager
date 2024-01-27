using AutoMapper;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using System.Collections.Generic;

public class MappingCode : Profile
{
    public MappingCode()
    {
        // ServiceOrder Mappings
        CreateMap<CreateServiceOrderRequestDto, ServiceOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateServiceOrderRequestDto, ServiceOrder>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<ServiceOrder, OrderResponseDto>()
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));

        CreateMap<IEnumerable<ServiceOrder>, OrderListResponseDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src));

        // Employee Mappings
        CreateMap<EmployeeCreateDto, Employee>();
        CreateMap<EmployeeUpdateDto, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Employee, EmployeeResponseDto>();

        CreateMap<Employee, LoginResponseDto>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

        CreateMap<ServiceType, ServiceTypeDto>();
        CreateMap<ServicePriority, ServicePriorityDto>();
    }
}