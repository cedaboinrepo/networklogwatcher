// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=397704
// To debug code on page load in Ripple or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.
(function () {
    "use strict";

    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    function onDeviceReady() {
        // Handle the Cordova pause and resume events
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);

        // TODO: Cordova has been loaded. Perform any initialization that requires Cordova here.

        createNokiaMap();
        function createNokiaMap() {
            var MapAppId = "4hqGaWnfNvhKB8O94Gvo";
            var MapAppToken = "MZQ9jHhuuIEh9uJGUEGYFQ";

            nokia.Settings.set("appId", MapAppId);
            nokia.Settings.set("authenticationToken", MapAppToken);
            var map = new nokia.maps.map.Display(
             document.getElementById("nokiamap"), {
                 // Zoom level for the map
                 zoomLevel: 5,
                 // Map center coordinates
                // center: [57.0415, -170.8583]
             }
           );

            var heatmapData = [
          	{ "value": 6.1, "longitude": 173.0219, "latitude": 53.1380 },
          	{ "value": 5.8, "longitude": -171.8583, "latitude": 52.0415 },
          	{ "value": 5.4, "longitude": -169.9851, "latitude": 53.3657 },
          	{ "value": 4.6, "longitude": -169.5266, "latitude": 51.2915 },
          	{ "value": 4.4, "longitude": -176.4482, "latitude": 51.5722 },
          	{ "value": 4.3, "longitude": -171.5867, "latitude": 51.8108 },
          	{ "value": 4.1, "longitude": -151.8272, "latitude": 59.8977 },
          	{ "value": 3.6, "longitude": -171.7213, "latitude": 51.6348 },
          	{ "value": 3.8, "longitude": -156.0880, "latitude": 56.1681 }
            ];

            // Definition of color gradient to be used in the Heatmap
            var colorizeAPI = {
                stops: {
                    "0": "#E8680C",
                    "0.25": "#F5A400",
                    "0.5": "#FF9000",
                    "0.75": "#FF4600",
                    "1": "#F51F00"
                },
                interpolate: true
            };

            var hmProvider = new nokia.maps.heatmap.Overlay({
                max: 20,
                colors: colorizeAPI,
                opacity: 0.8,
                type: "density"
            });

            // Assuming that data have been loaded previously:
            hmProvider.addData(heatmapData);
            map.overlays.add(hmProvider);
        }
    };

    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };
})();