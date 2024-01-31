using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using System;
using System.Threading.Tasks;

namespace NoSQLSkiServiceManager.Services
{
    public class ServiceOrderService : GenericService<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto>
    {
        private readonly IMongoCollection<ServiceType> _serviceTypes;
        private readonly IMongoCollection<ServicePriority> _servicePriorities;

        public ServiceOrderService(IMongoDatabase database, IMapper mapper)
            : base(database, mapper, "serviceOrders")
        {
            _serviceTypes = database.GetCollection<ServiceType>("serviceTypes");
            _servicePriorities = database.GetCollection<ServicePriority>("servicePriorities");
        }

        public override async Task<OrderResponseDto> CreateAsync(CreateServiceOrderRequestDto createDto)
        {
            var serviceType = await _serviceTypes
                                .Find(st => st.Id == createDto.ServiceTypeId)
                                .FirstOrDefaultAsync();

            if (serviceType == null)
            {
                throw new InvalidOperationException("ServiceType not found");
            }

            var servicePriority = await _servicePriorities
                                     .Find(sp => sp.Id == createDto.PriorityId)
                                     .FirstOrDefaultAsync();

            if (servicePriority == null)
            {
                throw new InvalidOperationException("ServicePriority not found");
            }

            var basePickupDate = createDto.CreationDate.AddDays(7 + servicePriority.DayCount);
            var desiredPickupDate = new DateTime(basePickupDate.Year, basePickupDate.Month, basePickupDate.Day);

            decimal priceAdjustmentFactor = 1.0m;
            if (servicePriority.PriorityName == "Express")
            {
                priceAdjustmentFactor = 1.2m;
            }
            else if (servicePriority.PriorityName == "Low")
            {
                priceAdjustmentFactor = 0.8m;
            }
            serviceType.Cost *= priceAdjustmentFactor;

            var serviceOrder = _mapper.Map<ServiceOrder>(createDto);
            serviceOrder.ServiceType = serviceType;
            serviceOrder.Priority = servicePriority;
            serviceOrder.DesiredPickupDate = desiredPickupDate;
            serviceOrder.Status = OrderStatus.Offen;

            await _collection.InsertOneAsync(serviceOrder);
            return _mapper.Map<OrderResponseDto>(serviceOrder);
        }

    }
}
