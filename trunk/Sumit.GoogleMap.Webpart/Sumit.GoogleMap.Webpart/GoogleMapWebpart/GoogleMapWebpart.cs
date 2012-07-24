using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.WebPartPages;

namespace Sumit.GoogleMap.Webpart.GoogleMapWebpart
{
    [ToolboxItemAttribute(false)]
    public class GoogleMapWebpart : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Sumit.GoogleMap.Webpart/GoogleMapWebpart/GoogleMapWebpartUserControl.ascx";

        #region WebPart Properties

        private string _lattitude = "28.60502008328845";
        [WebBrowsable(false),
        Category("Google Map Settings"),
        Personalizable(PersonalizationScope.Shared),
        Description("Location Lattitude"),
        WebDisplayName("Lattitude")]
        public string Lattitude
        {
            get
            {
                return _lattitude;
            }
            set
            {
                _lattitude = value;
            }
        }


        private string _longitude = "77.36267566680908";
        [WebBrowsable(false),
        Category("Google Map Settings"),
        Personalizable(PersonalizationScope.Shared),
        Description("Location Longitude"),
        WebDisplayName("Longitude")]
        public string Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
            }
        }

        private string _address = "Your Location Address";
        [WebBrowsable(false),
        Category("Google Map Settings"),
        Personalizable(PersonalizationScope.Shared),
        Description("Location Address"),
        WebDisplayName("Address")]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        private string _gmTitle = "Your Location Title";
        [WebBrowsable(false),
        Category("Google Map Settings"),
        Personalizable(PersonalizationScope.Shared),
        Description("Location Title"),
        WebDisplayName("Title")]
        public string GMTitle
        {
            get
            {
                return _gmTitle;
            }
            set
            {
                _gmTitle = value;
            }
        }
        #endregion

        #region Global Declaration
        public static string tbLattitudeID = string.Empty;
        public static string tbLongitudeID = string.Empty;
        public static string tbAddressID = string.Empty;
        public static string tbTitleID = string.Empty;
        private string tempAddValue = string.Empty;
        private string tempLatValue = string.Empty;
        private string tempLngValue = string.Empty;
        private string tempTitleValue = string.Empty;
        private bool isLocationChngd = false;
        private bool isAddressChngd = false;
        private bool isTitleChngd = false;
        #endregion


        protected override void CreateChildControls()
        {
            #region declarations

            #endregion

            #region Load Control
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
            #endregion

            #region Update Property if Location is changed

            VerifyIsLocationChanged();

            if (isAddressChngd || isLocationChngd)
            {
                //Save the value to the custom property
                UpdateCustomProperty();
            }
            #endregion

            #region Create Hidden Field

            //Hidden Field to store Lattitude and longitude
            HiddenField hfLatLong = new HiddenField();
            hfLatLong.Value = this.Lattitude + "," + this.Longitude;
            Controls.Add(hfLatLong);

            //Hidden Field to store Address
            HiddenField hfAddress = new HiddenField();
            hfAddress.Value = this.Address;
            Controls.Add(hfAddress);

            //Hidden Field to store Title
            HiddenField hfTitle = new HiddenField();
            hfTitle.Value = this.GMTitle;
            Controls.Add(hfTitle);

            //Store the control Id in ViewState
            ViewState[Constants.HIDDEN_FIELD_LOCATION_KEY] = SerializeString(hfLatLong.ClientID);
            ViewState[Constants.HIDDEN_FIELD_ADDRESS_KEY] = SerializeString(hfAddress.ClientID);
            ViewState[Constants.HIDDEN_FIELD_TITLE_KEY] = SerializeString(hfTitle.ClientID);

            #endregion

            #region Register Map Script

            if (this.WebPartManager.DisplayMode == WebPartManager.EditDisplayMode)
            {

                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditableMap", "ShowDragableMarkerMap('" + SerializeString(hfLatLong.ClientID)
                    + "','" + SerializeString(this.Lattitude + "," + this.Longitude)
                    + "','" + SerializeString(tbLattitudeID + "," + tbLongitudeID)
                    + "','" + SerializeString(tbAddressID + "," + hfAddress.ClientID)
                    + "','" + SerializeString(this.Address)
                    + "','" + SerializeString(tbTitleID + "," + hfTitle.ClientID)
                    + "','" + SerializeString(this.GMTitle)
                    + "')", true);
            }
            else
            {
                string latlan = this.Lattitude + "%2C" + this.Longitude;

                string googleMapSrc = "http://www.google.com/uds/modules/elements/mapselement/iframe.html?maptype=roadmap&latlng=" + latlan + "&mlatlng=" + latlan + "&maddress1=" + this.Address + "&zoom=9&mtitle=" + this.GMTitle + "&element=true";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowGoogleMaps", "ShowMap('" + googleMapSrc + "');", true);
            }
            #endregion
        }

        /// <summary>
        /// returns the serialized string
        /// </summary>
        /// <param name="serializingString"></param>
        /// <returns></returns>
        private string SerializeString(string serializingString)
        {
            string serialized = (new JavaScriptSerializer()).Serialize(serializingString);
            serialized = serialized.Remove(0, 1);
            serialized = serialized.Remove(serialized.LastIndexOf('"'), 1);
            return serialized;
        }


        /// <summary>
        /// Updates the Custom Property Values
        /// </summary>
        private void UpdateCustomProperty()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite objSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb objWeb = objSite.OpenWeb())
                    {
                        //Get the current webPart
                        SPFile currentPage = objWeb.GetFile(Convert.ToString(HttpContext.Current.Request.Url));
                        SPLimitedWebPartManager mngr = currentPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
                        System.Web.UI.WebControls.WebParts.WebPart objCurrentWebPart = mngr.WebParts[this.ID];

                        //Update the value
                        if (isLocationChngd)
                        {
                            ((Sumit.GoogleMap.Webpart.GoogleMapWebpart.GoogleMapWebpart)(objCurrentWebPart.WebBrowsableObject)).Lattitude = this.Lattitude;
                            ((Sumit.GoogleMap.Webpart.GoogleMapWebpart.GoogleMapWebpart)(objCurrentWebPart.WebBrowsableObject)).Longitude = this.Longitude;
                        }
                        if (isAddressChngd)
                        {
                            ((Sumit.GoogleMap.Webpart.GoogleMapWebpart.GoogleMapWebpart)(objCurrentWebPart.WebBrowsableObject)).Address = this.Address;
                        }
                        if (isTitleChngd)
                        {
                            ((Sumit.GoogleMap.Webpart.GoogleMapWebpart.GoogleMapWebpart)(objCurrentWebPart.WebBrowsableObject)).GMTitle = this.GMTitle;
                        }
                        //Save Changes
                        try
                        {
                            mngr.SaveChanges(objCurrentWebPart);
                        }
                        catch { }

                    }
                }
            });
        }

        /// <summary>
        /// Checks if the location is changed
        /// </summary>
        /// <returns></returns>
        private void VerifyIsLocationChanged()
        {
            //get the value from the Hidden Variable
            GetHiddenFieldValue();

            //Address
            if (!tempAddValue.Equals(this.Address))
            {
                isAddressChngd = true;
                if (!string.IsNullOrEmpty(tempAddValue))
                    this.Address = tempAddValue;
            }

            //Title
            if (!tempTitleValue.Equals(this.GMTitle))
            {
                isTitleChngd = true;
                if (!string.IsNullOrEmpty(tempTitleValue))
                    this.GMTitle = tempTitleValue;
            }

            //Lattitude and Longitude
            if (!tempLatValue.Equals(this.Lattitude) || !tempLngValue.Equals(this.Longitude))
            {
                isAddressChngd = true;
                isLocationChngd = true;
                if (!string.IsNullOrEmpty(tempLatValue) && !string.IsNullOrEmpty(tempLngValue))
                {
                    this.Lattitude = tempLatValue;
                    this.Longitude = tempLngValue;
                }
            }
        }


        /// <summary>
        /// gets the Hidden Field Value
        /// </summary>
        /// <returns></returns>
        private void GetHiddenFieldValue()
        {
            bool isAddKeyFound = false;
            bool isLocKeyFound = false;
            bool isTitleKeyFound = false;

            //Check if the there's any key in the form
            if (HttpContext.Current.Request.Form.Keys.Count > 0)
            {
                //Check if the View State holding the Hidden Variable Id is not null
                if (ViewState[Constants.HIDDEN_FIELD_ADDRESS_KEY] != null
                    || ViewState[Constants.HIDDEN_FIELD_LOCATION_KEY] != null
                    || ViewState[Constants.HIDDEN_FIELD_TITLE_KEY] != null)
                {
                    int index = 0;

                    //Get the index of the key holding the Hidden Field Value
                    foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                    {
                        string keyID = key.Replace("$", "_");

                        if (!isLocKeyFound && keyID.Equals(Convert.ToString(ViewState[Constants.HIDDEN_FIELD_LOCATION_KEY])))
                        {
                            isLocKeyFound = true;
                            tempLatValue = HttpContext.Current.Request.Form[index].Split(',')[0];
                            tempLngValue = HttpContext.Current.Request.Form[index].Split(',')[1];
                        }
                        else if (!isAddKeyFound && keyID.Equals(Convert.ToString(ViewState[Constants.HIDDEN_FIELD_ADDRESS_KEY])))
                        {
                            isAddKeyFound = true;
                            tempAddValue = HttpContext.Current.Request.Form[index];
                        }
                        else if (!isTitleKeyFound && keyID.Equals(Convert.ToString(ViewState[Constants.HIDDEN_FIELD_TITLE_KEY])))
                        {
                            isTitleKeyFound = true;
                            tempTitleValue = HttpContext.Current.Request.Form[index];
                        }

                        if (isLocKeyFound && isAddKeyFound & isTitleKeyFound)
                        {
                            break;
                        }
                        index++;
                    }
                }
            }
        }
    }
}
