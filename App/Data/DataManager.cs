using App.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Data
{
    public class DataManager
    {
        public static List<string> listOfMainLinks;
        List<HtmlDocument> listOfAllDocuments;
        List<Product> productList;
        static HtmlWeb web;
        static List<Product> getHarvyNormanProductList;
        HtmlDocument doc;

        public DataManager()
        {
            listOfMainLinks = new List<string>();
            productList = new List<Product>();
            listOfAllDocuments = new List<HtmlDocument>();
            listOfMainLinks.Add("https://www.harveynorman.com.au/headphones-audio-music/headphones/all-headphones?p=");
            listOfMainLinks.Add("https://www.jbhifi.com.au/headphones-speakers-audio/headphones/?p=");
            Common();

        }



        private async Task<List<Product>> Common()
        {

            web = new HtmlWeb();
            foreach (var link in listOfMainLinks)
                if (link.Contains("jbhifi"))
                {
                    var sellerCompany = link.Split('.')[1];
                    var builder = new StringBuilder();
                    // Append to StringBuilder.
                    for (int i = 1; i < 15; i++)
                    {
                        var url = builder.Append(link.ToString()).Append(i).Append("&s=releaseDate&sd=2").ToString();

                        var doc = await Task.Run(() => web.Load(url));

                        if (doc != null)
                        {
                            var selectedNodes = doc.DocumentNode.SelectNodes("//div[@class='span03 product-tile']");
                            foreach (var htmldoc in selectedNodes)
                            {
                                try
                                {
                                    var product = new Product();

                                    product.Name = htmldoc.SelectSingleNode(".//h4").InnerText;
                                    if (!string.IsNullOrEmpty(product.Name))
                                    {
                                        product.Brand = product.Name.Substring(0, product.Name.IndexOf(" "));

                                        if (htmldoc.SelectSingleNode(".//span[@class='amount onSale']") != null)
                                            product.Price = htmldoc.SelectSingleNode(".//span[@class='amount onSale']").InnerText.Replace(System.Environment.NewLine, "").Replace(" ", string.Empty);
                                        else if (htmldoc.SelectSingleNode(".//span[@class='amount regular']") != null)
                                            product.Price = htmldoc.SelectSingleNode(".//span[@class='amount regular']").InnerText.Replace(System.Environment.NewLine, "").Replace(" ", string.Empty);

                                        product.SellerCompany = sellerCompany;

                                        productList.Add(product);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }


                            }
                        }

                        else
                            break;
                    }
                    //POST(productList);
                    return productList;
                }

            return null;
        }
        public static void POST(Dictionary<string,Product> objEmployee)
        {
            string url = "http://localhost:55556/api/values";
            object result = string.Empty;
            // Uses the System.Net.WebClient and not HttpClient, because .NET 2.0 must be supported.
            using (var client = new WebClient())
            {
                // Set the header so it knows we are sending JSON.
                client.Headers[HttpRequestHeader.ContentType] = "application/json";

                // Serialise the data we are sending in to JSON
                string serialisedData = JsonConvert.SerializeObject(objEmployee);
                // Make the request
                var response = client.UploadString(url, serialisedData);
                // Deserialise the response into a GUID
                result = JsonConvert.DeserializeObject(response);
            }

        }
        public static List<Product> GetAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return null;
            var url = new Uri("http://localhost:55556/api/values/" + uri);
            try
            {

                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                req.Method = "GET";
                req.ContentType = "application/json";
                req.ContentLength = 0;
                //req.Timeout = 60 * 1000;
                WebResponse response = req.GetResponse();
                Stream webStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                String responseText = responseReader.ReadToEnd();
                var data = JsonConvert.DeserializeObject<List<Product>>(responseText);
                return data;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }
        public async static Task<Dictionary<string, Product>> jbhifiDataTemplate()
        {
            string URL = "https://www.jbhifi.com.au";
            Dictionary<string, Product> dictionary = new Dictionary<string, Product>();
            List<string> links = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = hw.Load(URL);
            var selectedNodes = doc.DocumentNode.SelectNodes("//div[@id='products_menu']//ul[@class='products navigation']//li//h3//a[@href]");
            if (selectedNodes != null)
            {
                foreach (HtmlNode link in selectedNodes)
                {
                    //a[@href]
                    // Get the value of the HREF attribute
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    //if (hrefValue.Length > 20)
                    //{
                    hrefValue = URL + hrefValue;

                    doc = hw.Load(hrefValue);
                    var innerNodes = doc.DocumentNode.SelectNodes("//div[@class='secondary']//ul[@class='collapsable show-hide category-menu']//a[@href]");
                    if (innerNodes != null)
                    {
                        foreach (HtmlNode innerlink in innerNodes)
                        {
                            string hrefinnerValue = innerlink.GetAttributeValue("href", string.Empty);
                            //if (hrefValue.Length > 20)
                            //{
                            hrefinnerValue = URL + hrefinnerValue;
                            if (!hrefinnerValue.Contains("all") && !hrefinnerValue.Contains("#"))
                            {

                                // Append to StringBuilder.
                                for (int i = 1; i < 5; i++)
                                {
                                    var builder = new StringBuilder();
                                    var url = builder.Append(hrefinnerValue.ToString()).Append("?p=").Append(i).Append("&s=releaseDate&sd=2").ToString();

                                    try
                                    {
                                        doc = await Task.Run(() => hw.Load(url));
                                    }
                                    catch (Exception ex)
                                    {
                                        doc = null;
                                    }


                                    if (doc != null)
                                    {

                                        var productNodes = doc.DocumentNode.SelectNodes("//div[@class='span03 product-tile']");
                                        foreach (var htmldoc in productNodes)
                                        {
                                            try
                                            {
                                                var product = new Product();

                                                product.Name = htmldoc.SelectSingleNode(".//h4").InnerText;
                                                if (!string.IsNullOrEmpty(product.Name))
                                                {
                                                    product.Brand = product.Name.Substring(0, product.Name.IndexOf(" "));

                                                    if (htmldoc.SelectSingleNode(".//span[@class='amount onSale']") != null)
                                                        product.Price = htmldoc.SelectSingleNode(".//span[@class='amount onSale']").InnerText.Replace(System.Environment.NewLine, "").Replace(" ", string.Empty);
                                                    else if (htmldoc.SelectSingleNode(".//span[@class='amount regular']") != null)
                                                        product.Price = htmldoc.SelectSingleNode(".//span[@class='amount regular']").InnerText.Replace(System.Environment.NewLine, "").Replace(" ", string.Empty);

                                                    product.SellerCompany = "JBHIFI";

                                                    if (!dictionary.ContainsKey(product.Name))
                                                        dictionary.Add(product.Name, product);

                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }


                                        }
                                    }
                                }

                            }

                        }
                    }                    
                }
            }
            POST(dictionary);
            return dictionary;
        }
        public static void harvynormanDataTemplate()
        {
            string URL = "https://www.harveynorman.com.au";
            Dictionary<string, Product> dictionary = new Dictionary<string, Product>();
            List<string> links = new List<string>();
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = hw.Load(URL);
            var selectedNodes = doc.DocumentNode.SelectNodes("//div[@class='navmega']//div[@id='nav']//a[@href]");
            if (selectedNodes != null)
            {
                foreach (HtmlNode link in selectedNodes)
                {
                    //a[@href]
                    // Get the value of the HREF attribute
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (hrefValue.Length > 20)
                    {
                        doc = hw.Load(hrefValue);
                        if (doc == null)
                            return;
                        var innerSelectedNodes = doc.DocumentNode.SelectNodes("//div[@class='minisite-browse-grid']//div[@class='photo-box']//a[@href]");
                        if (innerSelectedNodes != null)
                        {
                            foreach (HtmlNode innerLink in innerSelectedNodes)
                            {
                                var innerhrefValue = string.Empty;
                                for (var i = 1; i < 5; i++)
                                {
                                    innerhrefValue = URL + innerLink.GetAttributeValue("href", string.Empty) + "?p=" + i;
                                    if (Regex.Matches(innerhrefValue, "harveynorman").Count == 1)
                                    {
                                        doc = hw.Load(innerhrefValue);
                                        var productNodes = doc.DocumentNode.SelectNodes("//div[@id='category-grid']//div[@class='hproduct stock-in panel panel_product cfx']");
                                        if (productNodes != null)
                                        {
                                            var product = new Product();
                                            foreach (HtmlNode productDeatils in productNodes)
                                            {
                                                product.Name = productDeatils.SelectSingleNode(".//div[@class='info']//a").InnerText;
                                                if (product.Name.Length > 20)
                                                    product.Brand = product.Name.Substring(0, product.Name.IndexOf(" "));
                                                if (productDeatils.SelectSingleNode(".//div[@class='price-device']//span[@class='price']") != null)
                                                    product.Price = productDeatils.SelectSingleNode(".//div[@class='price-device']//span[@class='price']").InnerText;
                                                else
                                                    return;
                                                product.SellerCompany = "HARVEYNORMAN";

                                                if (!dictionary.ContainsKey(product.Name))
                                                    dictionary.Add(product.Name, product);

                                            }
                                        }
                                    }

                                }


                            }
                        }

                    }

                }
            }



        }
        private void officeworksDataTemplate()
        {

        }

    }
}
