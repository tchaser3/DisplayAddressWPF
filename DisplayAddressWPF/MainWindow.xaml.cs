using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;
using System.Xml;
using System.Net;
using System.Xml.XPath;

namespace DisplayAddressWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //global variables
        string BingMapsKey = "rlOQHqvgydklMdwaQpTs~2ABi0R5AuQXzlDyIS5RJwQ~Ajh8Q9JoMtW_PkcY-IQBgLRvc-3SOz8tDR52P-UtRD1uIUksrk0mdpmhNOp8K2Nz";

        string gstrAddress = "330 Yorktown Place, B-1, Vermilion, OH";

        public MainWindow()
        {
            InitializeComponent();
        }
        public XmlDocument Geocode(string addressQuery)
        {
            //Create REST Services geocode request using Locations API
            string geocodeRequest = "http://dev.virtualearth.net/REST/v1/Locations/" + addressQuery + "?o=xml&key=" + BingMapsKey;

            //Make the request and get the response
            XmlDocument geocodeResponse = GetXmlResponse(geocodeRequest);

            return (geocodeResponse);
        }

        private XmlDocument GetXmlResponse(string requestUrl)
        {
            System.Diagnostics.Trace.WriteLine("Request URL (XML): " + requestUrl);
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                return xmlDoc;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            XmlDocument searchResponse = Geocode(gstrAddress);

            FindandDisplayNearbyPOI(searchResponse);
        }
        private void FindandDisplayNearbyPOI(XmlDocument xmlDoc)
        {
            //Get location information from geocode response 

            //Create namespace manager
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("rest", "http://schemas.microsoft.com/search/local/ws/rest/v1");
            XmlNodeList locationElements = xmlDoc.SelectNodes("//rest:Location", nsmgr);

            //Get the geocode location points that are used for display (UsageType=Display)
            XmlNodeList displayGeocodePoints = locationElements[0].SelectNodes(".//rest:GeocodePoint/rest:UsageType[.='Display']/parent::node()", nsmgr);
                string latitude = displayGeocodePoints[0].SelectSingleNode(".//rest:Latitude", nsmgr).InnerText;
                string longitude = displayGeocodePoints[0].SelectSingleNode(".//rest:Longitude", nsmgr).InnerText;


            Location location = new Location(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            Pushpin pushpin = new Pushpin();
            pushpin.Location = location;
            myMap.Children.Add(pushpin);

            //Create the Bing Spatial Data Services request to get the user-specified POI entity type near the selected point  
            /*string findNearbyPOIRequest = "http://spatial.virtualearth.net/REST/v1/data/f22876ec257b474b82fe2ffcb8393150/NavteqNA/NavteqPOIs?spatialfilter=nearby("
            + latitude + "," + longitude + "," + distance.Content + ")"
            + "&$filter=EntityTypeID%20EQ%20'" + entityTypeID.Tag + "'&$select=EntityID,DisplayName,__Distance,Latitude,Longitude,AddressLine,Locality,AdminDistrict,PostalCode&$top=10"
            + "&key=" + BingMapsKey;*/

            //Submit the Bing Spatial Data Services request and retrieve the response
            /*XmlDocument nearbyPOI = GetXmlResponse(findNearbyPOIRequest);

                //Center the map at the geocoded location and display the results
                myMap.Center = new Location(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                myMap.ZoomLevel = 12;
                DisplayResults(nearbyPOI);*/

        }
        private void AddLabel(Panel parent, string labelString)
        {
            Label dname = new Label();
            dname.Content = labelString;
            dname.Style = (Style)FindResource("AddressStyle");
            parent.Children.Add(dname);
        }

        //Add a pushpin with a label to the map
        private void AddPushpinToMap(double latitude, double longitude, string pinLabel)
        {
            Location location = new Location(latitude, longitude);
            Pushpin pushpin = new Pushpin();
            pushpin.Content = pinLabel;
            pushpin.Location = location;
            myMap.Children.Add(pushpin);
        }
        
    }
}
