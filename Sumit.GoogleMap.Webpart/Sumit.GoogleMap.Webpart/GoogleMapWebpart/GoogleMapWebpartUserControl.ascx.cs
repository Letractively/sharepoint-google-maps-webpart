using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Sumit.GoogleMap.Webpart.GoogleMapWebpart
{
    public partial class GoogleMapWebpartUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Get the Id of controls added to the page
            GoogleMapWebpart.tbLattitudeID = tbLattitude.ClientID;
            GoogleMapWebpart.tbLongitudeID = tbLongitude.ClientID;
            GoogleMapWebpart.tbAddressID = tbAddress.ClientID;
        }
    }
}
