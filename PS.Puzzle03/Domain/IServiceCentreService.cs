using System.Collections.Generic;
using PS.Puzzle03.Models;

namespace PS.Puzzle03.Domain
{
    public interface IServiceCentreService
    {
        IEnumerable<ServiceCentre> GetNearby(MapLocation mapLocation);
        MapLocation GetLngLat(string postCode);
    }
}
