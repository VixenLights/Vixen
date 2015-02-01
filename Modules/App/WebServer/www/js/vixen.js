var apiUrl = "/api";
var playerUrl = apiUrl + "/play";
var elementUrl = apiUrl + "/element";
var timeoutTimer;
var storeTimeKey = "timeout";
var storeIntesityKey = "intensity";

function ViewModel() {

	var self = this;
	self.elements = ko.observableArray();
	self.sequences = ko.observableArray();
	self.selectedSequence = ko.observable();
	self.status = ko.observable();
	self.timeout = ko.observable(30);
	self.elementIntensity = ko.observable(100);
	self.delayedElementIntensity = ko.pureComputed(self.elementIntensity).extend({ rateLimit: { timeout: 500, method: "notifyWhenChangesStop" } });
	self.elementResults = ko.observableArray();
	self.elements = ko.observable();
	self.selectedElement = ko.observable();
	self.selectedColor = ko.observable("#FFFFFF");
	self.elementTree = ko.observableArray();
	self.selectedSequence = ko.observable();
	self.search = ko.observable(true);
	self.searchToken = ko.observable();
	self.delayedSearchToken = ko.pureComputed(self.searchToken).extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
	self.searchTokenHold = "";
	self.searchResultsOverflow = ko.observable(false);
	
	self.clearSearch = function() {
		self.searchToken("");
	}

	self.navigateChild = function (elem) {
		self.showLoading();
		self.elementTree.push(self.elements());
		if (self.search()) {
			self.search(false);
		}

		self.elements(elem.Children);
		self.hideLoading();
		return false;
	}

	self.navigateParent = function (elem) {
		
		self.showLoading();
		self.elements(self.elementTree().pop());
		if (self.elementTree().length == 0) {
			self.search(true);
		}
		self.hideLoading();
		
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
				.then(function (response) {
					if (response.length < 50) {
						self.elements(response);
						self.searchResultsOverflow(false);
					} else {
						self.searchResultsOverflow(true);
					}
					
					self.hideLoading();
				});
		} else {
			self.showLoading();
			self.getElements();
			self.searchResultsOverflow(false);
			self.hideLoading();
		}
	}

	self.getElements = function () {
		$.get(elementUrl + '/getElements')
			.done(function (data) {
				self.elements(data);
			}).error(function (jqXHR, status, error) {
				self.status(error);
				self.hideLoading();
			});
	}

	self.turnOnElement = function(data, event, timed) {
		var a = $(event.target).closest("form").serializeArray();;

		//model data
		var parms = {
			id : data.Id,
			duration : timed?self.timeout():0,
			intensity:self.elementIntensity()
		};
		
		//supplement with form data for the Color
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

		$.ajax({url: elementUrl + '/on',
			type: 'POST',
			datatype: "JSON",
			data:parms
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

	self.turnOffElement = function (data, event) {
		
		var parms = {
			id: data.Id
		};
		
		$.ajax({
			url: elementUrl + '/off',
			type: 'POST',
			dataType:"JSON",
			data: parms
		})
			.done(function (status) {
				self.status(status.Message);
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
			dataType:"json",
			data: self.selectedSequence()
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

	//subscriptions

	self.delayedSearchToken.subscribe(function (val) {
		self.searchElements(val);
	});

	self.timeout.subscribe(function(val) {
		amplify.store(storeTimeKey, val);
	});

	self.delayedElementIntensity.subscribe(function(val) {
		amplify.store(storeIntesityKey, val);
	});
	

	self.showLoading = function() {
		setTimeout(function () {
			$('#loading-indicator').show();
		}, 1);
	}

	self.hideLoading = function() {
		setTimeout(function () {
			$('#loading-indicator').hide();
		}, 300);
	}

	self.retrieveStoredSettings = function () {
		var intensity = amplify.store(storeIntesityKey);
		if (intensity) {
			self.elementIntensity(intensity);
		}
		var time = amplify.store(storeTimeKey);
		if (time) {
			self.timeout(time);
		}
		
	}

	self.init = function ()
	{
		self.showLoading();
		self.getStatus();
		self.getElements();
		self.retrieveStoredSettings();
		self.getSequences();
		self.hideLoading();
	}

};

ko.bindingHandlers.jqmSlider = {
	// Initialize slider
	//https://github.com/seiyria/bootstrap-slider#options
	init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
		var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
		$(element).slider({
			min: allBindings.get("min")||0,
			max: allBindings.get("max") || 100,
			value: allBindings.get("value") || valueUnwrapped
		}).on("change", function() {
			valueAccessor()( $(element).slider('getValue'));
		});
	},
	update: function(element, valueAccessor) {
		var value = ko.unwrap(valueAccessor());
		$(element).slider('setValue', value);
	}
};

//https://bgrins.github.io/spectrum/
ko.bindingHandlers.spectrumColorPicker = {
	init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
		var defaultColor = allBindings.get('default') || "#FFFFFF";
		var colors = bindingContext.$data.Colors;
		if (colors.length>0) {
			$(element).spectrum({
				color: colors[0],
				change: function (color) {
					$(element).val(color.toHexString());
				},
				theme: "sp-bigger",
				hideAfterPaletteSelect: true,
				//flat:true,
				showPaletteOnly: true,
				palette: colors
			}).val(colors[0]);
		} else {
			$(element).spectrum({
				color: defaultColor,
				change: function (color) {
					$(element).val(color.toHexString());
				},
				theme: "sp-big",
				showInput: true,
				preferredFormat: "hex",
				chooseText: "Ok",
				localStorageKey: "spectrum.color",
				hideAfterPaletteSelect: true,
				showPaletteOnly: true,
				togglePaletteOnly: true,
				togglePaletteMoreText: 'more',
				togglePaletteLessText: 'less',
				maxSelectionSize: 4,
				palette: [
					['red', 'green', 'blue', 'white']
				]
			}).val(defaultColor);
		}
		
	}
};

var model = new ViewModel();
ko.applyBindings(model);

model.init();

var b = document.documentElement;
b.setAttribute('data-useragent', navigator.userAgent);
b.setAttribute('data-platform', navigator.platform);
b.className += ((!!('ontouchstart' in window) || !!('onmsgesturechange' in window)) ? ' touch' : '');

$(document).on('click', '.navbar-collapse.in', function (e) {
	if ($(e.target).is('a')) {
		$(this).collapse('hide');
	}
});

