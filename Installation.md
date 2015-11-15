# Introduction #

This guide will help you in installing and configuring the weather web-part step by step.

# Prerequisites #

  * SharePoint 2010 Environment
  * Internet Connection to interact with the Google Map API's

# Step 1: Installing Solution #

  * [Download](http://code.google.com/p/sharepoint-google-maps-webpart/downloads/list) WSP and unzip the Weather Web-Part archive. This contains the WSP file, deploy solution batch file, upgrade solution batch file and retract solution batch file
  * Edit the Deploy Solution  batch file and update the server URL property on which want to install the package if solution is bieng added for the first time otherwise execute the upgrade batch file
![https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/deployURL.jpg](https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/deployURL.jpg)
  * Right click on the Deploy file and select the "Run as administrator" option
![https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/Install.jpg](https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/Install.jpg)
  * This will add the solution in the solution gallery
![https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/Solution.jpg](https://sharepoint-google-maps-webpart.googlecode.com/svn/wiki/Images/Solution.jpg)
  * This will also deploy the feature in site collection features gallery on specified web portal url


  * [Configure](http://code.google.com/p/sharepoint-google-maps-webpart/wiki/Configuration) your webpart