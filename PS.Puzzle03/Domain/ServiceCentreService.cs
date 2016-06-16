using System.Collections.Generic;
using System.Linq;
using PS.Puzzle03.Models;
using System.Net;
using System.Configuration;
using System.Xml.Linq;

namespace PS.Puzzle03.Domain
{
    public class ServiceCentreService : IServiceCentreService
    {
        private const string geoCodingUri = "https://maps.googleapis.com/maps/api/geocode/xml?address={0},+{1}&key={2}";
        private const string placeSearchUri = "https://maps.googleapis.com/maps/api/place/radarsearch/xml?location={0},{1}&rankby={2}&radius={3}&type={4}&key={5}";
        private const string placeDetailsUri = "https://maps.googleapis.com/maps/api/place/details/xml?placeid={0}&key={1}";
        private const string type = "local_government_office";
        private const int radius = 5000;
        private const int noOfRecords = 4;
        private const string rankby = "distance";
        private string countryCode = ConfigurationManager.AppSettings["countryCode"];
        private string key = ConfigurationManager.AppSettings["googleApiKey"];
        public IEnumerable<ServiceCentre> GetNearby(MapLocation mapLocation)
        {
            var list = new List<ServiceCentre>();
            
            double latitude = mapLocation.Latitude;
            double longitude = mapLocation.Longitude;

            var requestUri = string.Format(placeSearchUri, latitude, longitude, rankby, radius, type, key);
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());
            XElement parent = xdoc.Element("PlaceSearchResponse");
            if (parent == null) return list;
            var results = parent.Elements("result");
            foreach (var result in results.Take(noOfRecords)) { 
                var serviceCentre = GetServiceCentre(result);
                list.Add(serviceCentre);
            }
            return list;
        }

        public MapLocation GetLngLat(string postCode)
        {
            var mapLocation = new MapLocation();
            var requestUri = string.Format(geoCodingUri, postCode, countryCode, key);
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());
            XElement parent = xdoc.Element("GeocodeResponse");
            if (parent == null) return mapLocation;
            var result = parent.Element("result");
            if (result != null)
            {
                var geometry = result.Element("geometry");
                double lat = 0.0;
                double lng = 0.0;
                if (geometry != null)
                {
                    var locationElement = geometry.Element("location");
                    if (locationElement != null)
                    {
                        double.TryParse(CommonHelper.ParseXmlElement(locationElement.Element("lat")), out lat);
                        double.TryParse(CommonHelper.ParseXmlElement(locationElement.Element("lng")), out lng);
                    }
                }
                mapLocation.Latitude = lat;
                mapLocation.Longitude = lng;
            }
            return mapLocation;
        }

        private ServiceCentre GetServiceCentre(XElement result)
        {
            var geometry = result.Element("geometry");
            double lat = 0.0;
            double lng = 0.0;
            if (geometry != null)
            {
                var locationElement = geometry.Element("location");
                if (locationElement != null)
                {
                    double.TryParse(CommonHelper.ParseXmlElement(locationElement.Element("lat")), out lat);
                    double.TryParse(CommonHelper.ParseXmlElement(locationElement.Element("lng")), out lng);
                }
            }
            string placeId = CommonHelper.ParseXmlElement(result.Element("place_id"));

            var serviceCentre = new ServiceCentre { Latitude = lat, Longitude = lng };
            // Now call this function to get the extended details like phone number
            GetServiceCentreDetails(serviceCentre, placeId);
            return serviceCentre;
        }

        private void GetServiceCentreDetails(ServiceCentre serviceCentre, string placeId)
        {
            if (string.IsNullOrEmpty(placeId)) return;
            var requestUri = string.Format(placeDetailsUri, placeId, key);
            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("PlaceDetailsResponse").Element("result");
            if(result != null)
            {
                serviceCentre.Name = CommonHelper.ParseXmlElement(result.Element("name"));
                serviceCentre.Address = CommonHelper.ParseXmlElement(result.Element("formatted_address"));
                serviceCentre.Phone = CommonHelper.ParseXmlElement(result.Element("formatted_phone_number"));
                serviceCentre.Url = CommonHelper.ParseXmlElement(result.Element("url"));
                serviceCentre.ImageUrl = CommonHelper.ParseXmlElement(result.Element("icon"));
            }
        }
    }
}
