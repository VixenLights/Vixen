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
	self.timeout = ko.observable(30);
	self.elementIntensity = ko.observable(100);
	self.elementResults = ko.observableArray();
	self.selectedElement = ko.observable();
	self.selectedColor = ko.observable("#FFFFFF");
	self.elementTree = ko.observableArray();
	self.selectedSequence = ko.observable();
	
	self.elementClicked = function (elem) {

		self.selectedElement(elem);
		$(":mobile-pagecontainer").pagecontainer("change", "#ElementControlPage", { changeHash: false });
		return false;
	}
	self.navigateChild = function (elem) {
		self.showLoading();
		self.elementTree.push(self.selectedElement());
		self.selectedElement(elem);
		self.hideLoading();
		return false;
	}

	self.navigateParent = function (elem) {
		if (self.elementTree().length > 0) {
			self.showLoading();
			self.selectedElement(self.elementTree().pop());
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
					//$('#ElementSetFiltered').trigger('create');
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
				//$('#ElementSetFiltered').trigger('create');
			}).error(function (jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
	}

	self.turnOnElement = function(element) {
		var a = $(element).serializeArray();

		var parms = {};
		
		$.each(a, function () {
			if (parms[this.name] !== undefined) {
				if (!parms[this.name].push) {
					parms[this.name] = [parms[this.name]];
				}
				parms[this.name].push(this.value || '');
			} else {
				parms[this.name] = this.value || '';
			}
		});


		parms.duration = self.timeout();
		parms.intensity=self.elementIntensity();
		$.ajax({url: elementUrl + '/on',
				type: 'POST',
				contentType: "application/json",
				data: ko.toJSON(parms)
		})
			.done(function (status) {
				self.status(status.Message);
				self.updateStatus(self.timeout());
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

ko.bindingHandlers.jqmSlider = {
	// Initialize slider
	init: function (element, valueAccessor) {
		var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
		setTimeout(function () {
			// $(element) doesn't work as that has been removed from the DOM
			var curSlider = $('#' + element.id);
			// helper function that updates the slider and refreshes the thumb location
			function setSliderValue(newValue) {
				curSlider.val(newValue).slider('refresh');
			}
			// subscribe to the bound observable and update the slider when it changes
			valueAccessor().subscribe(setSliderValue);
			// set up the initial value from the observable
			setSliderValue(valueUnwrapped);
			// subscribe to the slider's change event and update the bound observable
			curSlider.bind('change', function () {
				valueAccessor()(curSlider.val());
			});
		}, 0);
	}
};


ko.bindingHandlers.jqmRefreshList = {
	update: function (element, valueAccessor) {
		ko.utils.unwrapObservable(valueAccessor()); //just to create a dependency
		var listview = $(element);
		if (listview) {
			try {
				model.showLoading();
				$(listview).trigger('create');
				$(listview).listview('refresh');
				$(listview).trigger('updatelayout');
			} catch (e) {
				// if the listview is not initialised, the above call with throw an exception
				// there does not appear to be any way to easily test for this state, so
				// we just swallow the exception here.
			} finally {
				model.hideLoading();
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



