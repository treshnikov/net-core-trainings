using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Exceptions;
using BusinessLogic.Managers;
using Domain;
using JoggingWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoggingWebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class JoggingDataController : ControllerBase
    {
        private readonly IJoggingDataManager _joggingDataManager;
        
        private long CurrentUserId => Convert.ToInt64(User.FindFirstValue(ClaimTypes.Name));

        public JoggingDataController(IJoggingDataManager joggingDataManager)
        {
            _joggingDataManager = joggingDataManager;
        }

        [HttpGet]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Get(int? skip = null, int take = 10, string filter = null)
        {
            if (skip < 0 || take < 1)
            {
                return BadRequest();
            }
            
            if (take > 50)
            {
                return BadRequest("Can't have more than 50 records on a page");
            }

            var data = _joggingDataManager.Get(CurrentUserId, filter, take, skip);
            return Ok(data.Select(ToModel));
        }
        
        [HttpGet("/Users/{userId}/JoggingData/")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Get(int userId, int take = 10, int? skip = null, string filter = null)
        {
            if (skip < 0 || take < 1 || userId <= 0)
            {
                return BadRequest();
            }
            
            if (take > 50)
            {
                return BadRequest("Can't have more than 50 records on a page");
            }

            var data = _joggingDataManager.Get(userId, filter, take, skip);
            return Ok(data.Select(ToModel));
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Get(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var joggingData = _joggingDataManager.Get(id, CurrentUserId);
            return Ok(ToModel(joggingData));
        }
        
        [HttpGet("/Users/{userId}/JoggingData/{id}")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Get(long id, long userId)
        {
            if (id <= 0 || userId <= 0)
            {
                return BadRequest();
            }

            var joggingData = _joggingDataManager.Get(id, userId);
            return Ok(ToModel(joggingData));
        }

        [HttpPost]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public async Task<IActionResult> Add([FromBody] JoggingDataUpdateModel model)
        {
            ValidateJoggingDataStrict(model);

            var data = FromModel(model);
            await _joggingDataManager.Add(data, CurrentUserId);
            return CreatedAtAction(nameof(Get), new { data.Id }, ToModel(data));
        }
        
        [HttpPost("/Users/{userId}/JoggingData/")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public async Task<IActionResult> Add(long userId, [FromBody] JoggingDataUpdateModel model)
        {
            if (userId <= 0)
            {
                return BadRequest();
            }
            
            ValidateJoggingDataStrict(model);
            
            var data = FromModel(model);
            await _joggingDataManager.Add(data, userId);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, ToModel(data));
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Update(long id, JoggingDataUpdateModel model)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            ValidateJoggingDataStrict(model);

//            var data = FromModel(model);
//            data.Id = id;
            _joggingDataManager.Update(id, model, CurrentUserId);
            return NoContent();
        }
        
        [HttpPut("/Users/{userId}/JoggingData/{id}")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Update(long id, long userId, [FromBody] JoggingDataUpdateModel model)
        {
            if (id <= 0 || userId <= 0)
            {
                return BadRequest();
            }
            
            ValidateJoggingDataStrict(model);

//            var data = FromModel(model);
//            data.Id = id;
            _joggingDataManager.Update(id, model, userId);
            return NoContent();
        }
        
        [HttpPatch("{id}")]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Patch(long id, JoggingDataUpdateModel model)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            ValidateJoggingDataSoft(model);
            
//            var data = FromModel(model);
//            data.Id = id;
            _joggingDataManager.Update(id, model, CurrentUserId);
            return NoContent();
        }
        
        [HttpPatch("/Users/{userId}/JoggingData/{id}")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Patch(long id, long userId, [FromBody] JoggingDataUpdateModel model)
        {
            if (id <= 0 || userId <= 0)
            {
                return BadRequest();
            }
            
            ValidateJoggingDataSoft(model);
            
//            var data = FromModel(model);
//            data.Id = id;
            _joggingDataManager.Update(id, model, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Delete(long id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            _joggingDataManager.Delete(id, CurrentUserId);

            return NoContent();
        }
        
        [HttpDelete("/Users/{userId}/JoggingData/{id}")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Delete(long id, long userId)
        {
            if (id <= 0 || userId <= 0)
            {
                return BadRequest();
            }
            
            _joggingDataManager.Delete(id, userId);

            return NoContent();
        }
        
        private void ValidateJoggingDataSoft(JoggingDataUpdateModel model)
        {
            if (model == null)
            {
                throw new BadRequestException();
            }

            var minDate = new DateTime(1900, 1, 1);
            if (model.Date.HasValue  && (model.Date < minDate || model.Date > DateTime.Today.AddDays(1)))
            {
                throw new BadRequestException("Date should not be empty");
            }

            if (model.Distance.HasValue && model.Distance < 0)
            {
                throw new BadRequestException("Distance should be greater than 0");
            }

            if (model.Time.HasValue && model.Time < 0)
            {
                throw new BadRequestException("Time should be greater than 0");
            }

            if (model.Latitude.HasValue && model.Latitude < -90 || model.Latitude > 90)
            {
                throw new BadRequestException("Invalid latitude");
            }

            if (model.Longitude.HasValue && model.Longitude < -180 || model.Longitude > 180)
            {
                throw new BadRequestException("Invalid longitude");
            }
        }
        
        private void ValidateJoggingDataStrict(JoggingDataUpdateModel model)
        {
            if (model == null)
            {
                throw new BadRequestException();
            }

            var minDate = new DateTime(1900, 1, 1);
            if (!model.Date.HasValue || model.Date < minDate || model.Date > DateTime.Today.AddDays(1))
            {
                throw new BadRequestException("Date should not be empty");
            }

            if (!model.Distance.HasValue || model.Distance <= 0)
            {
                throw new BadRequestException("Distance should be greater than 0");
            }

            if (!model.Time.HasValue || model.Time <= 0)
            {
                throw new BadRequestException("Time should be greater than 0");
            }

            if (!model.Latitude.HasValue || model.Latitude < -90 || model.Latitude > 90)
            {
                throw new BadRequestException("Invalid latitude");
            }

            if (!model.Longitude.HasValue ||  model.Longitude < -180 || model.Longitude > 180)
            {
                throw new BadRequestException("Invalid longitude");
            }
        }

        private JoggingDataGetModel ToModel(JoggingData data)
        {
            return new JoggingDataGetModel
            {
                Id = data.Id,
                Date = data.Date,
                Distance = data.Distance,
                Time = data.Time,
                Latitude = data.Latitude,
                Longitude = data.Longitude,
                WeatherInfo = data.WeatherInfo
            };
        }
        
        private JoggingData FromModel(JoggingDataUpdateModel model)
        {
            return new JoggingData
            {
                Date = model.Date.Value,
                Distance = model.Distance.Value,
                Time = model.Time.Value,
                Latitude = model.Latitude.Value,
                Longitude = model.Longitude.Value
            };
        }
    }
}