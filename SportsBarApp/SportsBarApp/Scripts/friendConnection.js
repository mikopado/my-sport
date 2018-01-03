  
$(function () {

    var friendRequest = $.connection.friendRequestHub;
   
    friendRequest.client.sendFriendRequest = function (message) {
        $('#friends').append('<div>New Friend Request</div>');
        var count = parseInt($('#count').text()) + 1;
        $('#count').text(count);
        console.log("ok");
    };

    $.connection.hub.start();

});


