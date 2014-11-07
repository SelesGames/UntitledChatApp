var calc = '<form id="frm" action="" onsubmit="launch(); return false"><div id="upperleft"><div id="radio1"><input class="even" type="radio" name="radr" checked="checked" onclick="switchLoc()" />Address&nbsp;<input class="even" type="radio" name="radr" onclick="switchLoc()" />Multiple input&nbsp;<input class="even" type="radio" name="radr" onclick="switchLoc()" />Latitude/longitude</div><br /><div id="DA"><label for="address0">Address or city:</label><br /><input type="text" id="address0" name="address" size="55" maxlength="60" onfocus="this.select()" /></div><div id="DA2" class="off"><label for="address1">Addresses (Each on a separate line):</label><br /><textarea id="address1" name="address" rows="2" cols="0" style="width: 26.5em" onfocus="this.select()"></textarea></div><div id="DL" class="off"><div class="DFI"><label for="latitude" accesskey="L">Latitude(s):</label><br /><textarea id="latitude" cols="16" rows="2" onfocus="this.select()"></textarea>&nbsp;&nbsp;</div><div class="DMA"><label for="longitude">Longitude(s):</label><br /><textarea id="longitude" cols="17" rows="2" onfocus="this.select()"></textarea></div></div><div id="DR" class="off"><label id="resultslabel" for="results">0 search results:</label><br /><div id="DRF"><select id="results"><option></option></select></div><br style="clear: both" /></div><div id="DB" class="fleft"><input id="add" type="submit" class="btn2" value="Add" accesskey="I" /><input type="button" class="btn2" value="Remove" accesskey="X" onclick="removeOptionSelected()" /><input type="button" class="btn2" value="Clear all" accesskey="0" onclick="clearAll()" /></div><div id="DB2" class="fleft"><input id="add2" type="button" class="btn2" value="Continue" onclick="contin()" /><input type="button" class="btn2" value="Cancel" onclick="cancelGeocode()" /></div><div id="DB3" class="fleft"><input id="ok" type="button" class="btn2" value="Ok" onclick="ok1()" /><input type="button" class="btn2" value="Skip" onclick="skip()" /><input type="button" class="btn2" value="Cancel" onclick="cancelGeocode()" /></div><div id="msg"></div></div><div id="upperright"><div id="radio2"><input class="even" type="radio" id="w0" name="radw" checked="checked" onclick="switchWeight()" />Weight by time&nbsp;&nbsp;<input class="even" type="radio" id="w1" name="radw" onclick="switchWeight()" />Other weight</div><br /><div id="DIC"><div id="DT"><div class="DFI"><label for="years0" accesskey="Y">Years:</label><br /><input type="text" id="years0" name="years" size="5" maxlength="5" onfocus="this.select()" /></div><div class="DFI"><label for="months0">Months:</label><br /><input type="text" id="months0" name="months" size="5" maxlength="5" onfocus="this.select()" /></div><div class="DMA"><label for="days0">Days:</label><br /><input type="text" id="days0" name="days" size="5" maxlength="5" onfocus="this.select()" /></div></div><div id="DT2" class="off"><div class="DFI"><label for="years1" accesskey="Y">Years:</label><br /><textarea id="years1" name="years" rows="2" cols="0" style="width: 4em" onfocus="this.select()"></textarea></div><div class="DFI"><label for="months1">Months:</label><br /><textarea id="months1" name="months" rows="2" cols="0" style="width: 4em" onfocus="this.select()"></textarea></div><div class="DMA"><label for="days1">Days:</label><br /><textarea id="days1" name="days" rows="2" cols="0" style="width: 4em" onfocus="this.select()"></textarea></div></div><div id="DW" class="off"><label for="weight0" accesskey="W">Weight:</label><br /><input type="text" id="weight0" name="weight" size="15" maxlength="15" onfocus="this.select()" /></div><div id="DW2" class="off"><label for="weight1" accesskey="W">Weight:</label><br /><textarea id="weight1" name="weight" rows="2" cols="15" onfocus="this.select()"></textarea> </div><a href="javascript:triggerMid()"><img id="micon" src="files/micon.jpg" alt="Midpoint info"></img></a><span>Leave blank for no weight</span></div></div><div class="DCL"></div><div id="map"></div><div id="DLB"><div id="DE" class="off"></div><div id="DP"><label id="placeslabel" for="places" accesskey="P">Your places:</label><br /><select id="places" size="8" onchange="openPlace()"><option>.</option><option>.</option><option>.</option></select></div><br /><br /><input type="checkbox" id="disp" checked="checked" />Display place markers<br /><br /><label for="method">Calculation method:</label><br /><div id="radio3"><input id="method" type="radio" name="method" checked="checked" onclick="changeMethod()" />Midpoint (Center of gravity)<br /><input type="radio" name="method" onclick="changeMethod()" />Center of minimum distance<br /> <input type="radio" name="method" onclick="changeMethod()" />Average latitude/longitude</div><br /><br /><input type="checkbox" id="large" onclick="switchMap()" />Larger map&nbsp;<input type="button" class="btn" value="Save map" onclick="save(0)" /></div></form>';

var map, geocoder, MM;
var cAddress = document.getElementsByName("address");
var cYear = document.getElementsByName("years");
var cMonth = document.getElementsByName("months");
var cDay = document.getElementsByName("days");
var cWeight = document.getElementsByName("weight");
var addresses = new Array();
var years = new Array();
var months = new Array();
var days = new Array();
var lats = new Array();
var lons = new Array();
var ck = new Array();
var mTxt = ["Geographic midpoint", "Center of minimum distance", "Average latitude/longitude"];
var f1, request, addressIndex, pause, cancel, parlat, parlng, sameMap = 0;
var par = 0;
var rI, wI, cI;
var rad90 = rad(90);
var rad180 = rad(180);
var M = Math;
var p, infoWindow, geoOverLimit;
document.write(calc);

function initialize() {
    f1 = D("frm");
    p = f1.places;
    p.length = 0;
    infoWindow = new google.maps.InfoWindow({
        content: ""
    });
    rI = 0;
    wI = 0;
    cI = 0;
    var n = "",
        g = "",
        h = "",
        v = "",
        o = "",
        l = "",
        t = "",
        k = "",
        u = "",
        A = "",
        b = "",
        f;
    var E = window.location.search.substring(1);
    E = decodeURI(E.replace(/\+/gi, " "));
    var a = E.split("&");
    if (a.length > 0) {
        for (j = 0; j < a.length; j++) {
            var C = a[j].split("=");
            var e = C[1];
            switch (C[0]) {
                case "ml":
                    parlat = e;
                    break;
                case "mn":
                    parlng = e;
                    break;
                case "l":
                    lats = e.split("|");
                    break;
                case "n":
                    lons = e.split("|");
                    break;
                case "a":
                    addresses = e.split("|");
                    break;
                case "y":
                    n = e.replace(/\|/g, "\n");
                    break;
                case "m":
                    g = e.replace(/\|/g, "\n");
                    break;
                case "d":
                    h = e.replace(/\|/g, "\n");
                    break;
                case "x":
                    t = 1;
                    f1.large.checked = (e == "1");
                    switchMap();
                    break;
                case "cl":
                    v = e;
                    break;
                case "cn":
                    o = e;
                    break;
                case "z":
                    l = parseInt(e);
                    break;
                case "c":
                    k = 1;
                    f1.method[e].checked = true;
                    cI = e;
                    break;
                case "p":
                    A = 1;
                    f1.disp.checked = (e == "1");
                    break;
                case "w":
                    f1.radw[e].checked = true;
                    switchWeight();
                    break;
                case "r":
                    f1.radr[e].checked = true;
                    switchLoc();
                    break
            }
        }
    }
    readCookie("ckData1");
    if (u == "" && ck[2] >= 0 && ck[2] <= 1) {
        wI = ck[2];
        f1.radw[wI].checked = true;
        switchWeight()
    }
    if (b == "" && ck[1] >= 0 && ck[1] <= 2) {
        rI = ck[1];
        f1.radr[rI].checked = true
    }
    switchLoc();
    if (k == "" && ck[3] >= 0 && ck[3] <= 2) {
        cI = ck[3];
        f1.method[cI].checked = true
    }
    if (A == "") {
        f1.disp.checked = (ck[4] == 1 || isNaN(ck[4]))
    }
    if (t == "") {
        f1.large.checked = (ck[0] == "1");
        switchMap()
    }
    if (v != "" && o != "" && v >= -90 && v <= 90 && o >= -180 && o <= 180) {
        if (l == "") {
            l = 3
        }
        f = new google.maps.LatLng(v, o)
    } else {
        if (!isNaN(ck[5]) && !isNaN(ck[6])) {
            f = new google.maps.LatLng(ck[5], ck[6]);
            l = ck[7] * 1
        } else {
            f = new google.maps.LatLng(39.17, -98.297);
            l = 3
        }
    }
    geocoder = new google.maps.Geocoder();
    var B = {
        zoom: l,
        center: f,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(D("map"), B);
    if (lats.length) {
        par = 1;
        years = getInput(n, lats.length);
        months = getInput(g, lats.length);
        days = getInput(h, lats.length);
        launch(1)
    } else {
        if (parlat && parlng) {
            par = 1;
            calculate()
        }
    }
    if ((MM || p.length) && (v == "" || o == "")) {
        setBounds()
    }
}

function unload() {
    setCookie("ckData1", f1.large.checked * 1, rI + f1.radr[2].checked * 1, wI, cI, f1.disp.checked * 1, map.getCenter().lat(), map.getCenter().lng(), map.getZoom())
}

function readCookie(e) {
    var b = "" + document.cookie;
    var d = b.indexOf(e);
    if (d == -1 || e == "") {
        return ""
    }
    var a = b.indexOf(";", d);
    if (a == -1) {
        a = b.length
    }
    var c = unescape(b.substring(d + e.length + 1, a));
    ck = c.split("|")
}

function setCookie(k, m, a, n, h, q, o, l, g) {
    var f = 2500;
    var e = new Date();
    var d = new Date();
    d.setTime(e.getTime() + 3600000 * 24 * f);
    var b = m + "|" + a + "|" + n + "|" + h + "|" + q + "|" + o + "|" + l + "|" + g;
    document.cookie = k + "=" + escape(b) + ";expires=" + d.toGMTString()
}

function selectText(a) {
    if (window.getSelection) {
        var c = window.getSelection();
        if (c.setBaseAndExtent) {
            c.setBaseAndExtent(a, 0, a, 1)
        } else {
            var b = document.createRange();
            b.selectNodeContents(a);
            c.removeAllRanges();
            c.addRange(b)
        }
    } else {
        var b = document.body.createTextRange();
        b.moveToElementText(a);
        b.select()
    }
}

function setBounds() {
    var b = new google.maps.LatLngBounds();
    var a;
    if (p.length || MM) {
        if (MM) {
            a = MM.getPosition();
            b.extend(a)
        }
        for (i = 0; i < p.length; i++) {
            var a = new google.maps.LatLng(p[i].marker.getPosition().lat(), p[i].marker.getPosition().lng());
            b.extend(a)
        }
        mapLoaded = false;
        map.fitBounds(b);
        if (map.getZoom() > 15) {
            map.setZoom(15)
        }
    }
}

function createMarker(a, c, f, h) {
    var e = null,
        k = null,
        g = true;
    if (f) {
        e = "images/paleblue_MarkerM.png";
        k = {
            url: "images/shadow50.png",
            size: new google.maps.Size(37, 34),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(10, 34)
        }
    } else {
        if (!f1.disp.checked) {
            g = false
        }
    }
    var b = new google.maps.Marker({
        position: a,
        map: map,
        icon: e,
        shadow: k,
        visible: g
    });
    if (h) {
        b.setDraggable(true)
    }
    google.maps.event.addListener(b, "click", function () {
        var d = "";
        if (!f) {
            if (b.dragged != 1) {
                d = formatInfo(splitAddress(p[b.i].text), "x", "", b.i)
            } else {
                d = formatInfo("Current location:", b.getPosition().lat(), b.getPosition().lng(), b.i)
            }
        } else {
            d = c
        }
        infoWindow.content = d;
        infoWindow.open(map, b)
    });
    google.maps.event.addListener(b, "dragstart", function () {
        saveLatLng(b.i, b.getPosition());
        infoWindow.close()
    });
    google.maps.event.addListener(b, "dragend", function () {
        sameMap = 1;
        calculate();
        b.dragged = 1
    });
    return b
}

function clearAll() {
    par = 0;
    infoWindow.close();
    dispMsg("Clearing...", 2);
    clear2()
}

function clear2() {
    D("DE").style.display = "none";
    for (i = p.length - 1; i >= 0; i--) {
        p[i].marker.setMap(null)
    }
    MM = remove(MM);
    p.length = 0;
    dispCount();
    clearGeocode();
    clearWeights();
    lockWeights();
    addresses.length = 0;
    lats.length = 0;
    lons.length = 0;
    years.length = 0;
    months.length = 0;
    days.length = 0;
    dispMsg("", 2)
}

function clearGeocode() {
    cAddress[0].value = "";
    cAddress[1].value = "";
    f1.latitude.value = "";
    f1.longitude.value = ""
}

function clearWeights() {
    for (i = 0; i < 2; i++) {
        cYear[i].value = "";
        cMonth[i].value = "";
        cDay[i].value = "";
        cWeight[i].value = ""
    }
    years.length = 0;
    months.length = 0;
    days.length = 0
}

function removeOptionSelected() {
    par = 0;
    infoWindow.close();
    var a = p.selectedIndex;
    dispMsg("", 2);
    D("DE").style.display = "none";
    if (a >= 0) {
        p[a].marker.setMap(null);
        p.remove(a);
        if (p.length == 0) {
            clearAll();
            return false
        } else {
            if (p.length == 1) {
                if (MM) {
                    MM = remove(MM)
                }
            }
        }
        for (j = a; j < p.length; j++) {
            p[j].marker.i = j
        }
        dispCount();
        calculate()
    }
}

function appendToList() {
    dispMsg("Adding...", addresses.length);
    var e = f1.results;
    var d = M.max(e.selectedIndex, 0);
    var c = e[d].text;
    if (MM) {
        MM = remove(MM)
    }
    appendOptionLast("places", c);
    var b = p.length - 1;
    p[b].y = years[addressIndex];
    p[b].m = months[addressIndex];
    p[b].d = days[addressIndex];
    var a = new google.maps.LatLng(roundx(e[d].lat, 6), roundx(e[d].lng, 6));
    p[b].marker = createMarker(a, "", null, 1);
    p[b].marker.i = b;
    dispCount();
    if (!b) {
        lockWeights()
    }
}

function calculate() {
    if (p.length > 1 || par) {
        var midlat = 0,
            midlng = 0;
        var x = 0;
        var y = 0;
        var z = 0;
        var x1, y1, z1;
        var pt = new Object();
        pt.lat = 0;
        pt.lon = 0;
        var totdays = 0;
        var lats1 = new Array();
        var lons1 = new Array();
        var days1 = new Array();
        var sinlats = new Array();
        var coslats = new Array();
        with (Math) {
            for (i = 0; i < p.length; i++) {
                lats1[i] = rad(p[i].marker.getPosition().lat());
                lons1[i] = rad(p[i].marker.getPosition().lng());
                sinlats[i] = sin(lats1[i]);
                coslats[i] = cos(lats1[i]);
                days1[i] = p[i].y * 365.25 + p[i].m * 30.4375 + p[i].d * 1;
                if (days1[i] == 0) {
                    days1[i] = 1
                }
                totdays = totdays + days1[i];
                x1 = coslats[i] * cos(lons1[i]);
                y1 = coslats[i] * sin(lons1[i]);
                z1 = sinlats[i];
                x += x1 * days1[i];
                y += y1 * days1[i];
                z += z1 * days1[i]
            }
            x = x / totdays;
            y = y / totdays;
            z = z / totdays;
            midlng = atan2(y, x);
            hyp = sqrt(x * x + y * y);
            midlat = atan2(z, hyp);
            if (cI != 2 && abs(x) < 1e-9 && abs(y) < 1e-9 && abs(z) < 1e-9) {
                if (MM) {
                    MM = remove(MM)
                }
                displayError("The midpoint is the center of the earth.")
            } else {
                if (cI == 2) {
                    y = 0;
                    x = 0;
                    for (i = 0; i < lats1.length; i++) {
                        y = y + lats1[i] * days1[i];
                        x = x + normalizeLongitude(lons1[i] - midlng) * days1[i]
                    }
                    midlat = y / totdays;
                    midlng = normalizeLongitude(x / totdays + midlng)
                } else {
                    if (cI == 1) {
                        if (lats1.length > 2 || lats1.length == 2 & days1[0] != days1[1]) {
                            var tries = 0;
                            lats1[lats1.length] = midlat;
                            lons1[lons1.length] = midlng;
                            var distrad = rad90;
                            var mindist = 10000000;
                            var sum, gMindist, lat2, slat, cdist, minlat, minlon;
                            var t = new Array(8, 6, 7, 2, 0, 1, 5, 3, 4);
                            var scale = new Array(0.7071, 0.7071, 1, 0.7071, 0.7071, 1, 1, 1, 1);
                            var testcenter = true;
                            i = lats1.length + 8;
                            while (distrad > 2e-8 && tries < 5000) {
                                if (i < 0) {
                                    i = 8
                                }
                                while (i >= 0) {
                                    if (i < 9) {
                                        y = floor(t[i] / 3) - 1;
                                        x = t[i] % 3;
                                        switch (x) {
                                            case 1:
                                                pt.lon = midlng;
                                                pt.lat = midlat - y * distrad;
                                                pt = normalizeLatitude(pt);
                                                break;
                                            case 0:
                                                pt.lon = midlng;
                                                pt.lat = midlat - y * distrad * scale[i];
                                                pt = normalizeLatitude(pt);
                                                lat2 = pt.lat;
                                                slat = sin(lat2);
                                                cdist = cos(distrad * scale[i]);
                                                pt.lat = asin(slat * cdist);
                                                pt.lon = normalizeLongitude(pt.lon + atan2(-sin(distrad * scale[i]) * cos(lat2), cdist - slat * sin(pt.lat)));
                                                break;
                                            case 2:
                                                pt.lon = normalizeLongitude(midlng + normalizeLongitude(midlng - pt.lon))
                                        }
                                    } else {
                                        pt.lat = lats1[i - 9];
                                        pt.lon = lons1[i - 9]
                                    }
                                    if (pt.lon != midlng || pt.lat != midlat || testcenter) {
                                        sum = 0;
                                        for (j = 0; j < lats1.length - 1; j++) {
                                            sum += acos(sinlats[j] * sin(pt.lat) + coslats[j] * cos(pt.lat) * cos(pt.lon - lons1[j])) * days1[j]
                                        }
                                        if (!testcenter) {
                                            if (sum < mindist) {
                                                mindist = sum;
                                                minlat = pt.lat;
                                                minlon = pt.lon
                                            }
                                        } else {
                                            gMindist = sum;
                                            testcenter = false
                                        }
                                    }
                                    i--
                                }
                                if (mindist - gMindist < -4e-14) {
                                    midlat = minlat;
                                    midlng = minlon;
                                    gMindist = mindist
                                } else {
                                    distrad = distrad * 0.5
                                }
                                tries++
                            }
                        }
                    }
                }
                if (!par) {
                    midlat = deg(midlat);
                    midlng = deg(midlng)
                } else {
                    midlat = parlat;
                    midlng = parlng
                }
                par = 0;
                if (MM) {
                    MM = remove(MM)
                }
                var point = new google.maps.LatLng(midlat, midlng);
                var h1 = formatInfo("<b>" + mTxt[cI] + "</b>", midlat, midlng, -1);
                var h2 = '<p class="pz"><a href="javascript:save(1)">Find nearby points of interest</a></p></div>';
                MM = createMarker(point, h1 + h2, 1, 0);
                MM.setMap(map);
                MM.h1 = h1;
                MM.h2 = h2;
                geocoder.geocode({
                    latLng: point
                }, revGeoCallback)
            }
            if (tries >= 5000) {
                displayError("The center of distance for these " + p.length + " places could not be precisely located. The displayed center of distance is probably accurate to within two degrees.")
            }
        }
    }
    if (!par && !sameMap) {
        setBounds()
    }
    sameMap = 0
}

function saveLatLng(a, b) {
    if (isNaN(p[a].lat)) {
        p[a].lat = b.lat();
        p[a].lng = b.lng()
    }
}

function reset(b) {
    infoWindow.close();
    p[b].marker.dragged = 0;
    var a = new google.maps.LatLng(p[b].lat, p[b].lng);
    p[b].marker.setPosition(a);
    calculate()
}

function r5(a) {
    p.selectedIndex = a;
    removeOptionSelected()
}

function remove(a) {
    if (a) {
        a.setMap(null)
    }
    return null
}

function appendOptionLast(d, b) {
    var c = document.createElement("option");
    c.text = b;
    c.value = b;
    var e = D(d);
    try {
        e.add(c, null)
    } catch (a) {
        e.add(c)
    }
}

function dispCount() {
    var b = p.length;
    var a = "s";
    switch (b) {
        case 1:
            a = "";
            break;
        case 0:
            b = ""
    }
    D("placeslabel").innerHTML = "Your " + b + " place" + a + ":"
}

function dispMsg(b, a) {
    if (a > 1) {
        D("msg").innerHTML = b
    }
}

function displayError(a) {
    D("DE").innerHTML = a;
    toggleDivs(["DE"], 1)
}

function rad(a) {
    return (a * Math.PI / 180)
}

function deg(a) {
    return (a * 180 / Math.PI)
}

function splitAddress(b) {
    var a = b.split(/,/);
    if (a.length > 3 || a.length > 2 && /\d/g.test(a[0])) {
        b = a[0] + "<br>";
        for (j = 1; j < a.length; j++) {
            if (j > 1) {
                b += ", "
            }
            b += trim(a[j])
        }
    }
    return b
}

function normalizeLongitude(a) {
    var b = Math.PI;
    if (a > b) {
        a = a - 2 * b
    } else {
        if (a < -b) {
            a = a + 2 * b
        }
    }
    return a
}

function normalizeLatitude(a) {
    if (Math.abs(a.lat) > rad90) {
        a.lat = rad180 - a.lat - 2 * rad180 * (a.lat < -rad90);
        a.lon = normalizeLongitude(a.lon - rad180)
    }
    return a
}

function trim(a) {
    if (a.charCodeAt(0) > 32 && a.charCodeAt(a.length - 1) > 32) {
        return a
    } else {
        return a.replace(/^\s+|\s+$/g, "")
    }
}

function rTrim(a) {
    if (a.charCodeAt(a.length - 1) > 32) {
        return a
    } else {
        return a.replace(/\s+$/g, "")
    }
}

function roundx(b, a) {
    return M.round(b * M.pow(10, a)) / M.pow(10, a)
}

function lockWeights() {
    f1.w0.disabled = (p.length > 0);
    f1.w1.disabled = (p.length > 0)
}

function toggleDivs(a, b) {
    for (i = 0; i < a.length; i++) {
        if (i < b) {
            D(a[i]).style.display = "block"
        } else {
            D(a[i]).style.display = "none"
        }
    }
}

function D(a) {
    return document.getElementById(a)
}

function switchLoc() {
    var a = rI;
    if (f1.radr[0].checked) {
        toggleDivs(["DA", "DB", "DA2", "DR", "DB2", "DB3", "DL"], 2);
        rI = 0
    } else {
        if (f1.radr[1].checked) {
            toggleDivs(["DA2", "DB", "DA", "DR", "DB2", "DB3", "DL"], 2);
            rI = 1
        } else {
            toggleDivs(["DL", "DB", "DA", "DA2", "DR", "DB2", "DB3"], 2);
            rI = 1
        }
    }
    if (a != rI) {
        clearGeocode()
    }
    switchWeight()
}

function switchWeight() {
    var a = wI;
    if (f1.radw[0].checked) {
        if (f1.radr[0].checked) {
            toggleDivs(["DT", "DT2", "DW", "DW2"], 1)
        } else {
            toggleDivs(["DT2", "DT", "DW", "DW2"], 1)
        }
        wI = 0
    } else {
        if (f1.radr[0].checked) {
            toggleDivs(["DW", "DW2", "DT", "DT2"], 1)
        } else {
            toggleDivs(["DW2", "DW", "DT", "DT2"], 1)
        }
        wI = 1
    }
    if (a != wI) {
        clearWeights()
    }
}

function changeMethod() {
    infoWindow.close();
    if (f1.method[2].checked) {
        cI = 2
    } else {
        if (f1.method[1].checked) {
            cI = 1
        } else {
            cI = 0
        }
    }
    if (p.length > 1) {
        calculate()
    }
}

function switchMap() {
    if (f1.large.checked) {
        D("map").style.width = "48em";
        D("map").style.height = "29.4em"
    } else {
        D("map").style.width = "30.1em";
        D("map").style.height = "24.1em"
    }
    if (map) {
        google.maps.event.trigger(map, "resize")
    }
    if (p.length > 0) {
        setBounds()
    }
}

function getTimes(a) {
    years = getInput(cYear[rI].value, a);
    months = getInput(cMonth[rI].value, a);
    if (wI == 1) {
        days = getInput(cWeight[rI].value, a)
    } else {
        days = getInput(cDay[rI].value, a)
    }
}

function getInput(c, a) {
    var b = rTrim(c);
    b = b.replace(/\r\n/g, "\n");
    var d = b.split("\n");
    if (a) {
        if (d.length != a) {
            d.length = a;
            c = d.join().replace(/undefined/g, "");
            d = c.split(",")
        }
    } else {
        if (b.length == 0) {
            d.length = 0
        }
    }
    return d
}

function validateTimes(e, d) {
    var c = "",
        b = ["time", "weight"];
    var g = ["day", "weight"];
    if (p.length) {
        var f = !p[0].y && !p[0].m && !p[0].d
    } else {
        var f = !parseFloat(years[0]) && !parseFloat(months[0]) && !parseFloat(days[0])
    }
    for (i = 0; i < d; i++) {
        if (e == "address") {
            c = addresses[i]
        }
        if (isNaN(years[i])) {
            displayError("The year '" + years[i] + "' for " + e + " #" + parseInt(i + 1) + " " + c + " is invalid.");
            return
        }
        if (isNaN(months[i])) {
            displayError("The month '" + months[i] + "' for " + e + " #" + parseInt(i + 1) + " " + c + " is invalid.");
            return
        }
        if (isNaN(days[i])) {
            displayError("The " + g[wI] + " '" + days[i] + "' for " + e + " #" + parseInt(i + 1) + " " + c + " is invalid.");
            return
        }
        years[i] = +years[i];
        months[i] = +months[i];
        days[i] = +days[i];
        cur = !years[i] && !months[i] && !days[i];
        if (!f && cur) {
            displayError("A " + b[wI] + " must be specified for " + e + " #" + parseInt(i + 1) + " " + addresses[i] + ".");
            return false
        }
        if (f && !cur) {
            displayError("You must either enter a time for all locations in Your Places, or leave the time blank or zero for all locations.");
            return false
        }
    }
    return true
}

function launch(b) {
    if (!b) {
        par = 0
    }
    var a;
    if (!map) {
        return
    }
    infoWindow.close();
    request = -2;
    pause = 0;
    cancel = 0;
    if (f1.radr[2].checked || par) {
        if (!par) {
            lats = getInput(f1.latitude.value, 0);
            lons = getInput(f1.longitude.value, 0);
            getTimes(lats.length)
        }
        if (lats.length != lons.length) {
            displayError("The number of latitudes is not the same as the number of longitudes. Please check your data.");
            return false
        }
        if (lats.length == 0 || lons.length == 0) {
            displayError("You must specify a latitude and longitude before continuing.");
            return false
        }
        for (j = 0; j < lats.length; j++) {
            if (!validateLl(lats[j], 0)) {
                displayError("The entry '" + lats[j] + "' for latitude #" + parseInt(j + 1) + " is invalid.");
                return false
            }
            if (!validateLl(lons[j], 1)) {
                displayError("The entry '" + lons[j] + "' for longitude #" + parseInt(j + 1) + " is invalid.");
                return false
            }
        }
        if (!validateTimes("location", lats.length)) {
            return false
        }
        addressIndex = -1;
        if (lats.length > 1) {
            dispProceed();
            dispMsg("Adding...", 2)
        }
        if (MM) {
            MM = remove(MM)
        }
        launchL();
        return false
    } else {
        addresses = getInput(cAddress[rI].value, 0);
        getTimes(addresses.length);
        if (addresses.length == 0) {
            displayError("You must specify an address before continuing.");
            return false
        }
        if (!validateTimes("address", addresses.length)) {
            return false
        }
    }
    addressIndex = 0;
    if (addresses.length > 1) {
        dispProceed();
        dispMsg("Searching...", 2)
    }
    launchG()
}

function launchL() {
    if (addressIndex >= lats.length - 1 || cancel) {
        clearWeights();
        clearGeocode();
        if (!par || parlat && parlng) {
            calculate()
        }
        dispStart();
        dispMsg("", 2);
        return false
    } else {
        addressIndex++
    }
    if (request >= addressIndex) {
        return false
    }
    request = addressIndex;
    D("DE").style.display = "none";
    if (par) {
        appendOptionLast("places", addresses[addressIndex])
    } else {
        appendOptionLast("places", "Lat: " + lats[addressIndex] + "  Long: " + lons[addressIndex])
    }
    var b = p.length - 1;
    p[b].y = parseFloat(years[addressIndex]);
    p[b].m = parseFloat(months[addressIndex]);
    p[b].d = parseFloat(days[addressIndex]);
    var a = new google.maps.LatLng(latLonToDecimal(lats[addressIndex]), latLonToDecimal(lons[addressIndex]));
    p[b].marker = createMarker(a, "", null, 1);
    p[b].marker.i = b;
    dispCount();
    window.setTimeout(launchL, 15);
    if (!b) {
        lockWeights()
    }
}

function launchG() {
    D("DE").style.display = "none";
    geocoder.geocode({
        address: addresses[addressIndex]
    }, gCallback)
}

function gCallback(c, a) {
    if (cancel || request >= addressIndex) {
        return false
    }
    var d = f1.results;
    if (a != "OK") {
        D("ok").value = "Retry";
        switch (a) {
            case "ZERO_RESULTS":
                displayError("Address #" + parseInt(addressIndex + 1) + " '" + addresses[addressIndex] + "' was not found.");
                break;
            case "OVER_QUERY_LIMIT":
                if (!geoOverLimit) {
                    displayError("Google geocoder speed limit. Click 'Resume' each time calculator stops.");
                    geoOverLimit = 1
                }
                D("ok").value = "Resume";
                break;
            case "REQUEST_DENIED":
                displayError("Request denied");
                break;
            case "INVALID_REQUEST":
                displayError("Invalid request");
                break
        }
        dispMsg("", 2);
        d.length = 0;
        pause = 1;
        toggleDivs(["DB3", "DB", "DB2"], 1);
        return
    }
    request = addressIndex;
    var g, f, e = 0;
    d.length = 0;
    for (i = 0; i < c.length; i++) {
        g = c[i].formatted_address;
        if (!g) {
            g = addresses[addressIndex]
        }
        try {
            if (c[i].types[0] == "street_address") {
                e = g.indexOf(",");
                if (e > -1) {
                    e += 2
                }
            }
        } catch (b) {
            e = -1
        }
        appendOptionLast("results", g);
        d[d.length - 1].lat = c[i].geometry.location.lat();
        d[d.length - 1].lng = c[i].geometry.location.lng();
        d[d.length - 1].i = e
    }
    if (d.length > 0) {
        appendToList()
    } else {
        D("ok").value = "Ok";
        toggleDivs(["DR", "DB3", "DA", "DA2", "DL", "DB", "DE", "DB2"], 2);
        D("resultslabel").innerHTML = "Select from " + d.length + " results:";
        dispMsg("", 2);
        f1.results.focus();
        pause = 1
    }
    loopG()
}

function loopG() {
    if (!pause) {
        if (addressIndex < addresses.length - 1 && !cancel) {
            addressIndex++;
            window.setTimeout(launchG, 60)
        } else {
            calculate();
            clearWeights();
            clearGeocode();
            dispStart();
            dispMsg("", 2)
        }
    }
}

function dispStart() {
    switchLoc();
    toggleDivs(["DB", "DB2", "DB3"], 1);
    f1.add.focus()
}

function dispProceed() {
    switchLoc();
    toggleDivs(["DB2", "DB", "DB3"], 1)
}

function ok1() {
    D("DE").style.display = "none";
    pause = 0;
    if (addresses.length > 1) {
        dispProceed()
    }
    if (D("ok").value == "Retry" || D("ok").value == "Resume") {
        request = -1;
        launchG();
        return false
    }
    appendToList();
    loopG()
}

function skip() {
    D("DE").style.display = "none";
    pause = 0;
    if (addresses.length > 1) {
        dispProceed()
    }
    loopG()
}

function cancelGeocode() {
    D("DE").style.display = "none";
    cancel = 1;
    if (p.length > 1) {
        calculate()
    }
    dispStart();
    dispMsg("", 2)
}

function contin() {
    D("DE").style.display = "none";
    dispMsg("Please wait...", 2);
    if (f1.radr[1].checked) {
        launchG()
    } else {
        launchL()
    }
}

function validateLl(c, a) {
    var g = ["ns", "ew"];
    var b = ["90(\\.0+)?|[0-8]?\\d", "180(\\.0+)?|(0?\\d?\\d|1[0-7]\\d)"];
    var f = "^(-|[" + g[a] + "]\\s*)?(" + b[a] + "([^\\w.-]+[0-5]?\\d){0,2}(\\.\\d+)?)(\\s*[" + g[a] + "])?$";
    var e = new RegExp(f, "i");
    return (e.test(c) && (/^[nsew-]/i.test(c) + /[nsew-]$/i.test(c) < 2))
}

function latLonToDecimal(c) {
    var a = c.replace(/-/, "");
    var d = a.split(/[^\d.]/);
    var b = 0;
    for (i = 0; i < d.length; i++) {
        b += d[i] / M.pow(60, i)
    }
    c = -2 * b * (/[sw-]/i.test(c) - 0.5);
    return c
}

function openPlace() {
    var a = p.selectedIndex;
    if (a > -1) {
        infoWindow.close();
        if (p[a].marker.getVisible() == false) {
            p[a].marker.setVisible(true)
        }
        google.maps.event.trigger(p[a].marker, "click")
    }
}

function triggerMid() {
    if (MM) {
        google.maps.event.trigger(MM, "click")
    }
}

function save(c) {
    if (!map || !map.getCenter()) {
        return
    }
    var o = "",
        v = 0,
        x = "",
        s = "",
        q = "",
        F = "",
        k = "",
        r = "",
        E = "",
        g = "",
        f = "",
        A = p.length;
    if (!c) {
        x = "cl=" + roundx(map.getCenter().lat(), 5) + "&cn=" + roundx(map.getCenter().lng(), 5) + "&z=" + map.getZoom() + "&x=" + f1.large.checked * 1 + "&c=" + cI + "&p=" + f1.disp.checked * 1 + "&r=" + rI + "&w=" + wI
    }
    if (A > 0) {
        var t = false,
            w = false,
            H = false;
        for (j = 0; j < p.length; j++) {
            if (p[j].y != 0) {
                t = true
            }
            if (p[j].m != 0) {
                w = true
            }
            if (p[j].d != 0) {
                H = true
            }
        }
        if (MM) {
            o += "ml=" + roundx(MM.getPosition().lat(), 5) + "&mn=" + roundx(MM.getPosition().lng(), 5) + "&"
        }
        var C = "Microsoft Internet Explorer";
        var z = location.href + "?";
        z = z.substring(0, z.indexOf("?"));
        var B = 2083 + 1927 * (navigator.appName != C) - z.length - o.length - x.length - 3 * (t + w + H) * (c == 0) - 10;
        var G = getLength(v, t, w, H, c, "");
        while (v < A && s.length + q.length + F.length + k.length + r.length + E.length + G.tot.length < B) {
            s += G.l;
            q += G.n;
            F += G.a;
            if (!c) {
                if (t) {
                    k += G.y
                }
                if (w) {
                    r += G.m
                }
                if (H) {
                    E += G.d
                }
            }
            v++;
            if (v < A) {
                G = getLength(v, t, w, H, c, "|")
            }
        }
        o += "l=" + s + "&n=" + q + "&a=" + F
    }
    if (v < A) {
        var b = "save";
        if (c) {
            b = "transfer"
        }
        f += " Your browser can " + b + " the midpoint for all places along with the first " + v + " place markers."
    }
    if (!c) {
        if (t) {
            o += "&y=" + k
        }
        if (w) {
            o += "&m=" + r
        }
        if (H) {
            o += "&d=" + E
        }
        if (o.length > 2) {
            o += "&"
        }
        g = "Click ok to refresh the page. You can then save the page/map in your Favorites/Bookmarks.";
        F = confirm(g + f);
        if (F) {
            window.location.search = "?" + o + x
        }
    } else {
        g = "Click ok to transfer the midpoint marker and other data to a page with a searchable map where you can search for points of interest near the midpoint.";
        F = confirm(g + f);
        if (F) {
            window.location = "meet/index.html?" + o
        }
    }
}

function getLength(e, f, g, d, c, b) {
    var a = new Object();
    a.l = b + roundx(p[e].marker.getPosition().lat(), 6);
    a.n = b + roundx(p[e].marker.getPosition().lng(), 6);
    a.a = b + encodeURI(p[e].text.replace(/ /gi, "+"));
    if (!c && f) {
        a.y = b + p[e].y
    } else {
        a.y = ""
    }
    if (!c && g) {
        a.m = b + p[e].m
    } else {
        a.m = ""
    }
    if (!c && d) {
        a.d = b + p[e].d
    } else {
        a.d = ""
    }
    a.tot = a.l + a.n + a.a + a.y + a.m + a.d;
    return a
}

function popMessage(b) {
    var a = '"resizeable=0,left=' + (screen.width / 2 - 180) + ",top=" + (screen.height / 2 - 120) + ',width=360,height=240"';
    messagewin = window.open("message.php", "messagewin", a);
    D("frm2").target = "messagewin";
    D("frm2").data2.value = b;
    D("frm2").submit()
}

function revGeoCallback(d, a) {
    if (a == google.maps.GeocoderStatus.OK) {
        if (d[0]) {
            var e = '<p class="pz">Nearest address:<br>' + splitAddress(d[0].formatted_address) + "</p>";
            var c = MM.h1;
            var b = MM.h2;
            MM.setMap(null);
            MM = createMarker(MM.getPosition(), c + e + b, 1, 0);
            MM.setMap(map);
            MM.h1 = c;
            MM.h2 = b
        }
    }
}

function formatInfo(e, f, b, c) {
    var d = '<div style="width: 16.5em"><p class="pz">' + e + "</p>";
    if (!isNaN(f)) {
        d += '<div class="DWH">Latitude:<br>Longitude:</div><div class="DBL" onclick="selectText(this)">' + roundx(f, 6) + "<br>" + roundx(b, 6) + '</div><div class="DCL"></div>'
    }
    if (c > -1) {
        if (f1.radw[0].checked && (p[c].y || p[c].m || p[c].d)) {
            d += '<p class="pz">Weight:<br>Years: ' + p[c].y + " Months: " + p[c].m + " Days: " + p[c].d + "</p>"
        } else {
            if (f1.radw[1].checked && p[c].d) {
                d += '<p class="pz">Weight: ' + p[c].d + "</p>"
            }
        }
        var a = '<b>Drag me</b> to pinpoint a location<br><a class="bluelink" href="javascript:r5(' + c + ')">Remove me</a>';
        if (p[c].marker.dragged == 1) {
            a += '&nbsp;&nbsp;&nbsp;&nbsp;<a class="bluelink" href="javascript:reset(' + c + ')">Reset location</a>'
        }
        d += '<p class="pz">' + a + "</p></div>"
    }
    return d
}
google.maps.event.addDomListener(window, "load", initialize);