let dotnetHelper, map, polyline, markerGroup, polylineGroup;

window.registerCallback = (_dotnetHelper) => {
    dotnetHelper = _dotnetHelper;
}

function loadMap() {
    map = L.map("map").setView([47.18015193287305, 19.504049742236568], 7);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    markerGroup = L.featureGroup().addTo(map)
    polylineGroup = L.featureGroup().addTo(map)
}

function loadMapWithId(mapId){
    map = L.map(mapId).setView([47.18015193287305, 19.504049742236568], 7);
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map);
    markerGroup = L.featureGroup().addTo(map)
    polylineGroup = L.featureGroup().addTo(map)
}

function addMarkers(latlngs) {
    latlngs.forEach(coord => {
        L.marker([coord.lat, coord.lng]).addTo(markerGroup);
    });
    if (latlngs.length > 0) {
        map.fitBounds(markerGroup.getBounds(), { padding: [40, 40] });
        map.setZoom(7);
    }
    addLine(latlngs);
}

function addLine(latlngs) {
    if (latlngs.length > 1) {
        if (isPolylineOnMap(polyline)) {
            map.removeLayer(polyline);
        }
        polyline = L.polyline(latlngs, { color: 'blue' }).addTo(polylineGroup);
        map.fitBounds(polyline.getBounds(), { padding: [40, 40] });
    }
}

function isPolylineOnMap(polyline) {
    return Object.values(map._layers).includes(polyline);
}

function clearMap() {
    markerGroup.clearLayers();
    polylineGroup.clearLayers();
}
