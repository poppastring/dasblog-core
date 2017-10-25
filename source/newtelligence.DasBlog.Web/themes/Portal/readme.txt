Portal theme for dasBlog by Johnny Hughes

http://johnnynine.com/blog


CHANGING THE STYLE SHEET
------------------------

There are several style sheets included with this theme.  Each style sheet and associated images are in seperate folders.

The available style sheets folders are called:

	style-default
	style-deepblue
	style-compass
	style-journal

To change the style sheet used, modify the homeTemplate.blogtemplate file and change the foldername in the <link> tag's href.

For example to change from using the "style-default" style sheet to the "style-compass" style sheet:

Change:
	<link href="themes/Portal/style-default/theme.css" type="text/css" rel="stylesheet"/>
	
To:
	<link href="themes/Portal/style-compass/theme.css" type="text/css" rel="stylesheet"/>
