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
        #endregion

        #region Global Declaration
        public static string tbLattitudeID = string.Empty;
        public static string tbLongitudeID = string.Empty;
        public static string tbAddressID = string.Empty;
        private string tempAddValue = string.Empty;
        private string tempLatValue = string.Empty;
        private string tempLngValue = string.Empty;
        private bool isLocationChngd = false;
        private bool isAddressChngd = false;
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

            HiddenField hfLatLong = new HiddenField();
            hfLatLong.Value = this.Lattitude + "," + this.Longitude;
            Controls.Add(hfLatLong);

            HiddenField hfAddress = new HiddenField();
            hfLatLong.Value = this.Address;
            Controls.Add(hfAddress);

            //Store the control Id in ViewState
            ViewState[Constants.Hidden_Field_Location_Key] = SerializeString(hfLatLong.ClientID);
            ViewState[Constants.Hidden_Field_Address_Key] = SerializeString(hfAddress.ClientID);

            #endregion

            #region Register Draggable Map Script

            ScriptManager.RegisterStartupScript(this, GetType(), "ShowEditableMap", "ShowDragableMarkerMap('" + SerializeString(hfLatLong.ClientID) + "','"
                + SerializeString(this.Lattitude) + "','" + SerializeString(this.Longitude) + "','" + SerializeString(tbLattitudeID)
                + "','" + SerializeString(tbLongitudeID) + "','" + SerializeString(tbAddressID) + "','" + SerializeString(hfAddress.ClientID)
                + "','" + SerializeString(this.Address) + "')", true);

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

            if (!tempAddValue.Equals(this.Address))
            {
                isAddressChngd = true;
                if (!string.IsNullOrEmpty(tempAddValue))
                    this.Address = tempAddValue;
            }

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

            //Check if the there's any key in the form
            if (HttpContext.Current.Request.Form.Keys.Count > 0)
            {
                //Check if the View State holding the Hidden Variable Id is not null
                if (ViewState[Constants.Hidden_Field_Address_Key] != null || ViewState[Constants.Hidden_Field_Location_Key] != null)
                {
                    int index = 0;

                    //Get the index of the key holding the Hidden Field Value
                    foreach (string key in HttpContext.Current.Request.Form.AllKeys)
                    {
                        string keyID = key.Replace("$", "_");

                        if (keyID.Equals(Convert.ToString(ViewState[Constants.Hidden_Field_Location_Key])))
                        {
                            isLocKeyFound = true;
                            tempLatValue = HttpContext.Current.Request.Form[index].Split(',')[0];
                            tempLngValue = HttpContext.Current.Request.Form[index].Split(',')[1];
                        }
                        else if (keyID.Equals(Convert.ToString(ViewState[Constants.Hidden_Field_Address_Key])))
                        {
                            isAddKeyFound = true;
                            tempAddValue = HttpContext.Current.Request.Form[index];
                        }

                        if (isLocKeyFound && isAddKeyFound)
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
