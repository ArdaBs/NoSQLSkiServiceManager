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
        private readonly IMapper _mapper;

        public ServiceOrderService(IMongoDatabase database, IMapper mapper)
        {
            _serviceOrders = database.GetCollection<ServiceOrder>("ServiceOrders");
            _mapper = mapper;
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
    }
}