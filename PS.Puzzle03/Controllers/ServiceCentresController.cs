using System;
using System.Collections.Generic;
using System.Web.Http;
using PS.Puzzle03.Domain;
using PS.Puzzle03.Models;
using System.Net.Http;

namespace PS.Puzzle03.Controllers
{
    [RoutePrefix("api/servicecentres")]
    public class ServiceCentresController : ApiController
    {
        private readonly IServiceCentreService _serviceCentreService;

        public ServiceCentresController(IServiceCentreService serviceCentreService)
        {
            _serviceCentreService = serviceCentreService;
        }

        [Route("getlnglat/{postCode}")]
        [HttpGet]
        public MapLocation Getlnglat(string postCode)
        {
            return _serviceCentreService.GetLngLat(postCode);
        }


        [Route("getnearby/{latitude:double}/{longitude:double}")]
        [HttpGet]
        public IEnumerable<ServiceCentre> Getnearby(double latitude, double longitude)
        {
            var mapLocation = new MapLocation { Latitude = latitude, Longitude = longitude };
            return _serviceCentreService.GetNearby(mapLocation);
        }
    }
}
