var apiUrl = "/api";
var playerUrl = apiUrl + "/play";
var elementUrl = apiUrl + "/element";
var timeoutTimer;

function ViewModel() {

	var self = this;
	self.elements = ko.observableArray();
	self.sequences = ko.observableArray();
	self.selectedSequence = ko.observable();
	self.status = ko.observable();
	self.elementOnTime = ko.observable(30);
	self.elementResults = ko.observableArray();
	self.selectedElement = ko.observable();
	self.selectedColor = ko.observable("#FFFFFF");
	self.elementTree = ko.observableArray();
	self.selectedSequence = ko.observable();
	
	self.elementClicked = function (elem) {

		self.selectedElement(elem);
		if (elem.Colors.length > 0) {
			self.selectedColor(elem.Colors[0]);
		} else {
			self.selectedColor("#FFFFFF");
		}
		$('#elementControlContainer').trigger('create');
		$(":mobile-pagecontainer").pagecontainer("change", "#ElementControlPage", { changeHash: false });
		return false;
	}
	self.navigateChild = function (elem) {
		self.showLoading();
		self.elementTree.push(self.selectedElement());
		self.selectedElement(elem);
		$('#elementControlContainer').trigger('create');
		self.hideLoading();
		return false;
	}

	self.navigateParent = function (elem) {
		if (self.elementTree().length > 0) {
			self.showLoading();
			self.selectedElement(self.elementTree().pop());
			$('#elementControlContainer').trigger('create');
			self.hideLoading();
		} else {
			$(":mobile-pagecontainer").pagecontainer("change", "#Elements");
		}
		return false;
	}

	// Status functions
	self.getStatus = function() {
		$.get(playerUrl + '/status')
			.done(function (status) {
				self.status(status.Message);
			});
	}

	self.clearStatus = function(seconds) {
		if (timeoutTimer) {
			clearTimeout(timeoutTimer);
		}
		timeoutTimer = setTimeout(function () {
			self.status("");
		}, seconds * 1000);

	}

	self.updateStatus = function (seconds) {
		if (timeoutTimer) {
			clearTimeout(timeoutTimer);
		}
		timeoutTimer = setTimeout(function () {
			self.getStatus();
		}, seconds * 1000);

	}

	//Element retrieval 
	self.searchElements = function (value) {
		if (value && value.length > 0) {
			self.showLoading();
			$.ajax({
				url: elementUrl + '/searchElements',
				dataType: "json",
				data: {
					q: value
				}
			})
				.then(function(response) {
					self.elementResults(response);
					self.selectedElement(response[0]);
					//$('#ElementSearch').trigger('create');
					$('#ElementSetFiltered').listview('refresh');
					$('#ElementSetFiltered').trigger('updatelayout');
					self.hideLoading();
				});
		} else {
			self.getElements();
		}
	}

	self.getElements = function() {
		$.get(elementUrl + '/getElements')
			.done(function (data) {
				self.elementResults(data);
				$('#ElementSetFiltered').listview('refresh');
				$('#ElementSetFiltered').trigger('updatelayout');
			}).error(function (jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
	}

	self.turnOnElement = function() {
		//var parms = $(element).serialize();
		$.get(elementUrl + '/on', {id:self.selectedElement().Id, time:self.elementOnTime, color:self.selectedColor})
			.done(function (status) {
				self.status(status.Message);
				self.updateStatus(model.elementOnTime());
			}).error(function (jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
		return false;
	}

	//Sequence functions

	self.stopSequence = function() {
		$.post(playerUrl + '/stopSequence')
			.done(function (status) {
				self.status(status.Message);
				self.getStatus();
			}).error(function (jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
	}

	self.playSequence = function() {
		self.showLoading();
		$.ajax({
			url: playerUrl + '/playSequence',
			type: 'POST',
			contentType: "application/json",
			data: ko.toJSON(self.selectedSequence)
			})
			.done(function (status) {
				self.status(status.Message);
				self.hideLoading();
				var time = status.Message.substring(status.Message.length - 16);
				self.updateStatus(moment.duration(time).asSeconds());
		}).error(function(jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
		});
	}

	self.getSequences = function() {
		$.get(playerUrl + '/getSequences')
			.done(function (data) {
				self.sequences(data);
			}).error(function(jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
	}

	self.showLoading = function() {
		setTimeout(function () {
			$.mobile.loading('show');
		}, 1);
	}

	self.hideLoading = function() {
		setTimeout(function () {
			$.mobile.loading('hide');
		}, 300);
	}

};

ko.bindingHandlers.jqmRefreshList = {
	update: function (element, valueAccessor) {
		ko.utils.unwrapObservable(valueAccessor()); //just to create a dependency
		var listview = $(element).parents()
                             .andSelf()
                             .filter("[data-role='listview']");

		if (listview) {
			try {
				$(listview).listview('refresh');
				$(listview).trigger('updatelayout');
			} catch (e) {
				// if the listview is not initialised, the above call with throw an exception
				// there doe snot appear to be any way to easily test for this state, so
				// we just swallow the exception here.
			}
		}

	}
};

var model = new ViewModel();
ko.applyBindings(model);

model.getStatus();


// event stuff
$(document).on("pagecreate", "#Elements", function () {
	$("#ElementSetFiltered").on("filterablebeforefilter", function (e, data) {
		var $input = $(data.input);
		var value = $input.val();
		model.searchElements(value);
	});
});

$('#Elements').on('pagecreate', function (event) {
    model.getElements();
});

$('#Sequences').on('pagecreate', function (event) {
    model.getSequences();
    model.getStatus();
});

var b = document.documentElement;
b.setAttribute('data-useragent', navigator.userAgent);
b.setAttribute('data-platform', navigator.platform);
b.className += ((!!('ontouchstart' in window) || !!('onmsgesturechange' in window)) ? ' touch' : '');



