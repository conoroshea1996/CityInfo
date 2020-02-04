using CityInfo.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Api.Services
{
   public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        PointOfInterest GetPointOfInterest(int cityId, int pointOfInterestId);

        bool cityExist(int cityId);

        void AddPointOfInterestForCity(int cityId, PointOfInterest pointofInterest);

        void DeletePointOfInterestFromCity(PointOfInterest pointOfInterest);
        bool save();
    }
}
