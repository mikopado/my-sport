  
$(function () {
    

    var friendRequest = $.connection.friendRequestHub;
   
    friendRequest.client.sendFriendRequest = function (userName, photoFileName, requestId) {
        $('#pending-requests ul:first-child').prepend('<li><img class="img-friends thumbnail" src="' + photoFileName + '" />' + userName + '<section><input type="button" id="' + requestId + '" class="accept-btn btn btn-info btn-requests" value="Accept"/> | <input type="button" id="' + requestId + '" class="ignore-btn btn btn-info btn-requests" value="Ignore"/></section></li>');
        if ($('#count').hasClass('hidden')) {
            $('#count').removeClass('hidden');
        }

        $.get(window.location.origin + '/Friends/IncreasePendingCookie/',            
            function (data) {
                $('#count').text(data);               
            });
       
       
        $('.accept-btn').click(function () {            
            var self = $(this);
            $.ajax({
                type: 'POST',
                url: window.location.origin + '/Friends/AcceptFriendship/' + requestId,
                success: function (data) {                   
                    self.parent().parent().remove();
                    $('#friends ul:first-child').prepend('<li><img class="img-friends thumbnail" src="' + photoFileName + '" />' + userName + '<input type="button" id="' + requestId + '" class="btn btn-info btn-friends remove-btn disabled" value="Remove"/></li>');
                   
                    $('#count').text(data);
                    if (parseInt(data) === 0) {
                        $('#count').addClass('hidden');
                    }
                }
            });
        });

        $('.ignore-btn').click(function () {           
            var self = $(this);
            $.ajax({
                type: 'POST',
                url: window.location.origin + '/Friends/IgnoreFriendshipRequest/' + requestId,
                success: function (data) {
                    self.parent().parent().remove();                                  
                    $('#count').text(data);                    
                    if (parseInt(data) === 0) {
                        $('#count').addClass('hidden');
                    }
                }
            });
        });
       
               
    };

    $.connection.hub.start();
    

    
});


