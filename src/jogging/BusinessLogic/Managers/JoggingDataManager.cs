using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DAL;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using Domain;
using JoggingWebApp.Models;

namespace BusinessLogic.Managers
{
    public class JoggingDataManager : IJoggingDataManager
    {
        private readonly IRepository<JoggingData> _repository;
        private readonly IUserManager _userManager;
        private readonly IWeatherProvider _weatherProvider;

        public JoggingDataManager(
            IRepository<JoggingData> repository, 
            IUserManager userManager,
            IWeatherProvider weatherProvider)
        {
            _repository = repository;
            _userManager = userManager;
            _weatherProvider = weatherProvider;
        }

        public JoggingData Get(long id, long userId)
        {
            return _repository.Query().FirstOrDefault(d => d.User.Id == userId && d.Id == id) ??
                   throw new NotFoundException("Data not found");
        }

        public Paging<JoggingData> Get(long userId, string filter, int take, int? skip)
        {
            IQueryable<JoggingData> query = _repository.Query()
                .Where(d => d.User.Id == userId)
                .OrderBy(d => d.Date);
            
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filterParser = new FilterParser.FilterParser<JoggingData>(filter);
                query = filterParser.Filter(query);
            }

            var totalCount = query.Count();
            
            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            query = query.Take(take);

            return new Paging<JoggingData>( totalCount, skip ?? 0, query.AsEnumerable());
        }

        public async Task Add(JoggingData data, long userId)
        {
            data.User = _userManager.Get(userId);
            data.WeatherInfo = await _weatherProvider.Get(data.Latitude, data.Longitude);
            _repository.Add(data);
        }

        public void Update(long id, JoggingDataUpdateModel model, long userId)
        {
            var dbData = Get(id, userId);

            if (model.Distance.HasValue)
            {
                dbData.Distance = model.Distance.Value;
            }

            if (model.Time.HasValue)
            {
                dbData.Time = model.Time.Value;
            }

            if (model.Date.HasValue)
            {
                dbData.Date = model.Date.Value;
            }

            if (model.Longitude.HasValue)
            {
                dbData.Longitude = model.Longitude.Value;
            }
            
            if (model.Latitude.HasValue)
            {
                dbData.Latitude = model.Latitude.Value;
            }
            
            _repository.Update(dbData.Id, dbData);
        }

        public void Delete(long id, long userId)
        {
            if (!_repository.Query().Any(d => d.User.Id == userId && d.Id == id))
            {
                throw new NotFoundException("Data not found");
            }
            
            _repository.Delete(id);
        }
    }
}