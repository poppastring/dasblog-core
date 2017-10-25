<%@ Control Language="c#" AutoEventWireup="True" Codebehind="TimeLineControl.ascx.cs" Inherits="newtelligence.DasBlog.Web.TimelineControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript">
var tl;
function onLoad() {
  var eventSource = new Timeline.DefaultEventSource();
  var theme = Timeline.ClassicTheme.create();
  
  var curdate = new Date()

  var bandInfos = [
    Timeline.createBandInfo({
        eventSource:    eventSource,
        date:           curdate.toGMTString(),
        width:          "80%", 
        intervalUnit:   Timeline.DateTime.DAY, 
        intervalPixels: 100
    }),
    Timeline.createBandInfo({
        showEventText:  false,
        trackHeight:    0.5,
        trackGap:       0.2,
        eventSource:    eventSource,
        date:           curdate.toGMTString(),
        width:          "20%", 
        intervalUnit:   Timeline.DateTime.MONTH, 
        intervalPixels: 150
    })
  ];
  bandInfos[1].syncWith = 0;
  bandInfos[1].highlight = true;
  bandInfos[1].eventPainter.setLayout(bandInfos[0].eventPainter.getLayout());
   
  tl = Timeline.create(document.getElementById("my-timeline"), bandInfos);
  Timeline.loadXML("timeline.ashx", function(xml, url) { eventSource.loadXML(xml, url); });

  setupFilterHighlightControls(document.getElementById("my-timeline-controls"), tl, [0,1], theme);

}

var resizeTimerID = null;
function onResize() {
    if (resizeTimerID == null) {
        resizeTimerID = window.setTimeout(function() {
            resizeTimerID = null;
            tl.layout();
        }, 500);
    }
}
</script>

<div class="bodyContentStyle">
	<div class="controls" id="my-timeline-controls"></div>
	<div id="my-timeline" style="height: 300px; border: 1px solid #aaa"></div>
	<asp:PlaceHolder id="contentPlaceHolder" runat="server"></asp:PlaceHolder>
</div>

<script type="text/javascript">
onLoad();
</script>
