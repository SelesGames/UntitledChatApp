$(function () {
    // Declare a proxy to reference the hub.
    var COOKIE_KEY = "chatHubCookie";
    var chat = $.connection.chatHub;

    var userInfo = { id: null, name: null };

    var existingCookie = $.cookie(COOKIE_KEY);
    if (existingCookie != null) {
        userInfo = JSON.parse(existingCookie);
    }

    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        //var encodedName = $('<div class="txtUserName" />').text(name).html();
        //var encodedMsg = $('<div class="txtMessage" />').text(message).html();
        // Add the message to the page.

        var messageElement = '<div class="textEntry"><div class="txtUserName">' +
            name +
            ':</div><div class="txtMessage">' +
            message +
            '</div></div>';

        $('#chatbox').append(messageElement);

        $('html').stop().animate({
            scrollTop: $("html")[0].scrollHeight
        }, 800);
    };

    function onLoggedIn() {
        subscribeToSendButton();
        subscribeToEnterKeyPress();
    }

    function subscribeToSendButton() {
        $('#sendmessage').click(function () { sendMessage() });
    };

    function subscribeToEnterKeyPress() {
        $('#message').keyup(
            function (event) {
                switch (event.keyCode) {
                    case 13:
                        sendMessage();
                        return;
                }
            });                
    }

    function sendMessage() {
        // Call the Send method on the hub.
        chat.server.send($('#message').val());
        // Clear text box and reset focus for next comment.
        $('#message').val('').focus();
    }

    function onLocationNotFound() {
        alert("location not found!!");
    }

    function onLocated(position) {
        var latitude = position.coords.latitude;
        var longitude = position.coords.longitude;
        //alert("Latitude : " + latitude + " Longitude: " + longitude);

        chat.server
            .login(
                userInfo.id,
                userInfo.name,
                latitude,
                longitude)
            .done(onLoggedIn);
    };

    function beginGpsQuery() {
        if (!navigator.geolocation) {
            alert("Sorry, browser does not support geolocation!");
        }
        else
            navigator.geolocation.getCurrentPosition(onLocated, onLocationNotFound);
    };

    function onConnectedToChatHub() {
        if (userInfo.name == null || !userInfo.name) {
            userInfo.name = prompt('Enter your name:', '');

            chat.server.createUser(userInfo.name).done(function (u) {
                userInfo = u;
                var jsonRep = JSON.stringify(userInfo);
                $.cookie(COOKIE_KEY, jsonRep, { expires: 10 });
                beginGpsQuery();
            });
        }
        else {
            //alert("Cookie recovered: " + userInfo.name);
            beginGpsQuery();
        }
    };

    // Set initial focus to message input box.
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(onConnectedToChatHub);
});