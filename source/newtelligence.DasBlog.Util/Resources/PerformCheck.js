<script type="text/javascript" language="JavaScript">
//<![CDATA[
function performCheck(checkbox, controlIDs)
{
	var controls = controlIDs.split(",");
	var disabled = !document.getElementById(checkbox).checked;
	
	for(i = 0; i < controls.length; i++)
	{
		document.getElementById(controls[i]).disabled = disabled;
	}
}
//]]>
</script>