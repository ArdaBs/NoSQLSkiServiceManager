using MongoDB.Bson;
using MongoDB.Driver;
using AutoMapper;
using NoSQLSkiServiceManager.Models;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoSQLSkiServiceManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NoSQLSkiServiceManager.Services
{

    public class GenericService<TModel, TCreateDto, TUpdateDto, TResponseDto>
    where TModel : class, IEntity
    where TCreateDto : class
    where TUpdateDto : class
    where TResponseDto : class, IResponseDto
    {
        private readonly IMongoCollection<TModel> _collection;
        private readonly IMapper _mapper;

        public GenericService(IMongoDatabase database, IMapper mapper, string collectionName)
        {
            _collection = database.GetCollection<TModel>(collectionName);
            _mapper = mapper;
        }

        public async Task<TResponseDto> CreateAsync(TCreateDto createDto)
        {
            var model = _mapper.Map<TModel>(createDto);
            await _collection.InsertOneAsync(model);
            return _mapper.Map<TResponseDto>(model);
        }

        public async Task<IEnumerable<TResponseDto>> GetAllAsync()
        {
            var items = await _collection.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<TResponseDto>>(items);
        }

        public async Task<TResponseDto> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var item = await _collection.Find<TModel>(x => x.Id == objectId).FirstOrDefaultAsync();
            return _mapper.Map<TResponseDto>(item);
        }

        public async Task<bool> UpdateAsync(string id, TUpdateDto updateDto)
        {
            var objectId = new ObjectId(id);
            var existingItem = await _collection.Find(x => x.Id == objectId).FirstOrDefaultAsync();
            if (existingItem == null)
            {
                return false;
            }

            _mapper.Map(updateDto, existingItem);
            var result = await _collection.ReplaceOneAsync(x => x.Id == objectId, existingItem);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var result = await _collection.DeleteOneAsync(x => x.Id == objectId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }

}
