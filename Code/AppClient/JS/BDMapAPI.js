function GetRoutePoints(StartPointX, StartPointY, EndPointX, EndPointY) {
    var driving = new BMap.DrivingRoute(map); //驾车实例
    //var driving = new BMap.WalkingRoute(map);
    driving.search(new BMap.Point(StartPointX, StartPointY), new BMap.Point(EndPointX, EndPointY));
    driving.setSearchCompleteCallback(function () {
        var pts = driving.getResults().getPlan(0).getRoute(0).getPath(); //通过驾车实例，获得一系列点的数组
        var content = "";
        for (var i = 0; i < pts.length; i++) {
            content += pts[i].lng + "," + pts[i].lat + ";";
        }
        document.getElementById("points").innerText = content;
    });
}

function SetPanoramaPosition(PointX, PointY) {
    var BDPoint = new BMap.Point(PointX, PointY);
    panorama.setPosition(BDPoint);
}

function Test() {
    alert("ok");
}