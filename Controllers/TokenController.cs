using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace ekyc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : Controller
    {
        private IConfiguration Configuration;

        public TokenController(IConfiguration _configuration, ILogger<TokenController> logger)
        {
            Configuration = _configuration;
            _logger = logger;
        }

        public TokenController()
        {
        }

        private readonly ILogger<TokenController> _logger;

        public string GetTokenDemo()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    _logger?.LogInformation(DateTime.Now.ToString() + ": Token executed successfully.");
                    Token tk = new Token();
                    //var baseAddress = "https://api1.artemisuat.cynopsis.co";
                    //var baseAddress = "http://localhost:5132";
                    var api = "";
                    //var baseAddress = "https://crm-demo.cynopsis.co/oauth/token";
                    //var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImNybS1rZXktaWQifQ.eyJzdWIiOm51bGwsInNjb3BlIjpbInJlYWQsd3JpdGUsYXJ0ZW1pcyJdLCJpc3MiOiJodHRwczovL2NybS1kZW1vLmN5bm9wc2lzLmNvIiwiZXhwIjoxNzEyOTEzMDEyLCJhdXRob3JpdGllcyI6WyJST0xFX1NFUlZJQ0UiXSwianRpIjoiZjUyNTdjY2MtNDRmMS00YzIxLWJiM2UtYTUzYTU5ODFhZjcwIiwiY2xpZW50X2lkIjoiOWQ1OWU5N2MtNzM5Ni00ZTc3LThjNzAtNWU0MGY0MDc5OTI0In0.SRoVJcpjI43CqtLP8KOEb_ChSuVBDU2Xs0UPoC4Y3zqvy74LAAAgFgCRJdbH8GBOSIUWXYvZwbumH2EwHL6cYRixlTbPRFmCGWYl5pmEoBhm_Lvogg_xb1szKcPH30qezRiW6zQL087si7W3uxpGSjhji3G0MxPTjp-F5_eQFmRSkFJtvGNgZlcKpfwsK3ffIGgklXh1j9Rk0w3AC9EA4rcBssv_WhCsxynlf-CZlp_VnmgF1PUKyD1ydubVix1k6pCbH5z0Nq5Sy-ACHBUkJq_7hiA3mvrPHUttfeIbJsxHS5sK33gPbdRXGrKzqwBvRnzBK3mZWS5EsbW3_N5qGg";

                    //client.BaseAddress = new Uri(baseAddress);
                    var contentType = new ContentType("application/x-www-form-urlencoded");
                    //client.DefaultRequestHeaders.Add(contentType);
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    //client.DefaultRequestHeaders.Add("X-Domain-ID", "174");

                    //var postData = JsonConvert.SerializeObject(userRequest);
                    //Dictionary<string, Customers> postContent = new Dictionary<string, Customers>();
                    //postContent.Add("customerDTO", cust);

                    tk.client_id = "9d59e97c-7396-4e77-8c70-5e40f4079924";
                    tk.grant_type = "client_credentials";
                    tk.client_secret = "72dbfc03c148aa5f930f1c5d84779f54";
                    var postData = SerializeObjectWithoutBackslashes(tk);
                    var contentData = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", "9d59e97c-7396-4e77-8c70-5e40f4079924"),
                        new KeyValuePair<string, string>("client_secret", "72dbfc03c148aa5f930f1c5d84779f54")
                        // Add more key-value pairs as needed
                    });

                    var response = client.PostAsync(Api.tokenAddress + api, formData).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string stringData = response.Content.ReadAsStringAsync().Result;

                        using (JsonTextReader reader = new JsonTextReader(new StringReader(stringData)))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            Resp data = serializer.Deserialize<Resp>(reader);
                            //return data.access_token;
                            //string connString = Configuration.GetConnectionString("myDb1");
                            //using (SqlConnection myConnection = new SqlConnection(connString))
                            //{
                            //    myConnection.Open();
                            //    string query = "INSERT INTO ApiToken(token,project) VALUES (@res,1)";
                            //    using (SqlCommand command = new SqlCommand(query, myConnection))
                            //    {
                            //        command.Parameters.AddWithValue("@res", data.access_token);
                            //        command.ExecuteNonQuery();
                            //    }
                            //    myConnection.Close();
                                return data.access_token;
                            //}
                        }
                    }
                    return "false";
                }
            }catch(Exception ex)
            {
                _logger.LogError(ex, DateTime.Now.ToString()+ ": An error occurred in " + System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "error";
            }
        }

        public static string SerializeObjectWithoutBackslashes(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None, 
                StringEscapeHandling = StringEscapeHandling.Default 
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
