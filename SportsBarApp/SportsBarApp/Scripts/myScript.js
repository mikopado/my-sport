$(function () {


    $(".datePicker").datepicker({

        dateFormat: "dd/mm/yy",
        changeYear: true,
        maxDate: '0'
    });

    $("#friend-status").on("click", function () {
        $(this).addClass("disabled");
        $(this).text("Pending Request");
    });

    var friendRequest = $.connection.friendRequestHub;
    friendRequest.client.notifyUser = function (name, message) {

    };

    
});
