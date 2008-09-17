ampie version 1.5.2.0
********************************************************************************
Check documentation for help on all topics:
http://www.amcharts.com/docs/

Incase you don't find something, post your questions to support forum:
http://www.amcharts.com/forum/
********************************************************************************
1.5.2.0

New features:

JavaScript function amError(chart_id, error_message) is called when one of the
known errors occures.

You can "click" or "roll-over" the slice with JavaScript functions:

flashMovie.clickSlice(index)
flashMovie.rollOverSlice(index)
flashMovie.rollOutSlice()

Bug fix:
When <pull_out_time> was set to 0, the pie label didn't move on click. Fixed.
********************************************************************************
1.5.1.0

Bug fix: When reloading settings with reloadSettings() function, if settings
file contained data, the data wasn't refreshed. This is fixed in this version.
********************************************************************************