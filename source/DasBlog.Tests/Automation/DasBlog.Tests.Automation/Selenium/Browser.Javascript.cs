namespace DasBlog.Tests.Automation.Selenium
{
	public partial class Browser
	{
		private const string isVisibleScript= @"
var elem = document.getElementById('element-id');
var dde = document.documentElement;

let isWithinViewport = true
while (elem.parentNode && elem.parentNode.getBoundingClientRect) {
	var elemDimension = elem.getBoundingClientRect();
	var elemComputedStyle = window.getComputedStyle(elem);
	var viewportDimension = {
		width: dde.clientWidth,
		height: dde.clientHeight
	};

	isWithinViewport = isWithinViewport &&
	(elemComputedStyle.display !== 'none' &&
		elemComputedStyle.visibility === 'visible' &&
		parseFloat(elemComputedStyle.opacity, 10) > 0 &&
		elemDimension.bottom > 0 &&
		elemDimension.right > 0 &&
		elemDimension.top < viewportDimension.height &&
		elemDimension.left < viewportDimension.width);

	elem = elem.parentNode;
}

return isWithinViewport;

";
	}
}
