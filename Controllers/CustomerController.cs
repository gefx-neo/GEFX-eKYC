using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml.Linq;
using static ekyc.Customers;

namespace ekyc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private IConfiguration Configuration;
        
        public CustomerController(IConfiguration _configuration, ILogger<CustomerController> logger)
        {
            Configuration = _configuration;
            _logger = logger;
        }        

        private readonly ILogger<CustomerController> _logger;     

        [HttpGet(Name = "SubmitKYCstatus")]
        public async Task SubmitKYCstatus()
        {
            try
            {
                string connString = Configuration.GetConnectionString("myDb1");
                string userRequest="";int custid = 0;
                List<Customers> listCustomer = new List<Customers>();
                List<NaturalCustomer> listNaturalCustomer = new List<NaturalCustomer>();
                using (SqlConnection myConnection = new SqlConnection(connString))
                {
                    string oString = "Select a.ID,a.Customer_Title,a.Company_Nationality,a.Company_ICPassport,a.Company_ContactName,d.JobTitle,a.Natural_Name,a.DOB,a.Natural_DOB, a.Company_PlaceOfRegistration,a.Company_RegisteredName," +
                        "a.Company_BusinessAddress1,a.Company_PostalCode,a.Company_BusinessAddress2,a.Company_BusinessAddress3,a.Company_RegistrationNo,a.Company_TelNo,a.Company_Email,a.Company_DateOfRegistration, " +
                        "a.CustomerType,a.Natural_EmployedEmployerName,a.Natural_DOB,a.Natural_EmployedEmployerName, a.Natural_EmployedJobTitle, a.Natural_EmployedRegisteredAddress " +
                        "from CustomerParticulars a,eKYC b LEFT JOIN CustomerAppointmentOfStaffs d ON d.CustomerParticularId=b.cust_id where a.ID=b.cust_id AND b.status=0";
                    SqlCommand oCmd = new SqlCommand(oString, myConnection);
                    //oCmd.Parameters.AddWithValue("@Fname", fName);
                    myConnection.Open();
                    TokenController tk = new TokenController();
                    string token = tk.GetTokenDemo();

                    using (SqlDataReader oReader = oCmd.ExecuteReader())
                    {                   
                        while (oReader.Read())
                        {            
                            //custcrp.active = true;
                            //cust.crps[0].id = Int32.Parse(oReader["id"].ToString()??"0");
                            if (oReader["CustomerType"].ToString() != "Natural Person")
                            {
                                Customers cust = new Customers();
                                Customers.Customer custItem = new Customers.Customer();
                                Customers.CustomerOther custOther = new CustomerOther();
                                Customers.CrpOther custCrpOther = new Customers.CrpOther();
                                Customers.Crp custcrp = new Customers.Crp();
                                Customers.CrpParticulars custcrpPart = new Customers.CrpParticulars();
                                Customers.CustomerParticulars cp = new Customers.CustomerParticulars();
                                cust.crps = new List<Crp>();
                                Customers.Role custRole = new Customers.Role();
                                custItem.domainId = new List<int>();
                                Random rnd = new Random();
                                custcrp.roles = new List<Role>();
                                custcrp.profileReferenceId = rnd.Next(100000000, 999999999).ToString();
                                custcrp.type = "INDIVIDUAL";
                                custRole.appointedDate = "2024-04-01";
                                custRole.cid = rnd.Next(100000000, 999999999).ToString();
                                //custRole.cid = "draft-4";
                                custRole.resignedDate = null;
                                custRole.role = oReader["JobTitle"].ToString() ?? "Person-in-charge";

                                custcrp.roles.Add(custRole);

                                custcrpPart.salutation = oReader["Customer_Title"].ToString().ToUpper();
                                custcrpPart.name = oReader["Company_ContactName"].ToString().ToUpper();
                                custcrpPart.gender = oReader["Customer_Title"].ToString() == "Mr" ? "MALE" : "FEMALE";
                                custcrpPart.nationality = [oReader["Company_Nationality"].ToString().ToUpper()];
                                custcrpPart.countryOfResidence = oReader["Company_PlaceOfRegistration"].ToString().ToUpper();
                                custcrpPart.identityDocumentType = oReader["Company_Nationality"].ToString().ToUpper() == "SINGAPOREAN" || oReader["Company_Nationality"].ToString().ToUpper() == "SINGAPORE" ? "NATIONAL ID" : "PASSPORT";
                                custcrpPart.identityNumber = oReader["Company_ICPassport"].ToString().ToUpper();
                                custcrpPart.countryOfBirth = oReader["Company_Nationality"].ToString().ToUpper();
                                DateTime datedob = DateTime.Parse(oReader["DOB"].ToString());
                                custcrpPart.dateOfBirth = datedob.ToString("yyyy-MM-dd");
                                custcrpPart.phone = [oReader["Company_TelNo"].ToString() ?? ""];
                                custcrpPart.email = [oReader["Company_Email"].ToString() ?? ""];
                                custcrpPart.address = [oReader["Company_BusinessAddress1"].ToString() + "," + oReader["Company_BusinessAddress2"].ToString() + "," + oReader["Company_BusinessAddress3"].ToString() + oReader["Company_PlaceOfRegistration"].ToString().ToUpper() + " " + oReader["Company_PostalCode"].ToString().ToUpper()];

                                custCrpOther.status = "CURRENT";
                                custCrpOther.sourceOfFunds = "BUSINESS REVENUE";
                                custcrp.particular = custcrpPart;
                                custcrp.other = custCrpOther;
                                cust.crps.Add(custcrp);

                                custItem.active = true;
                                //custItem.batchUploadId = 0;
                                //custItem.id = rnd.Next(100000000, 999999999);
                                //custItem.profileId = rnd.Next(100000000, 999999999);
                                custItem.profileReferenceId = oReader["ID"].ToString() ?? "0"; 
                                //custItem.referenceId = rnd.Next(100000000, 999999999).ToString();
                                custOther.entityType = "OTHERS";
                                custOther.industry = "OTHERS";
                                custOther.onBoardingMode = "FACE-TO-FACE";
                                custOther.ownershipStructureLayer = "1";
                                custOther.paymentMode = ["CHEQUE (LOCAL)"];
                                custOther.productServiceComplexity = "SIMPLE";
                                custOther.sourceOfFunds = "BUSINESS REVENUE";

                                custItem.particular = new CustomerParticulars();
                                cp.incorporated = true;
                                cp.name = oReader["Company_RegisteredName"].ToString().ToUpper();
                                cp.countryOfIncorporation = oReader["Company_PlaceOfRegistration"].ToString().ToUpper();
                                cp.countryOfOperation = [oReader["Company_PlaceOfRegistration"].ToString().ToUpper()];
                                cp.address = [oReader["Company_BusinessAddress1"].ToString() + "," + oReader["Company_BusinessAddress2"].ToString() + "," + oReader["Company_BusinessAddress3"].ToString() + oReader["Company_PlaceOfRegistration"].ToString().ToUpper() + " " + oReader["Company_PostalCode"].ToString().ToUpper()];
                                cp.incorporateNumber = oReader["Company_RegistrationNo"].ToString();
                                cp.phone = [oReader["Company_TelNo"].ToString() ?? ""];
                                cp.email = [oReader["Company_Email"].ToString() ?? ""];
                                cp.dateOfBirth = datedob.ToString("yyyy-MM-dd");
                                DateTime dateCompanyRegisDate = DateTime.Parse(oReader["Company_DateOfRegistration"].ToString());
                                cp.dateOfIncorporation = dateCompanyRegisDate.ToString("yyyy-MM-dd");

                                custItem.domainId.Add(174);
                                custItem.assigneeId = 1416;
                                custItem.type = "CORPORATE";
                                //custItem.vendorEntityGuid = newGuid.ToString();
                                //custItem.vendorName = oReader["Company_RegisteredName"].ToString();
                                custItem.particular = cp;
                                custItem.other = custOther;

                                cust.customer = custItem;
                                cust.triggerScreening = true;
                                cust.triggerScreeningForCrp = true;
                                listCustomer.Add(cust);
                            }
                            else
                            {
                                NaturalCustomer cust = new NaturalCustomer();
                                NaturalCustomer.Customer custItem = new NaturalCustomer.Customer();
                                NaturalCustomer.CustomerOther custOther = new NaturalCustomer.CustomerOther();
                                NaturalCustomer.CrpOther custCrpOther = new NaturalCustomer.CrpOther();
                                NaturalCustomer.Crp custcrp = new NaturalCustomer.Crp();
                                NaturalCustomer.CrpParticulars custcrpPart = new NaturalCustomer.CrpParticulars();
                                NaturalCustomer.CustomerParticulars cp = new NaturalCustomer.CustomerParticulars();
                                cust.crps = new List<NaturalCustomer.Crp>();
                                NaturalCustomer.Role custRole = new NaturalCustomer.Role();
                                custItem.domainId = new List<int>();
                                Random rnd = new Random();
                                custcrp.roles = new List<NaturalCustomer.Role>();
                                custcrp.profileReferenceId = rnd.Next(100000000, 999999999).ToString();
                                //custcrp.profileReferenceId = oReader["ID"].ToString()??"0";
                                custcrp.type = "CORPORATE";
                                custRole.appointedDate = "2024-04-01";
                                //custRole.cid = "draft-4";
                                custRole.resignedDate = null;
                                //custRole.role = oReader["Natural_EmployedJobTitle"].ToString() ?? "Person-in-charge";
                                custRole.role = "NOT APPLICABLE - NOT APPLICABLE";
                                custcrp.roles.Add(custRole);

                                custcrpPart.incorporated = "";
                                custcrpPart.name = oReader["Natural_EmployedEmployerName"].ToString()??"".ToUpper();                               
                                custcrpPart.countryOfOperation = ["SINGAPORE"];
                                custcrpPart.countryOfIncorporation = "SINGAPORE";               
                                custcrpPart.phone = [];
                                custcrpPart.email = [];
                                custcrpPart.address = [oReader["Natural_EmployedRegisteredAddress"].ToString()];

                                custCrpOther.status = "CURRENT";
                                custCrpOther.sourceOfFunds = "SALARY";
                                //custCrpOther.otherSourceOfFundsValid = "";
                                //custCrpOther.otherSourceOfFundsValid = "NOT APPLICABLE - NOT APPLICABLE";
                                custCrpOther.entityType = "LOCAL COMPANY";
                                custCrpOther.ownershipStructureLayer = "1";
                                custcrp.particular = custcrpPart;
                                custcrp.other = custCrpOther;
                                cust.crps.Add(custcrp);

                                custItem.active = true;
                                //custItem.batchUploadId = 0;
                                //custItem.id = rnd.Next(100000000, 999999999);
                                //custItem.profileId = rnd.Next(100000000, 999999999);
                                custItem.profileReferenceId = oReader["ID"].ToString() ?? "0"; 
                                //custItem.referenceId = rnd.Next(100000000, 999999999).ToString();
                              
                                custOther.industry = "OTHERS";
                                custOther.onBoardingMode = "FACE-TO-FACE";
                                //custOther.occupation = oReader["Natural_EmployedJobTitle"].ToString()??"OTHERS";
                                custOther.occupation = "NOT APPLICABLE - NOT APPLICABLE";
                                custOther.paymentMode = ["CHEQUE (LOCAL)"];
                                custOther.productServiceComplexity = "SIMPLE";
                                custOther.sourceOfFunds = "SALARY";
                                
                                custItem.particular = new NaturalCustomer.CustomerParticulars();                                
                                cp.name = oReader["Natural_Name"].ToString()??"".ToUpper();                              
                                cp.address = [oReader["Natural_EmployedRegisteredAddress"].ToString() ];
                                cp.countryOfBirth = oReader["Company_RegistrationNo"].ToString();
                                cp.salutation = oReader["Customer_Title"].ToString().ToUpper();
                                //cp.phone = [oReader["Company_TelNo"].ToString() ?? ""];
                                //cp.email = [oReader["Company_Email"].ToString() ?? ""];
                                DateTime datedob = DateTime.Parse(oReader["Natural_DOB"].ToString());
                                cp.dateOfBirth = datedob.ToString("yyyy-MM-dd");
                                //DateTime dateCompanyRegisDate = DateTime.Parse(oReader["Company_DateOfRegistration"].ToString());
                                cp.countryOfResidence = "SINGAPORE";
                                cp.nationality = ["SINGAPORE"];

                                custItem.domainId.Add(174);
                                custItem.assigneeId = 1416;
                                custItem.type = "INDIVIDUAL";
                                //custItem.vendorEntityGuid = newGuid.ToString();
                                //custItem.vendorName = oReader["Company_RegisteredName"].ToString();
                                custItem.particular = cp;
                                custItem.other = custOther;

                                cust.customer = custItem;
                                cust.triggerScreening = true;
                                cust.triggerScreeningForCrp = true;
                                listNaturalCustomer.Add(cust);
                            }                                                     
                        }

                        //call API
                        //using (HttpClient client = new HttpClient())
                        //{
                        //    var baseAddress = "https://api1.artemisuat.cynopsis.co";
                        //    //var baseAddress = "http://localhost:5132";
                        //    //var api = "/Customer/GetText"; 
                        //    var api = "/api/customer/createWithCrps";
                        //    var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImNybS1rZXktaWQifQ.eyJzdWIiOm51bGwsInNjb3BlIjpbInJlYWQsd3JpdGUsYXJ0ZW1pcyJdLCJpc3MiOiJodHRwczovL2NybS1kZW1vLmN5bm9wc2lzLmNvIiwiZXhwIjoxNzEyMDU0NTU5LCJhdXRob3JpdGllcyI6WyJST0xFX1NFUlZJQ0UiXSwianRpIjoiMGUwM2JkMWYtMmFkYy00ZTI1LWExOWItNzNkMDRjNmE1MzVjIiwiY2xpZW50X2lkIjoiOWQ1OWU5N2MtNzM5Ni00ZTc3LThjNzAtNWU0MGY0MDc5OTI0In0.Puyx0xEdb1mKWqugNqLYnx98rulME7-NyEZqpqUuCx0dogh3mb9y7GY_4WkcS59ks96_mXmTtoifwcAxE7fxMjCkgcdftvme98kalg7NrUZpsDQV-x-ojjDNnNE1-jiDnUzswku6RzQMIbpj1juZyV3Y1w0LHKdpL55akg3rL87Zd5ZJCasEeLWaRx0Oyyz4eQOGyvsW0872_feLs8ePDIMDsG8tS2VXTZM4JqE01cbbQYQuy6dIMEtakDgcCbWWBdmBgphN_k8M-Clu5kVXFcFRtrwV4Ckuq_jKPweMhtEiiSNtNRHqsiDkP6zsgYDpRSnY0SwoVANh9K_7zHd5Ug";

                        //    client.BaseAddress = new Uri(baseAddress);
                        //    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                        //    client.DefaultRequestHeaders.Accept.Add(contentType);
                        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        //    client.DefaultRequestHeaders.Add("X-Domain-ID", "174");

                        //    //var postData = JsonConvert.SerializeObject(userRequest);
                        //    Dictionary<string, Customers> postContent = new Dictionary<string, Customers>();
                        //    postContent.Add("customerDTO", cust);
                        //    var postData = JsonConvert.SerializeObject(postContent); 
                        //    var contentData = new StringContent(postData, Encoding.UTF8, "application/json");

                        //    var response = await client.PostAsync(baseAddress + api, contentData);

                        //    if (response.IsSuccessStatusCode)
                        //    {
                        //        var stringData = await response.Content.ReadAsStringAsync();
                        //        //return JsonConvert.DeserializeObject<UserResponse>(stringData);
                        //        //return null;
                        //        myConnection.Open();
                        //        query = "INSERT INTO ResApi (result,status) VALUES (@res,2)";
                        //        using (SqlCommand command = new SqlCommand(query, myConnection))
                        //        {
                        //            command.Parameters.AddWithValue("@res", stringData);
                        //            int result = command.ExecuteNonQuery();
                        //        }
                        //        myConnection.Close();
                        //    }
                        //    else
                        //    {
                        //    }

                        //}
                    }

                    myConnection.Close();

                    for (int i =0;i<listCustomer.Count;i++) 
                    {
                        userRequest = JsonConvert.SerializeObject(listCustomer[i]);

                        myConnection.Open();

                        String query = "INSERT INTO ResApi (result) VALUES (@res)";

                        using (SqlCommand command = new SqlCommand(query, myConnection))
                        {
                            command.Parameters.AddWithValue("@res", userRequest);
                            command.ExecuteNonQuery();
                        }
                        myConnection.Close();

                        //call here
                        using (HttpClient client = new HttpClient())
                        {
                            //var baseAddress = "http://localhost:5132";
                            //var api = "/Customer/GetText"; 
                            var url = "/api/customer/createWithCrps";


                            //var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImNybS1rZXktaWQifQ.eyJzdWIiOm51bGwsInNjb3BlIjpbInJlYWQsd3JpdGUsYXJ0ZW1pcyJdLCJpc3MiOiJodHRwczovL2NybS1kZW1vLmN5bm9wc2lzLmNvIiwiZXhwIjoxNzEyOTI2MTY4LCJhdXRob3JpdGllcyI6WyJST0xFX1NFUlZJQ0UiXSwianRpIjoiZDYxYzU2YmQtOWY1ZC00MGNlLWIzNDgtNTY4NDgzZTZhMjVmIiwiY2xpZW50X2lkIjoiOWQ1OWU5N2MtNzM5Ni00ZTc3LThjNzAtNWU0MGY0MDc5OTI0In0.VVxtcIMWHcoLUHgguthY9k_BZF5FfHODQDeiKGQYbOMooPo36bOlqFjEwo9TvqTm2cJYjBgB2XIQEjImX2fTHwzSopERQSS3w1qa_Y0KG0jlIo72rnSSaQw7mPxjncssamk8RbXi44MBH4K51vcBSID_V-9VEyK2WLSRRB-VUCFbzGvGFdBTirhElM_DbqgncCPtfmHb9R9TJi5jvVz1EQk-uzH1qtKHrhnhfN2EHBkj4FmCTxxjU2kZ3Enno4JTOS5FrHIBhhFzwmw1tJjDqGn-oOoO74QLY29dFVaTHhH19ZUCg0nJ6_b3SfQxNQdx8qg3lXPnQWbcPXK7jhIiMA";

                            //client.BaseAddress = new Uri(api.baseAddress);
                            var contentType = new ContentType("application/json");
                            //client.DefaultRequestHeaders.Add(contentType);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            client.DefaultRequestHeaders.Add("X-Domain-ID", "174");

                            //var postData = JsonConvert.SerializeObject(userRequest);
                            //Dictionary<string, Customers> postContent = new Dictionary<string, Customers>();
                            //postContent.Add("customerDTO", cust);
                            var contentData = new StringContent(userRequest, Encoding.UTF8, "application/json");

                            var response = await client.PostAsync(Api.baseAddress + url, contentData);
                            //var response = await client.GetAsync(baseAddress + api);
                            var stringData = ""; int status = 2;

                            stringData = await response.Content.ReadAsStringAsync();
                            if (!response.IsSuccessStatusCode)
                            {
                                status = 3;
                            }

                            myConnection.Open();
                            query = "INSERT INTO ResApi (result,status) VALUES (@res,@status)";
                            using (SqlCommand command = new SqlCommand(query, myConnection))
                            {
                                command.Parameters.AddWithValue("@res", stringData);
                                command.Parameters.AddWithValue("@status", status);
                                command.ExecuteNonQuery();
                            }
                            myConnection.Close();

                            if(status == 2)
                            {
                                using (JsonTextReader reader = new JsonTextReader(new StringReader(stringData)))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    Resp data = serializer.Deserialize<Resp>(reader);
                                    
                                    int cust_id = data.customer.id;
                                    myConnection.Open();
                                    query = "UPDATE eKYC SET status=1,Artemis_custId=@custid WHERE cust_id=@id";
                                    using (SqlCommand command = new SqlCommand(query, myConnection))
                                    {
                                        command.Parameters.AddWithValue("@id", listCustomer[i].customer.profileReferenceId);
                                        command.Parameters.AddWithValue("@custid", cust_id);
                                        command.ExecuteNonQuery();
                                    }
                                    myConnection.Close();
                                }
                            }
                        }
                    }

                    for (int i = 0; i < listNaturalCustomer.Count; i++)
                    {
                        userRequest = JsonConvert.SerializeObject(listNaturalCustomer[i]);

                        myConnection.Open();

                        String query = "INSERT INTO ResApi (result) VALUES (@res)";

                        using (SqlCommand command = new SqlCommand(query, myConnection))
                        {
                            command.Parameters.AddWithValue("@res", userRequest);
                            command.ExecuteNonQuery();
                        }
                        myConnection.Close();

                        //call here
                        using (HttpClient client = new HttpClient())
                        {
                            //var baseAddress = "http://localhost:5132";
                            //var api = "/Customer/GetText"; 
                            var url = "/api/customer/createWithCrps";


                            //var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImNybS1rZXktaWQifQ.eyJzdWIiOm51bGwsInNjb3BlIjpbInJlYWQsd3JpdGUsYXJ0ZW1pcyJdLCJpc3MiOiJodHRwczovL2NybS1kZW1vLmN5bm9wc2lzLmNvIiwiZXhwIjoxNzEyOTI2MTY4LCJhdXRob3JpdGllcyI6WyJST0xFX1NFUlZJQ0UiXSwianRpIjoiZDYxYzU2YmQtOWY1ZC00MGNlLWIzNDgtNTY4NDgzZTZhMjVmIiwiY2xpZW50X2lkIjoiOWQ1OWU5N2MtNzM5Ni00ZTc3LThjNzAtNWU0MGY0MDc5OTI0In0.VVxtcIMWHcoLUHgguthY9k_BZF5FfHODQDeiKGQYbOMooPo36bOlqFjEwo9TvqTm2cJYjBgB2XIQEjImX2fTHwzSopERQSS3w1qa_Y0KG0jlIo72rnSSaQw7mPxjncssamk8RbXi44MBH4K51vcBSID_V-9VEyK2WLSRRB-VUCFbzGvGFdBTirhElM_DbqgncCPtfmHb9R9TJi5jvVz1EQk-uzH1qtKHrhnhfN2EHBkj4FmCTxxjU2kZ3Enno4JTOS5FrHIBhhFzwmw1tJjDqGn-oOoO74QLY29dFVaTHhH19ZUCg0nJ6_b3SfQxNQdx8qg3lXPnQWbcPXK7jhIiMA";

                            //client.BaseAddress = new Uri(api.baseAddress);
                            var contentType = new ContentType("application/json");
                            //client.DefaultRequestHeaders.Add(contentType);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            client.DefaultRequestHeaders.Add("X-Domain-ID", "174");

                            //var postData = JsonConvert.SerializeObject(userRequest);
                            //Dictionary<string, Customers> postContent = new Dictionary<string, Customers>();
                            //postContent.Add("customerDTO", cust);
                            var contentData = new StringContent(userRequest, Encoding.UTF8, "application/json");

                            var response = await client.PostAsync(Api.baseAddress + url, contentData);
                            //var response = await client.GetAsync(baseAddress + api);
                            var stringData = ""; int status = 4;

                            stringData = await response.Content.ReadAsStringAsync();
                            if (!response.IsSuccessStatusCode)
                            {
                                status = 3;
                            }

                            myConnection.Open();
                            query = "INSERT INTO ResApi (result,status) VALUES (@res,@status)";
                            using (SqlCommand command = new SqlCommand(query, myConnection))
                            {
                                command.Parameters.AddWithValue("@res", stringData);
                                command.Parameters.AddWithValue("@status", status);
                                command.ExecuteNonQuery();
                            }
                            myConnection.Close();

                            if (status == 4)
                            {
                                using (JsonTextReader reader = new JsonTextReader(new StringReader(stringData)))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    Resp data = serializer.Deserialize<Resp>(reader);

                                    int cust_id = data.customer.id;
                                    myConnection.Open();
                                    query = "UPDATE eKYC SET status=1,Artemis_custId=@custid WHERE cust_id=@id";
                                    using (SqlCommand command = new SqlCommand(query, myConnection))
                                    {
                                        command.Parameters.AddWithValue("@id", listNaturalCustomer[i].customer.profileReferenceId);
                                        command.Parameters.AddWithValue("@custid", cust_id);
                                        command.ExecuteNonQuery();
                                    }
                                    myConnection.Close();
                                }
                            }
                        }
                    }
                }
                //return matchingPerson;
                //return "yes";
            }
            catch (Exception ex){
                _logger.LogError(ex, DateTime.Now.ToString() + ": An error occurred in " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }            
        }

        [HttpPost("GetText")]
        public String GetText([FromBody] string Reques)
        {            
            return Reques;
        }
    }
}
