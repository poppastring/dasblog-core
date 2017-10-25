/**
 * Version: 1.4.0
 * Build Date: 27-Aug-2008
 * Copyright (c) 2006-2008, Coolite Inc. (http://www.coolite.com/). All rights reserved.
 * Website: http://www.coolite.com/
 */
Coolite.TimePicker=function(config){if(!config||config===null){return null;}
var defaults={timeFormat:"h:mm tt",displayType:"SingleDropDownList",is24Hour:null,nullTimeText:"{0}",hourNullText:"",minuteNullText:"",secondNullText:"",readOnly:false,nullable:true,autopostback:false,hourIncrement:-1,minuteIncrement:-1,secondIncrement:-1,minimumTime:null,maximumTime:null,events:{selectionChanged:""},styles:{},cssClasses:{}};for(var p in defaults){this[p]=defaults[p];if(p=="events"||p=="styles"||p=="cssClasses"){for(var p2 in defaults[p]){this[p][p2]=defaults[p][p2];}}}
for(var c in config){this[c]=config[c];if(c=="events"||c=="styles"||c=="cssClasses"){for(var c2 in config[c]){this[c][c2]=config[c][c2];}}}
this._isTime=true;this.input=null;this.hourInput=null;this.minuteInput=null;this.secondInput=null;this.designatorInput=null;this.isMultiInput=false;this.hasHourInput=false;this.hasMinuteInput=false;this.hasSecondInput=false;this.hasDesignatorInput=false;this.applyTo=function(config){if(!config||config===null){return null;}
if(config.input){this.input=Coolite.Dom.get(config.input);Coolite.Event.addListener(this.input,'change',this._selectionChanged,this);this.hasInput=true;}else{this.isMultiInput=true;var inputs="hour minute second designator".split(" "),temp="";for(var i=0;i<inputs.length;i++){temp=inputs[i]+"Input";if(config[temp]){this[temp]=Coolite.Dom.get(config[temp]);Coolite.Event.addListener(this[temp],'change',this._selectionChanged,this);this["has"+temp.substring(0,1).toUpperCase()+temp.substring(1)]=true;}}
this._syncInputs();}
$COOL.controls.push({type:"TimePicker",control:this});return this;};this._selectionChanged=function(ev,el){if(!$COOL.isNullOrEmpty(el.events.selectionChanged)){window[el.events.selectionChanged](el,null);}
el._syncInputs();el._validateDatePicker();return;};this.rebindMinutes=function(){var value;this.minuteInput.options.length=0;for(var i=0;i<=59;i++){value=(i<10)?'0'+i:i;this.minuteInput.options[i]=new Option(value,value);}};this._syncInputs=function(){if(this.displayType=="MultipleDropDownList"&&this.hourInput!=null&&this.minuteInput!=null&&!this.nullable){var selectedMinute=parseInt(this.minuteInput.options[this.minuteInput.selectedIndex].value,10),end;this.rebindMinutes();var value;if(this.getMinimumTime()!==null&&this.getSelectedTime().getHours()==this.getMinimumTime().getHours()){for(var i=0;i<this.minuteInput.options.length;i++){if(parseInt(this.minuteInput.options[i].value,10)<this.getMinimumTime().getMinutes()){this.minuteInput.remove(i);i--;}}}else if(this.getMaximumTime()!==null&&this.getSelectedTime().getHours()==this.getMaximumTime().getHours()){for(var i=0;i<this.minuteInput.options.length;i++){value=parseInt(this.minuteInput.options[i].value,10);if(value>this.getMaximumTime().getMinutes()){this.minuteInput.remove(i);i--;}}}
end=this.minuteInput.options.length-1;if(selectedMinute<parseInt(this.minuteInput.options[0].value,10)){this.minuteInput.options[0].selected=true;}else if(selectedMinute>parseInt(this.minuteInput.options[end].value,10)){this.minuteInput.options[end].selected=true;}else{for(var i=0;i<=end;i++){value=parseInt(this.minuteInput.options[i].value,10);if(selectedMinute==value){this.minuteInput.options[i].selected=true;return;}}}}};this.common=new Coolite.Common();this.setSelectedTime=function(time){if(time===null){var useSpecificNullText=(this.nullTimeText=="{0}");if(this.isMultiInput){if(this.hasHourInput){this.setInputValue(this.hourInput,(useSpecificNullText)?this.hourNullText:this.nullTimeText);}
if(this.hasMinuteInput){this.setInputValue(this.minuteInput,(useSpecificNullText)?this.minuteNullText:this.nullTimeText);}
if(this.hasSecondInput){this.setInputValue(this.secondInput,(useSpecificNullText)?this.secondNullText:this.nullTimeText);}
if(this.hasDesignatorInput){this.setInputValue(this.designatorInput,Date.CultureInfo.amDesignator);}}else{this.setInputValue(this.input,(useSpecificNullText)?this.timeFormat:this.nullTimeText);}
return this;}
if(time instanceof Date){time=new TimeSpan(0,time.getHours(),time.getMinutes(),time.getSeconds());}
var tokens=$COOL.tokenize(this.timeFormat);var hourToken=null,minuteToken=null,secondToken=null,designatorToken=null;for(var i=0;i<tokens.length;i++){var token=tokens[i];switch(token.toLowerCase()){case"h":case"hh":if(!hourToken){hourToken=token;}
break;case"m":case"mm":if(!minuteToken){minuteToken=token;}
break;case"s":case"ss":if(!secondToken){secondToken=token;}
break;case"t":case"tt":if(!designatorToken){designatorToken=token;}
break;}}
if(this.isMultiInput){if(this.hasHourInput){this.setInputValue(this.hourInput,time.toString(hourToken));}
if(this.hasMinuteInput){this.setInputValue(this.minuteInput,time.toString(minuteToken));}
if(this.hasSecondInput){this.setInputValue(this.secondInput,time.toString(secondToken));}
if(this.hasDesignatorInput){this.setInputValue(this.designatorInput,time.toString(designatorToken));}}else{this.setInputValue(this.input,time.toString(this.timeFormat));}
if(!$COOL.isNullOrEmpty(this.onClientSelectionChanged)){window[this.onClientSelectionChanged](this,null);}
if(this.autoPostBack){eval(this.postBackFunction);}
return this;};this.getSelectedTime=function(){var hourText=null,minuteText=null,secondText=null,designatorText=null;var hour=null,minute=null,second=null;if(this.isMultiInput){hourText=this.getInputValue(this.hourInput);minuteText=this.getInputValue(this.minuteInput);secondText=this.getInputValue(this.secondInput);designatorText=this.getInputValue(this.designatorInput);if((hourText===this.hourNullText||!$COOL.isNumeric(hourText))||(minuteText===this.minuteNullText||!$COOL.isNumeric(hourText))||(secondText===this.secondNullText||!$COOL.isNumeric(hourText))){return null;}
hour=this.parseIntFromMultiInput(hourText,this.hourNullText);minute=this.parseIntFromMultiInput(minuteText,this.minuteNullText);second=this.parseIntFromMultiInput(secondText,this.secondNullText);if(!this.is24Hour&&designatorText!==null){if(designatorText==Date.CultureInfo.amDesignator&&hour==12){hour=0;}else if(designatorText==Date.CultureInfo.pmDesignator&&hour!=12){hour=hour+12;}}
return new TimeSpan(0,hour,minute,second);}else{var designators=[Date.CultureInfo.amDesignator,Date.CultureInfo.pmDesignator];var timeText=this.common.trim(this.getInputValue(this.input));if(timeText.length===0||timeText.toLowerCase()==this.nullTimeText.toLowerCase()||timeText.toLowerCase()==this.timeFormat.toLowerCase()){return null;}
var temp=Date.parse(timeText);return new TimeSpan(0,temp.getHours(),temp.getMinutes(),temp.getSeconds());}};this.getSelectedTimeFormatted=function(){var time=this.getSelectedTime();return(time===null)?"":time.toString(this.timeFormat);};this.getMinimumTime=function(){return this.minimumTime;};this.setMinimumTime=function(time){this.minimumTime=time;return this;};this.getMaximumTime=function(){return this.maximumTime;};this.setMaximumTime=function(time){this.maximumTime=time;return this;};this.parseIntFromMultiInput=function(numberText,nullText){if(!$COOL.isNullOrEmpty(numberText)&&$COOL.isNumeric(numberText)&&numberText!=nullText){return parseInt(numberText,10);}
return 0;};this.getInputValue=function(input){return(!input)?null:(input.options)?input.options[input.selectedIndex].value:input.value;};this.setInputValue=function(input,value){if(input){var options=input.options;if(options){for(var i=0;i<options.length;i++){if(options[i].value==value){input.selectedIndex=i;$COOL.fireOnChange(input);this._validateDatePicker();return;}}
var opt=document.createElement("OPTION");opt.value=value;opt.text=value;options.add(opt,0);input.selectedIndex=0;}else{input.value=value;}
$COOL.fireOnChange(input);}
return this;};this._validateDatePicker=function(){for(var j=0;j<$COOL.controls.length;j++){if($COOL.controls[j].type==="DatePicker"){var dp=$COOL.controls[j].control;if(dp.getTimePicker()!==null){if(dp.getTimePicker().clientID===this.clientID){$COOL.fireOnChange(dp.input);}}}}};this.clear=function(){return this.setSelectedTime(null,true);};this.getIsNull=function(){return(this.getSelectedTime()===null);};this.getIsTime=function(){return this._isTime;};this.setEnabled=function(val){if(this.isMultiInput){var parent;if(this.hasHourInput){this.hourInput.disabled=!val;parent=this.hourInput;}
if(this.hasMinuteInput){this.minuteInput.disabled=!val;parent=this.minuteInput;}
if(this.hasSecondInput){this.secondInput.disabled=!val;parent=this.secondInput;}
if(this.hasDesignatorInput){this.designatorInput.disabled=!val;parent=this.designatorInput;}
if(parent.parentElement){parent.parentElement.disabled=!val;}}
else{this.input.disabled=!val;if(this.input.parentElement){this.input.parentElement.disabled=!val;}}
return this;};this.getEnabled=function()
{if(this.isMultiInput)
{if(this.hasHourInput){return!this.hourInput.disabled;}
if(this.hasMinuteInput){return!this.minuteInput.disabled;}
if(this.hasSecondInput){return!this.secondInput.disabled;}
if(this.hasDesignatorInput){return!this.designatorInput.disabled;}}
return!this.input.disabled;};this.getHourIncrement=function(){return(this.hourIncrement==-1)?0:this.hourIncrement;};this.getMinuteIncrement=function(){return(this.minuteIncrement==-1&&this.displayType=="SingleDropDownList")?30:(this.minuteIncrement==-1)?15:this.minuteIncrement;};this.getSecondIncrement=function(){return(this.secondIncrement==-1)?0:this.secondIncrement;};this.increment=function(val,token){var t=(this.getSelectedTime()===null)?new TimeSpan():this.getSelectedTime();if(val&&this.common.isNumeric(val)){if(token){switch(token.toLowerCase()){case"d":this.setSelectedTime(t.addDays(val));break;case"h":this.setSelectedTime(t.addHours(val));break;case"m":this.setSelectedTime(t.addMinutes(val));break;case"s":this.setSelectedTime(t.addSeconds(val));break;}}else{this.setSelectedTime(t.addMinutes(val));}
return this;}
return this.setSelectedTime(t.addHours(this.getHourIncrement()).addMinutes(this.getMinuteIncrement()).addSeconds(this.getSecondIncrement()));};this.decrement=function(val,token){var t=(this.getSelectedTime()===null)?new TimeSpan():this.getSelectedTime();if(val&&this.common.isNumeric(val)){if(token){switch(token.toLowerCase()){case"d":this.setSelectedTime(t.addDays(-val));break;case"h":this.setSelectedTime(t.addHours(-val));break;case"m":this.setSelectedTime(t.addMinutes(-val));break;case"s":this.setSelectedTime(t.addSeconds(val));break;}}else{this.setSelectedTime(t.addMinutes(-val));}
return this;}
return this.setSelectedTime(t.addHours(-this.getHourIncrement()).addMinutes(-this.getMinuteIncrement()).addSeconds(-this.getSecondIncrement()));};};
if(typeof Sys!=="undefined"){Sys.Application.notifyScriptLoaded();}
