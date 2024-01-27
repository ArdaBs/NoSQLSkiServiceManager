using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Models;
using MongoDB.Bson;

namespace NoSQLSkiServiceManager.Services
{

    public class ServiceOrderService
    {
        private readonly IMongoCollection<ServiceOrder> _serviceOrders;
        private readonly IMongoDatabase _database;
        private readonly IMapper _mapper;

        public ServiceOrderService(IMongoDatabase database, IMapper mapper)
        {
            _serviceOrders = database.GetCollection<ServiceOrder>("serviceOrders");
            _database = database;
            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateWithDetails(CreateServiceOrderRequestDto createDto)
        {
            var serviceType = await _database.GetCollection<ServiceType>("serviceTypes")
                                      .Find(st => st.Id == createDto.ServiceTypeId)
                                      .FirstOrDefaultAsync();

            if (serviceType == null)
            {
                throw new InvalidOperationException("ServiceType not found");
            }

            var servicePriority = await _database.GetCollection<ServicePriority>("servicePriorities")
                                                 .Find(sp => sp.Id == createDto.PriorityId)
                                                 .FirstOrDefaultAsync();

            if (servicePriority == null)
            {
                throw new InvalidOperationException("ServicePriority not found");
            }

            var basePickupDate = createDto.CreationDate.AddDays(7 + servicePriority.DayCount);
            var desiredPickupDate = new DateTime(basePickupDate.Year, basePickupDate.Month, basePickupDate.Day);

            var serviceOrder = new ServiceOrder
            {
                CustomerName = createDto.CustomerName,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                CreationDate = createDto.CreationDate,
                Comments = createDto.Comments,
                Status = "Offen",
                ServiceType = serviceType,
                Priority = servicePriority,
                DesiredPickupDate = desiredPickupDate
            };

            await _serviceOrders.InsertOneAsync(serviceOrder);
            return _mapper.Map<OrderResponseDto>(serviceOrder);
        }


        public async Task<List<OrderResponseDto>> GetAllAsync()
        {
            var serviceOrders = await _serviceOrders.Find(_ => true).ToListAsync();
            return _mapper.Map<List<OrderResponseDto>>(serviceOrders);
        }

        public async Task<OrderResponseDto> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var serviceOrder = await _serviceOrders.Find<ServiceOrder>(so => so.Id == objectId).FirstOrDefaultAsync();
            return _mapper.Map<OrderResponseDto>(serviceOrder);
        }

        public async Task<bool> UpdateAsync(string id, UpdateServiceOrderRequestDto updateDto)
        {
            var objectId = new ObjectId(id);
            var serviceOrder = _mapper.Map<ServiceOrder>(updateDto);
            serviceOrder.Id = objectId;
            var result = await _serviceOrders.ReplaceOneAsync(so => so.Id == objectId, serviceOrder);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var result = await _serviceOrders.DeleteOneAsync(so => so.Id == objectId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<List<OrderResponseDto>> GetAllWithDetailsAsync()
        {
            var serviceOrders = await _serviceOrders
                .Aggregate()
                .Lookup("ServiceType", "ServiceTypeId", "_id", "ServiceType")
                .Lookup("ServicePriority", "PriorityId", "_id", "ServicePriority")
                .ToListAsync();

            return _mapper.Map<List<OrderResponseDto>>(serviceOrders);
        }

    }
}