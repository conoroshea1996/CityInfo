using AutoMapper;
using CityInfo.Api.Models;
using CityInfo.Api.Services;
using CityInfo.API;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Api.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository , IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _cityInfoRepository.GetCities();

            //var results = new List<CityWithoutPointsOfInterestDto>();

            //foreach(var city in cities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = city.Id,
            //        Name = city.Name,
            //        Description = city.Description
            //    });
            //}

            //Created ienumerable of citywithout using value from cities object 
            var results = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);

            return Ok(results);
        }
    
        [HttpGet("{id}")]
        public IActionResult GetCity(int id,bool includePointOfInterest)
        {
            var city = _cityInfoRepository.GetCity(id,includePointOfInterest);

            if(city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                var cityResult = _mapper.Map<CityDto>(city);
                //var cityResult = new CityDto()
                //{
                //    Id = city.Id,
                //    Name = city.Name,
                //    Description = city.Description
                //};

                //foreach(var point in city.PointsOfInterest)
                //{
                //    cityResult.PointsOfInterest.Add(new PointOfInterestDto()
                //    {
                //        Id = point.Id,
                //        Name = point.Name,
                //        Description = point.Description
                //    });
                //}
                return Ok(cityResult);
            }

            //var cityWithoutPointsOfInterest = new CityWithoutPointsOfInterestDto()
            //{
            //    Id = city.Id,
            //    Name =city.Name,
            //    Description = city.Description
            //};
            
            var cityWithoutPointsOfInterest = _mapper.Map<CityWithoutPointsOfInterestDto>(city);
            return Ok(cityWithoutPointsOfInterest);
        }
    }   

}
