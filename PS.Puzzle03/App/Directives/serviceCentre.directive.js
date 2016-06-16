/**
 * Created by DA on 28/04/2016.
 */

(function (app) {
    app.directive('serviceCentre', ['$http', '$q', '$routeParams', 'configs', function ($http, $q, $routeParams, configs) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/app/directives/serviceCentre.html',
            scope: true,
            link: function (scope) {
                scope.selectedPostCode = '';
                scope.serviceCentres = [];

                var config = {
                    latitude: '0',
                    longitude: '0'
                };

                var noOfServiceCentres = 4;

                scope.getServiceCentres = function () {
                    if (!scope.selectedPostCode) {
                        scope.serviceCentres = [];
                        return;
                    }
                    var latlng_url = "/api/servicecentres/getlnglat/" + scope.selectedPostCode + "/";
                    console.log(latlng_url);
                    $http.get(latlng_url, null)
                        .then(function (result) {
                            latlng_completed(result);
                        }, function (error) {
                            latlng_failed(error);
                        });
                };

                function latlng_completed(response) {
                    config.latitude = response.data.Latitude;
                    config.longitude = response.data.Longitude;
                    var search_url = "/api/servicecentres/getnearby/" + config.latitude + "/" + config.longitude + "/";
                    $http.get(search_url, null)
                        .then(function (result) {
                            search_completed(result);
                        }, function (error) {
                            search_failed(error);
                        });
                }

                function latlng_failed(response) {
                    config.latitude = 0;
                    config.longitude = 0;
                }

                function search_completed(response) {
                    scope.serviceCentres = response.data;
                }

                function search_failed(response) {
                    scope.serviceCentres = [];
                }
            }
        }
}])

})(angular.module('common'))