(function(){
	'use strict';

	var app = angular.module('app', ['ngRoute', 'common']);
	app.constant('configs', {
		
	});
	app.config(config);
	config.$inject = ['$routeProvider'];
	function config($routeProvider) {
	    
		$routeProvider.when("/", {
			templateUrl: "/App/Controller/index.html",
			controller: "indexCtrl"
		}).otherwise({redirectTo: "/"});
	}
})();

