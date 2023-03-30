using AuthClient.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration config;

        public HomeController(IConfiguration config)
        {
            this.config = config;
        }


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public void GoogleRedirect()
        {
            string client_id = this.config["Authentication:Google:ClientId"];
            string client_secret = this.config["Authentication:Google:ClientSecret"];
            string redirect_url = "https://localhost:8080/signin-google";
            var url = HttpContext.Request.Query;

            if (url.Count>0)
            {
                string queryString = url.ToString();
                char[] delimiterChars = { '=' };
                string[] words = queryString.Split(delimiterChars);
                string code = words[1];
                if (code != null)
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
                    webRequest.Method = "POST";
                    var Parameters = "code=" + code + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_url + "&grant_type=authorization_code";
                    byte[] byteArray = Encoding.UTF8.GetBytes(Parameters);
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.ContentLength = byteArray.Length;
                    Stream postStream = webRequest.GetRequestStream();
                    postStream.Write(byteArray, 0, byteArray.Length);

                    postStream.Close();

                    WebResponse response = webRequest.GetResponse();

                    postStream = response.GetResponseStream();

                    StreamReader reader = new StreamReader(postStream);

                    string responseFromServer = reader.ReadToEnd();

                    GoogleAccessToken serStatus = JsonConvert.DeserializeObject<GoogleAccessToken>(responseFromServer);

                    if (serStatus != null)
                    {
                        string accessToken = string.Empty;
                        accessToken = serStatus.access_token;
                        HttpContext.Session.Set("token", Encoding.UTF8.GetBytes(accessToken));

                        if (!string.IsNullOrEmpty(accessToken))

                        {

                            //call get user information function with access token as parameter

                        }
                    }

                }
            }
        }


        public void GetUserInfo()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
