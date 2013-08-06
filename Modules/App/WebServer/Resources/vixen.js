var elementOnTimeValue = '30';

function setValue(x) {
	elementOnTimeValue = x;
}

function turnElementOn(id, color) {
	$.post('/element/' + id + '/' + elementOnTimeValue + '/' + color);
}
