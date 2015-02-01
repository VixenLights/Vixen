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
	self.elements = ko.observable();
	self.selectedElement = ko.observable();
	self.selectedColor = ko.observable("#FFFFFF");
	self.elementTree = ko.observableArray();
	self.selectedSequence = ko.observable();
	self.search = ko.observable(true);
	self.searchToken = ko.observable();
	self.delayedSearchToken = ko.pureComputed(self.searchToken).extend({ rateLimit: { timeout: 300, method: "notifyWhenChangesStop" } });
	self.searchTokenHold = "";
	
	self.navigateChild = function (elem) {
		self.showLoading();
		self.elementTree.push(self.elements());
		if (self.search()) {
			self.search(false);
			//self.searchTokenHold = self.searchToken();
			//self.searchToken("");
		}

		self.elements(elem.Children);
		self.hideLoading();
		return false;
	}

	self.navigateParent = function (elem) {
		
		self.showLoading();
		self.elements(self.elementTree().pop());
		if (self.elementTree().length == 0) {
			//self.searchToken(self.searchTokenHold);
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
				.then(function(response) {
					self.elements(response);
					self.hideLoading();
				});
		} else {
			self.getElements();
		}
	}
	self.delayedSearchToken.subscribe(function(val) {
		self.searchElements(val);
	});

	self.getElements = function() {
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

};

ko.bindingHandlers.jqmSlider = {
	// Initialize slider
	init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
		var valueUnwrapped = ko.utils.unwrapObservable(valueAccessor());
		$(element).slider({
			min: allBindings.get("min")||0,
			max: allBindings.get("max") || 100,
			value: allBindings.get("value") || valueUnwrapped
		}).on("change", function() {
			valueAccessor()( $(element).slider('getValue'));
		});

		//setTimeout(function () {
		//	// $(element) doesn't work as that has been removed from the DOM
		//	var curSlider = $('#' + element.id);
		//	// helper function that updates the slider and refreshes the thumb location
		//	function setSliderValue(newValue) {
		//		curSlider.val(newValue).slider('refresh');
		//	}
		//	// subscribe to the bound observable and update the slider when it changes
		//	valueAccessor().subscribe(setSliderValue);
		//	// set up the initial value from the observable
		//	setSliderValue(valueUnwrapped);
		//	// subscribe to the slider's change event and update the bound observable
		//	curSlider.bind('change', function () {
		//		valueAccessor()(curSlider.val());
		//	});
		//}, 0);
	}
};


ko.bindingHandlers.jqmRefreshList = {
	update: function (element, valueAccessor) {
		ko.utils.unwrapObservable(valueAccessor()); //just to create a dependency
		var listview = $(element);
		if (listview) {
			try {
				model.showLoading();
				//$(listview).trigger('create');
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

ko.bindingHandlers.jqmColorPicker = {
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

model.getStatus();
model.getElements();
model.getSequences();

var b = document.documentElement;
b.setAttribute('data-useragent', navigator.userAgent);
b.setAttribute('data-platform', navigator.platform);
b.className += ((!!('ontouchstart' in window) || !!('onmsgesturechange' in window)) ? ' touch' : '');

$(document).on('click', '.navbar-collapse.in', function (e) {
	if ($(e.target).is('a')) {
		$(this).collapse('hide');
	}
});

