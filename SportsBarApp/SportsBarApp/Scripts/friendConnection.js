  
$(function () {
    

    var friendRequest = $.connection.friendRequestHub;
   
    friendRequest.client.sendFriendRequest = function (userName, photoFileName, requestId) {
        $('#pending-requests ul:first-child').prepend('<li><img class="img-friends thumbnail" src="' + photoFileName + '" />' + userName + '</a><section><input type="button" id="' + requestId + '" class="accept-btn btn btn-info btn-requests" value="Accept"/> | <input type="button" id="' + requestId + '" class="ignore-btn btn btn-info btn-requests" value="Ignore"/></section></li>');
        if ($('#count').hasClass('hidden')) {
            $('#count').removeClass('hidden');
        }
        var count = parseInt($('#count').text()) + 1;
        $('#count').text(count);        
        

        $('.accept-btn').click(function () {            
            var self = $(this);
            $.ajax({
                type: 'POST',
                url: window.location.origin + '/Profile/AcceptFriendship/' + requestId,
                success: function (response) {
                    self.parent().parent().remove();
                    var newCount = parseInt($('#count').text()) - 1;
                    $('#count').text(newCount);
                    if (newCount === 0) {
                        $('#count').addClass('hidden');
                    }
                }
            });
        });

        $('.ignore-btn').click(function () {           
            var self = $(this);
            $.ajax({
                type: 'POST',
                url: window.location.origin + '/Profile/IgnoreFriendshipRequest/' + requestId,
                success: function (response) {
                    self.parent().parent().remove();
                    var newCount = parseInt($('#count').text()) - 1;
                    $('#count').text(newCount);
                    if (newCount === 0) {
                        $('#count').addClass('hidden');
                    }
                }
            });
        });

        
    };

    $.connection.hub.start();

    

});


