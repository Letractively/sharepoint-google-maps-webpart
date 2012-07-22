<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleMapWebpartUserControl.ascx.cs" Inherits="Sumit.GoogleMap.Webpart.GoogleMapWebpart.GoogleMapWebpartUserControl" %>


<script type="text/javascript" src="http://maps.google.com/maps/api/js?sensor=false"></script>

<script type="text/javascript">
    var geocoder = new google.maps.Geocoder();
    var lattitude = " ";
    var longitude = " ";
    var hf_Id = " ";
    var tbLat_Id = " ";
    var tbLng_Id = " ";
    var tbAddress_ID = " ";
    var hfAdd_Id = " ";

    function geocodePosition(pos) {
        geocoder.geocode({
            latLng: pos
        }, function (responses) {
            if (responses && responses.length > 0) {
                updateMarkerAddress(responses[0].formatted_address);
            } else {
                updateMarkerAddress('Cannot determine address at this location.');
            }
        });
    }

    function updateMarkerStatus(str) {
        //document.getElementById('markerStatus').innerHTML = str;
    }

    function updateMarkerPosition(latLng) {

        document.getElementById(tbLat_Id).value = latLng.lat();     //Displays the lattitude in the text box
        document.getElementById(tbLng_Id).value = latLng.lng();     //Displays the longitude in the text box

        document.getElementById(hf_Id).value = [
    latLng.lat(),
    latLng.lng()
  ].join(', ');
    }

    function updateMarkerAddress(str) {
        document.getElementById(tbAddress_ID).value = str;
        document.getElementById(hfAdd_Id).value = str;

    }

    function initialize() {
        var latLng = new google.maps.LatLng(lattitude, longitude);
        var map = new google.maps.Map(document.getElementById('mapCanvas'), {
            zoom: 8,
            center: latLng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        });
        var marker = new google.maps.Marker({
            position: latLng,
            title: 'Point A',
            map: map,
            draggable: true
        });

        // Update current position info.
        updateMarkerPosition(latLng);
        geocodePosition(latLng);

        // Add dragging event listeners.
        google.maps.event.addListener(marker, 'dragstart', function () {
            updateMarkerAddress('Dragging...');
        });

        google.maps.event.addListener(marker, 'drag', function () {
            updateMarkerStatus('Dragging...');
            updateMarkerPosition(marker.getPosition());
        });

        google.maps.event.addListener(marker, 'dragend', function () {
            updateMarkerStatus('Drag ended');
            geocodePosition(marker.getPosition());
        });
    }


    function ShowDragableMarkerMap(hf, lat, long, tblat, tblng, tbAddress, hf_Add, addValue) {
        hf_Id = hf;
        lattitude = lat;
        longitude = long;
        tbLat_Id = tblat;
        tbLng_Id = tblng;
        tbAddress_ID = tbAddress;
        hfAdd_Id = hf_Add;

        //SHow the stored address
        document.getElementById(tbAddress_ID).value = addValue;

        // Onload handler to fire off the app.
        google.maps.event.addDomListener(window, 'load', initialize);
    }

    function AddChanged(textControl) {
        document.getElementById(tbAddress_ID).value = textControl.value;
        document.getElementById(hfAdd_Id).value = textControl.value;
    }

</script>

<style type="text/css">
    #mapCanvas
    {
        width: 500px;
        height: 400px;
        float: left;
    }
    .outerDiv
    {
        width: 500px;
        height: 400px;
        background: none repeat scroll 0 0 #FFFFFF !important;
        border: 1px solid #999999 !important;
        border-collapse: collapse !important;
        box-shadow: 5px 5px 5px #AAAAAA !important;
        padding: 2px;
    }
    .fltlft
    {
        float: left;
    }
    .fltrgt
    {
        float: right;
        padding-right:8px;
    }
    .info
    {
        margin-top: 410px;
    }
    .adddiv
    {
        width: 500px;
        float: left;
        padding: 5px;
    }
</style>

<div class="outerDiv">
    <div id="mapCanvas">
    </div>
    <table width="100%" class="info">
        <tr>
            <td class="fltlft" colspan="2">
                <asp:Label Text="Lattitude" ID="Lattitudelbl" runat="server"></asp:Label>
                <asp:TextBox ID="tbLattitude" runat="server" Text="Lattitude" Enabled="false"></asp:TextBox>
            </td>
            <td class="fltrgt">
                <asp:Label Text="Longitude" ID="Longitudelbl" runat="server"></asp:Label>
                <asp:TextBox ID="tbLongitude" runat="server" Text="Longitude" Enabled="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="adddiv">
                <asp:Label Text="Address" ID="Addresslbl" runat="server"></asp:Label>
                <asp:TextBox ID="tbAddress" runat="server" Text="Your Location Address" Width="870%"
                    onchange="AddChanged(this);"></asp:TextBox>
            </td>
        </tr>
    </table>
</div>