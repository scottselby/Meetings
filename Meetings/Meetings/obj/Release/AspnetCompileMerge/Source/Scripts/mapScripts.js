





// _.-=~^*'`"||  START OLD SCRIPTS ||"


//globals go here
var map, marker, doubleMarker, doubleMarkerInt,bounds ;
var googleMarkerPoints = [];
var googleLatLngArray = [];
var numberPattern = /\d+/g;


window.onload = function () {
    geoLocateMe();
};

function initializeMap(foundMe, latlong) {

    if (!foundMe) {
        //if geolocation fails center map at State & Madison - middle of Chicago       
        latlong = new google.maps.LatLng(41.882039, -87.627813);
    }

    //set map options
    var mapOptions = {
        zoom: 12,
        center: latlong,
        navigationControlOptions: { style: google.maps.NavigationControlStyle.SMALL },
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    //create map
    map = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);

    //create first marker - this one is blue for user's location or the Chicago default    
    var MyLocationmarker = new google.maps.Marker({
        position: latlong,
        map: map,
        title: "My Current Location",
        icon: new google.maps.MarkerImage("http://labs.google.com/ridefinder/images/mm_20_blue.png"),
        zIndex: 1000
    });

    google.maps.Map.prototype.clearOverlays = function () {
        for (var i = 0; i < googleMarkerPoints.length; i++) {
            googleMarkerPoints[i].setMap(null);
        }
        googleMarkerPoints.length = 0;
    }
   // AddMarkers();
}

function geoLocateMe() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(geolocateSuccess, error);
    } else {
        error('not supported');
    }
}

function geolocateSuccess(position) {
    var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
    initializeMap(true, latlng);
}

function error(msg) {
    // TODO: obivously need to do more here - graceful fallback to ip locating , and better alert to user
    alert(msg);
    alert('If you allow this site to locate you then the default will be meetings around your location in the next 5 hours!');
    return;
}

$(function () {
    //This makes the appropiate marker bouce on mouseover of list of meetings on right side of map
    //TODO: show user apropiate marker some other way then this stupid bounce
    $('.search-results').on('mouseenter', '.resultListItem', function () {
        var indexNum = parseInt($(this).attr('data-num'));
        googleMarkerPoints[indexNum].setAnimation(google.maps.Animation.BOUNCE);
    });
    $('.search-results').on('mouseleave', '.resultListItem', function () {
        var indexNum = parseInt($(this).attr('data-num'));
        googleMarkerPoints[indexNum].setAnimation(null);
    });
});

function AddMarkers() {

    $.ajax({
        url: "JSON/GetMarkers",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                //add marker to map for each result
                var obj = data[i];
                var latitude = obj.Latitude;
                var longitude = obj.Longitude;
                var title = obj.Title;
                var thisLatlng = new google.maps.LatLng(latitude, longitude);
                var thismarker = new google.maps.Marker({
                    position: thisLatlng,
                    map: map,
                    title: title,
                    icon: new google.maps.MarkerImage("http://labs.google.com/ridefinder/images/mm_20_red.png")
                });

                //add to list on right side of map for each result
                var clone = $('.template').clone().removeClass('template');

                googleMarkerPoints.push(thismarker);
            }
            for (var i = 0; i <= 6; i++) {
                (function (i) {
                    google.maps.event.addListener(googleMarkerPoints[i], 'click', function () {
                        $('.result').css({ "background-color": "#fff" });
                        var selector = "div[data-num='" + i + "']";
                        $(selector).css({ "background-color": "#999" });
                    });
                })(i)
            }
        }
    });
};

function codeAddress(address) {
    $('.loading, .loadingLeft').show();
    geocoder = new google.maps.Geocoder();
    bounds = new google.maps.LatLngBounds();
    // send Geocode request
    geocoder.geocode({ 'address': address }, function (results, status) {
        // response succcess
        if (status == google.maps.GeocoderStatus.OK) {
            // ask user if returned formatted_address is correct
          //  if (confirm('Found similar address:"' + results[0].formatted_address + '",  Is this correct?')) {
                // alert('Latitude: ' + results[0].geometry.location.lat() + ', Longitude: ' + results[0].geometry.location.lng());
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location,
                    icon: new google.maps.MarkerImage("http://labs.google.com/ridefinder/images/mm_20_blue.png"),
                    title: results[0].formatted_address
                });
                bounds.extend(results[0].geometry.location);
                map.setZoom(14);
                $('#Longitude').val(results[0].geometry.location.lng());
                $('#Latitude').val(results[0].geometry.location.lat());
                $('#formatted_address').val(results[0].formatted_address);
                $('#Location').val(results[0].formatted_address)
                finishSearch();
         //   }
         //   else { return false; }
        } else {
            alert('Geocode was not successful for the following reason: ' + status);
            $('.loading, .loadingLeft').hide();
        }
    });
}

function finishSearch() {
    var longitude = $('#Longitude').val();
    var latitude = $('#Latitude').val();
    var radius = $('#Radius').val();
    $.ajax({
        url: '/Home/GetMeetingsInRadius',
        type: 'POST',
        data: { 'latitude': latitude, 'longitude': longitude, 'radius': radius },
        success: function (data) {
            $('.search-results').html('');
            for (var i = 0; i < data.length; i++) {
                var template = $('._template').clone().removeClass('_template');
                var time = formatAMPM(data[i].Time);
                template.attr('data-num', i);
                template.attr('data-Location', data[i].LocationName)
                template.find('.Name').html(data[i].MeetingName);
                template.find('.location-name').html(data[i].LocationName);
                template.find('.Address').html(data[i].Address);
                template.find('.time').html(time);
                template.find('.day-of-week').html(GetDayOfWeek(data[i].DayOfWeek));
                template.find('.distance').html((Math.round(data[i].distance * 100) / 100) + " miles");
                $('.search-results').append(template);

                var latitude = data[i].Latitude;
                var longitude = data[i].Longitude;
                var title = data[i].MeetingName + " - " + time;
                var thisLatlng = new google.maps.LatLng(latitude, longitude);

                // if there is already a Marker at this location 
                var googleMarkerPointsThatOverlap = [];
                for (var q = 0; q < googleMarkerPoints.length; q++) {
                    if (thisLatlng.lat() == googleMarkerPoints[q].position.lat()) {
                        if (thisLatlng.lng() == googleMarkerPoints[q].position.lng()) {
                            googleMarkerPointsThatOverlap.push(googleMarkerPoints[q]);
                            title = googleMarkerPointsThatOverlap[googleMarkerPointsThatOverlap.length - 1].title + ", " + title;
                        }
                    }
                }

                var thismarker = new google.maps.Marker({
                    position: thisLatlng,
                    map: map,
                    title: title,
                    icon: new google.maps.MarkerImage("http://labs.google.com/ridefinder/images/mm_20_red.png")
                });
                bounds.extend(thisLatlng);
                googleMarkerPoints.push(thismarker);
                (function (i) {                 
                    google.maps.event.addListener(googleMarkerPoints[i], 'click', function () {
                        $('.resultListItem').css({ "background-color": "#fff" });
                        var selector = "li[data-Location='" + data[i].LocationName + "']";
                        $(selector).css({ "background-color": "#aaa" });
                    });
                })(i)
            }
            map.fitBounds(bounds);
            $('.loadingLeft, .loading').hide();  
        }
      
    });
}
$(function () {
    $('#DayOfWeek').val('1').prop('disabled', true);
        $('.loading, .loadingLeft').hide();
    $('#searchSubmit').click(function () {
        codeAddress();
        return false;
    });
    $(document).on('click', '#SearchSubmit', function () {
        var Address = $('#Location').val();
        if (!Address || Address.length <= 2) {
            alert('Please Enter a valid Location.');
            return false;
        }
        map.clearOverlays();
        codeAddress(Address);
        
    });
});

function formatAMPM(date) {
    date = date.match(numberPattern);
    var utcSeconds = parseInt(date);
    var d = new Date(0); // The 0 there is the key, which sets the date to the epoch
    d.setUTCSeconds(utcSeconds);
    var hoursMin = new Date(d).toString("HH:mm");
    var hours = hoursMin.split(':')[0];
    var min = hoursMin.split(':')[1];
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    var strTime = hours + ':' + min + ' ' + ampm;
    return strTime;
}

function GetDayOfWeek(number){
    if(number == 0){return "Sunday";}
    else if(number == 1){return "Monday";}
    else if(number == 2){return "Tuesday";}
    else if(number == 3){return "Wednesday";}
    else if(number == 4){return "Thursday";}
    else if(number == 5){return "Friday";}
    else if(number == 6){return "Saturday";}
    else{return "Fuck You Day!";}
}

