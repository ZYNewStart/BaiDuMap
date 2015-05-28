//去除网页中的连接地址
window.onload = inifA;
function inifA() {
    for (var i = 0; i < window.document.getElementsByTagName('a').length; i++) {
        window.document.getElementsByTagName('a')[i].onclick = function () { return false; };
    }
}
//*定义必要的公共变量
var maker;//标注对象
var distance;//测距对象
var drawingManager;//绘图对象
var drag;//拖框缩放对象
//绘制工具栏外观设定
var styleOptions = {
    strokeColor: 'red',    //边线颜色。
    fillColor: 'red',      //填充颜色。当参数为空时，圆形将没有填充效果。
    strokeWeight: 3,       //边线的宽度，以像素为单位。
    strokeOpacity: 0.8,       //边线透明度，取值范围0 - 1。
    fillOpacity: 0.6,      //填充的透明度，取值范围0 - 1。
    strokeStyle: 'solid' //边线的样式，solid或dashed。
};

//*
//*结束//

//*地图基础方法*//
//地图平移
function PanTo(lng, lat) {
    map.panTo(new BMap.Point(lng, lat));
}

//返回当前地图中心坐标
function GetCenter() {
    window.document.getElementById('lng').innerText = map.getCenter().lng;
    window.document.getElementById('lat').innerText = map.getCenter().lat;
}

//设置当前地图所在城市
function SetCity(cityName) {
    map.setCenter(cityName);
}

//将地图放大一级
function ZoomIn() {
    map.zoomIn();
}

//将地图缩小一级
function ZoomOut() {
    map.zoomOut();
}

//添加版权控件
function AddCopyrightControl() {
    map.addControl(new BMap.CopyrightControl());
}

//添加地图类型控件
function AddMapTypeControl() {
    map.addControl(new BMap.MapTypeControl());
}

//添加比例尺控件
function AddScaleControl() {
    map.addControl(new BMap.ScaleControl());
}

//添加缩略图控件
function AddOverviewMapControl() {
    map.addControl(new BMap.OverviewMapControl());
}

//开启滚轮调节地图
function EnableScrollWheelZoom() {
    map.enableScrollWheelZoom();
}

//关闭滚轮调节地图
function DisableScrollWheelZoom() {
    map.disableScrollWheelZoom();
}
//*
//*结束*//


//*覆盖物方法*//
//添加一个圆
function AddCirle(lng, lat, r) {
    var circle = new BMap.Circle(new BMap.Point(lng, lat), r);
    map.addOverlay(circle);
}

//添加交通流图层
function AddTrafficLayer() {
    var traffic = new BMap.TrafficLayer();
    map.addTileLayer(traffic);
}

//添加普通标注
function AddNormalMaker(lng, lat) {
    var marker = new BMap.Marker(new BMap.Point(lng, lat));  // 创建标注
    map.addOverlay(marker);
}

//标注开启拖拽
function OpenMakerDraging() {
    marker.enableDragging(true);
}

//标注关闭拖拽
function CloseMakerDraging() {
    marker.disableDragging(true);
}
//添加动画标注
function AddAnimationMaker(lng, lat) {
}

//添加包含一个标签的标注
function AddLabelMaker(lng, lat, content) {
    var marker = new BMap.Marker(new BMap.Point(lng, lat));  // 创建标注
    var label = new BMap.Label(content);
    marker.setLabel(label);
}

//添加包含一个信息窗口的标注
function AddWindowMaker(lng, lat, content) {
    var marker = new BMap.Marker(new BMap.Point(lng, lat));  // 创建标注
    maker.addEventListener('click', function () {
        // 创建信息窗口对象
        var info = new BMap.InfoWindow(content);
        marker.openInfoWindow(infoWindow);
    });
}

//添加城市边界
function SetBoundary(city) {
    var bdary = new BMap.Boundary();
    bdary.get(city, function (rs) {       //获取行政区域
        map.clearOverlays();        //清除地图覆盖物       
        var count = rs.boundaries.length; //行政区域的点有多少个
        for (var i = 0; i < count; i++) {
            var ply = new BMap.Polygon(rs.boundaries[i], { strokeWeight: 2, strokeColor: '#ff0000' }); //建立多边形覆盖物
            map.addOverlay(ply);  //添加覆盖物
            map.setViewport(ply.getPath());    //调整视野         
        }
    });
}

//添加一个信息窗口
function AddInfoWindow(lng, lat, content) {
    var point = new BMap.Point(lng, lat);
    var info = new BMap.InfoWindow(content);
    map.openInfoWindow(info, point);
}

//添加一个标注
function AddLabel(lng, lat, content) {
    var point = new BMap.Point(lng, lat);
    var label = new BMap.Label(content, { point: point });
}
//*
//*结束*//

//*地图服务*//

//本地搜索
function LocalSearch(keywords) {
    var local = new BMap.LocalSearch(map, {
        renderOptions: {
            map: map,
            autoViewport: true
        }
    });
    local.search(keywords);
}

//周边搜索
function SearchNearby(keywords, center) {
    var local = new BMap.LocalSearch(map, {
        renderOptions: {
            map: map,
            autoViewport: true
        }
    });
    local.searchNearby(keywords, center);
}

//范围搜索
function SearchInBounds(keywords) {
    var local = new BMap.LocalSearch(map, {
        renderOptions: {
            map: map
        }
    });
    local.searchInBounds(keywords, map.getBounds());
}

//公交导航
function search(start, end, route) {
    var transit = new BMap.DrivingRoute(map, {
        renderOptions: { map: map, panel: 'r-result' },
        policy: route
    });
    transit.search(start, end);
}

//公交策略
function GetTakeTransiteWay(start, end, route) {
    var transit;
    transit = new BMap.TransitRoute(map, {
        renderOptions: { map: map, panel: 'r-result' },
        policy: route
    });
    transit.search(start, end);
}

//步行导航
function GetWalkingRoute(start, end) {
    var walking = new BMap.WalkingRoute(map, {
        renderOptions: {
            map: map,
            autoViewport: true
        }
    });
    walking.search(start, end);
}

//驾车导航
function GetDrivingRoute(start, end, route) {
    var transit = new BMap.DrivingRoute(map, {
        renderOptions: { map: map, panel: 'r-result' },
        policy: route
    });
    transit.search(start, end);
}


function GetDrivingRouteDate(start, end, route) {
    var options = {
        onSearchComplete: function (results) {
            if (driving.getStatus() == BMAP_STATUS_SUCCESS) {
                // 获取第一条方案
                var plan = results.getPlan(0);

                // 获取方案的驾车线路
                var route = plan.getRoute(0);

                // 获取每个关键步骤,并输出到页面
                var s = [];
                for (var i = 0; i < route.getNumSteps() ; i++) {
                    var step = route.getStep(i);
                    s.push((i + 1) + '. ' + step.getDescription());
                }
                //document.getElementById('r-result').innerHTML = s.join('<br/>');
            }
        }
    };
    var driving = new BMap.DrivingRoute(map, options);
    driving.search('天安门', '百度大厦');
}

function GetInnerText() {
    return document.getElementById('r-result').innerText;
}

function Remove() {
    document.getElementById('r-result').innerText = '';
}

//返回指定坐标所在地址
function GetByPoint(lng, lat) {
    var gc = new BMap.Geocoder();
    pt = new BMap.Point(lng, lat);
    gc.getLocation(pt, function (rs) {
        var addComp = rs.addressComponents;
        document.getElementById('geo').innerText = addComp.province + ', ' + addComp.city + ', ' + addComp.district + ', ' + addComp.street + ', ' + addComp.streetNumber;
    });
}

//返回指定地址的坐标
function GetByAddress(geo) {
    //通过IP定位获取当前城市名称
    IP();
    var cityName = document.getElementById('geo').innerText;
    var myGeo = new BMap.Geocoder();
    // 将地址解析结果显示在地图上,并调整地图视野
    myGeo.getPoint(geo, function (point) {
        if (point) {
            map.centerAndZoom(point, 16);
            map.addOverlay(new BMap.Marker(point));
            document.getElementById('lng').innerText = point.lng;
            document.getElementById('lat').innerText = point.lat;
        }
    }, cityName);
}

//IP定位
function IP() {
    var myCity = new BMap.LocalCity();
    myCity.get(myFun);
    function myFun(result) {
        var cityName = result.name;
        document.getElementById('geo').innerText = cityName;
        map.setCenter(cityName);
    }

}
//*
//*结束*//

//*地图工具*//

//开启地图测距工具
function DistanceToolOpen() {
    var distance = new BMapLib.DistanceTool(map);//测距组件
    distance.open();
}

//关闭地图测距工具
function DistanceToolOpen() {
    distance.close();
}
//开启地图拖拽放大工具
function DragAndZoomOpen() {
    var drag = new BMap.DragAndZoomTool(map);
    drag.open();
}


//关闭地图拖拽放大工具
function DragAndZoomOpen() {
    drag.close();
}

//开启地图绘制工具
function DrawingManagerOpen() {

    //实例化鼠标绘制工具
    var drawingManager = new BMapLib.DrawingManager(map, {
        isOpen: true, //是否开启绘制模式
        enableDrawingTool: true, //是否显示工具栏
        drawingToolOptions: {
            anchor: BMAP_ANCHOR_BOTTOM_RIGHT, //位置
            offset: new BMap.Size(5, 5), //偏离值
            scale: 0.8 //工具栏缩放比例
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
}

//关闭地图绘制工具
function DrawingManagerClose() {
    drawingManager.close();
}
//*
//*结束*//

function DownloadPoint(startPoint, endPoint) {
    alert("ok");
    var driving = new BMap.DrivingRoute(map);    //驾车实例
    driving.search(startPoint, endPoint);
    driving.setSearchCompleteCallback(function () {
        var pts = driving.getResults().getPlan(0).getRoute(0).getPath();    //通过驾车实例，获得一系列点的数组
        var paths = pts.length;    //获得有几个点
        return pts;
    });
}

//*地图事件*//