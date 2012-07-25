'use strict';

var ui = {
    picturesLoading: ko.observable(false),
    picturesResponse: ko.observable(),

    pictureId: ko.observable(''),
    pictureLoading: ko.observable(false),
    pictureResponse: ko.observable(),

    tagsLoading: ko.observable(false),
    tagsResponse: ko.observable(),

    tag: ko.observable(''),
    tagPicturesLoading: ko.observable(false),
    tagPicturesResponse: ko.observable(),

    picturePostResponse: ko.observable(),
    picturePostSubmitting: ko.observable(false),

    deletePictureId: ko.observable(''),
    pictureDeleteResponse: ko.observable(),
    pictureDeleting: ko.observable(false),

    retrievePictures: function () {
        var uri = '/api/pictures';
        ui.picturesLoading(true);
        $.getJSON(uri, function(list) {
            ui.picturesResponse(list);
            ui.picturesLoading(false);
        }).error(function (jqXHR, textStatus, errorThrown) {
            ui.picturesResponse(jqXHR.responseText);
            ui.picturesLoading(false);
        });
    },

    retrievePicture: function () {
        var picId = ui.pictureId();
        if (!picId) return;

        var uri = '/api/pictures/' + picId;
        ui.pictureLoading(true);
        $.getJSON(uri, function (picMetadata) {
            ui.pictureResponse(picMetadata);
            ui.pictureLoading(false);
        }).error(function (jqXHR, textStatus, errorThrown) {
            ui.pictureResponse(jqXHR.responseText);
            ui.pictureLoading(false);
        });
    },

    deletePicture: function () {
        var picId = ui.deletePictureId();
        if (!picId) return;

        var uri = '/api/pictures/' + picId;
        ui.pictureDeleting(true);
        $.ajax({
            type: "DELETE",
            url: uri,
            success: function (data) {
                ui.pictureDeleteResponse(data);
                ui.pictureDeleting(false);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ui.pictureDeleteResponse(jqXHR.responseText);
                ui.pictureDeleting(false);
            }
        });
    },

    retrieveTags: function () {
        var uri = '/api/tags';
        ui.tagsLoading(true);
        $.getJSON(uri, function (tags) {
            ui.tagsResponse(tags);
            ui.tagsLoading(false);
        }).error(function (jqXHR, textStatus, errorThrown) {
            ui.tagsResponse(jqXHR.responseText);
            ui.tagsLoading(false);
        });
    },

    retrieveTagPictures: function () {
        var tag = ui.tag();
        if (!tag) return;

        var uri = '/api/tags/' + tag + '/pictures';
        ui.tagPicturesLoading(true);
        $.getJSON(uri, function (pictures) {
            ui.tagPicturesResponse(pictures);
            ui.tagPicturesLoading(false);
        }).error(function (jqXHR, textStatus, errorThrown) {
            ui.tagPicturesResponse(jqXHR.responseText);
            ui.tagPicturesLoading(false);
        });
    },

    closePictures: function () {
        ui.picturesLoading(false);
        ui.picturesResponse('');
    },

    closePicture: function () {
        ui.pictureLoading(false);
        ui.pictureResponse('');
    },

    closeTags: function () {
        ui.tagsLoading(false);
        ui.tagsResponse('');
    },

    closeTagPictures: function () {
        ui.tagPicturesLoading(false);
        ui.tagPicturesResponse('');
    },

    closePicturePostResponse: function () {
        ui.picturePostResponse('');
        ui.picturePostSubmitting(false);
    }
};

ko.bindingHandlers.formattedJson = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        function urlAsHref(url) {
            if (url.indexOf('http://') == 0 || url.indexOf('https://') == 0) {
                return '<a href=' + url + '>' + url + '</a>';
            }

            return url;
        }

        // First get the latest data that we're bound to
        var value = valueAccessor(), allBindings = allBindingsAccessor();

        // Next, whether or not the supplied model property is observable, get its current value
        value = ko.utils.unwrapObservable(value);
        if (value) {
            if ($.isArray(value)) {
                for (var i = 0; i < value.length; i++) {
                    if (value[i].Url) value[i].Url = urlAsHref(value[i].Url)
                }
            } else {
                if (value.Url) value.Url = urlAsHref(value.Url);
            }
        }

        var json = JSON.stringify(value, null, 4);

        $(element).html(json); // Make the element visible
        prettyPrint();
    }
};

$(function () {
    // Knockout: bind viewModel
    ko.applyBindings();

    $('#picturePostForm').ajaxForm({
        dataType: 'json',
        beforeSubmit: function () { ui.picturePostSubmitting(true); },
        success: function (response) {
            ui.picturePostSubmitting(false);
            ui.picturePostResponse(response);
        }
    });
});
