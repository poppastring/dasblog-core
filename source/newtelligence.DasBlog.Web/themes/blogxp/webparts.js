function webpartExpand(htmlNode,imgNode) {
    if (document.getElementById && document.getElementById(htmlNode) != null) {
		document.getElementById(imgNode).src=wp_img_expanded;
		document.getElementById(htmlNode).className='webpartExpanded';
	}
}

function webpartCollapse(htmlNode,imgNode) {
	if (document.getElementById && document.getElementById(htmlNode) !=  null) {
		document.getElementById(imgNode).src=wp_img_collapsed;
		document.getElementById(htmlNode).className='webpartCollapsed';
	}
}

function webpartToggleExpansionStatus(nodeName) {
	htmlNode = nodeName + 'Panel';
	imgNode = nodeName + 'Image';

	if (document.getElementById && document.getElementById(htmlNode) !=  null) {
		nodeState = document.getElementById(htmlNode).className;
	}
    if (nodeState == 'webpartCollapsed') {
        webpartExpand(htmlNode,imgNode);
	}
	else {
		webpartCollapse(htmlNode,imgNode);
	}
	return false;
}