using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using CityInfo.API;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Api.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger
            , IMailService mailService 
            , ICityInfoRepository cityInfoRepository
            , IMapper mapper 
            )
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.cityExist(cityId))
                {
                    _logger.LogInformation($"City with {cityId} was not found :(");
                    return NotFound();
                }
                var pointOfInterestForCity = _cityInfoRepository.GetPointsOfInterest(cityId);
                //var pointsOfInterestForCityResults = new List<PointOfInterestDto>();

                //foreach(var point in pointOfInterestForCity)
                //{
                //    pointsOfInterestForCityResults.Add(new PointOfInterestDto()
                //    {
                //        Id = point.Id,
                //        Name = point.Name,
                //        Description = point.Description
                //    });
                //}
                var pointsOfInterestForCityResults = _mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestForCity);

                return Ok(pointsOfInterestForCityResults);
            }

            catch(Exception ex)
            {
                _logger.LogCritical($"Exception while trying to get points of interest",ex);
                return StatusCode(500);
            }
            

        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.cityExist(cityId)) { 
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            //var pointOfInterestResult = new PointOfInterestDto()
            //{
            //    Id = pointOfInterest.Id,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            var pointOfInterestResult = _mapper.Map<PointOfInterestDto>(pointOfInterest);
            return Ok(pointOfInterestResult);
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, PointOfInterestForCreationDto pointForCreation)
        {
            if (pointForCreation.Name == pointForCreation.Description)
            {
                ModelState.AddModelError("Invalid fields", "Name and Descriptiong should be different");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
           
            if (!_cityInfoRepository.cityExist(cityId))
            {
                return NotFound();
            }

            //selects all points of interest from cities and gets the highest number
            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            //creates new final point of interest class
            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxPointOfInterestId,
            //    Name = pointForCreation.Name,
            //    Description = pointForCreation.Description
            //};

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointForCreation);

            _cityInfoRepository.AddPointOfInterestForCity(cityId,finalPointOfInterest);
            _cityInfoRepository.save();

            var createdPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            //returns StatusCode 201 with location of new item and also the object
            return CreatedAtRoute(
                "GetPointOfInterest",
                new { cityId, id = createdPointOfInterest.Id },
                createdPointOfInterest
                );
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, PointOfInterestForUpdateDto pointOfInterestForUpdate)
        {
            if (pointOfInterestForUpdate.Name == pointOfInterestForUpdate.Description)
            {
                ModelState.AddModelError("Invalid fields", "Name and Descriptiong should be different");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.cityExist(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId,id);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterestForUpdate, pointOfInterestEntity);

            _cityInfoRepository.save();
            
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id, JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            //if (pointOfInterest == null)
            //{
            //    return NotFound();
            //}

            if (!_cityInfoRepository.cityExist(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);


            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }


            //Created copy of point of Interest document
            //var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            //{
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            //apply json patch methods 
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Invalid fields", "Name and Descriptiong should be different");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            _cityInfoRepository.save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}
            if (!_cityInfoRepository.cityExist(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterestFromCity(pointOfInterestEntity);
            _cityInfoRepository.save();
            
            _mailService.Send("Point of interest deleted", $"Point of interest with id of {pointOfInterestEntity.Id} has been deleted");
            return NoContent();
        }
    }
}
