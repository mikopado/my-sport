$(function () {

    $('#search').autocomplete({ //Autocomplete for jQuery UI
        source: function (request, response) { // get function with request object who holds a term property and response that is a function
            
            $.ajax({
                url: window.location.origin + '/Profile/Search',
                dataType: 'json',
                data: { search: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            value: item.Name,
                            label: item.Photo,
                            id: item.CurrentProfile.ProfileId
                        };
                    }));
                }

            });
        },
        minLength: 1,
        select: function (event, ui) {
            window.location.pathname = "Profile/MyProfile/" + ui.item.id;
            return false;
        }
    }).data("ui-autocomplete")._renderItem = function (ul, item) {       
        return $('<div/>')
            .data('ui-autocomplete-item', item)
            .append("<li class='list-group-item'><div class='row'><div class='col-md-12 search-result-item'><div class='col-md-2'><img src=" + item.label + " /></div><div class='col-md-10'>" + item.value + "</div></div></div></li>")
            .appendTo(ul);
    };



})