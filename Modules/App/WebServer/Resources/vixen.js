var apiUrl = "/api";
var playerUrl = apiUrl + "/play";
var elementUrl = apiUrl + "/element";
var timeoutTimer;

var viewModel = {
    elements: ko.observableArray(),
    sequences: ko.observableArray(),
    selectedSequence: ko.observable(),
    status: ko.observable(),
    elementOnTime:ko.observable(30)

};

ko.applyBindings(viewModel);

function afterElementsRendered() {
    $('#element-list').trigger('create');
}

function stopSequence() {
    $.post(playerUrl + '/stopSequence')
        .done(function(status) {
            viewModel.status(status.Message);
            getStatus();
        });
}

function getStatus() {
    $.get(playerUrl + '/status')
        .done(function (status) {
            viewModel.status(status.Message);
        });
}

function playSequence() {
    showLoading();
    var file = $("#select-sequence").val();
    //Get because Kayak stinks for getting post parms out.
    $.get(playerUrl + '/playSequence', { name: file })
        .done(function (status) {
            viewModel.status(status.Message);
            hideLoading();
        });
}

function getSequences() {
    $.get(playerUrl + '/getSequences')
        .done(function(data) {
            viewModel.sequences(data);
        });
}

function getElements() {
    $.get(elementUrl + '/getElements')
        .done(function (data) {
            viewModel.elements(data);
        });
}

function turnOnElement(element) {
    var parms = $(element).serialize();
    $.get(elementUrl + '/on', parms)
        .done(function (status) {
            viewModel.status(status.Message);
            clearStatus(viewModel.elementOnTime());
        });
    return false;
}

function clearStatus(seconds) {
    if (timeoutTimer) {
        clearTimeout(timeoutTimer);
    }
    timeoutTimer = setTimeout(function() {
        viewModel.status("");
    },seconds*1000);
    
}

function showLoading() {
    setTimeout(function () {
        $.mobile.loading('show');
    }, 1);
}

function hideLoading() {
    setTimeout(function () {
        $.mobile.loading('hide');
    }, 300);
}

$(document).on('pagebeforecreate', '[data-role="page"]', function () {
    showLoading();
});

$(document).on('pageshow', '[data-role="page"]', function () {
    hideLoading();
});

$('#Elements').on('pagecreate', function (event) {
    getElements();
});

$('#Sequences').on('pagecreate', function (event) {
    getSequences();
    getStatus();
});

$(document).ready(function () {
    
    //Generally not needed for JQM
    
});


