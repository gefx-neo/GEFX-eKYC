namespace ekyc
{
    public class Resp
    {
        public string status { get; set; }
        public int id { get; set; }
        public string access_token { get; set; }
        public CustomerResp customer { get; set; }
    }
    public class Api
    {
        public static string baseAddress = "https://api1.artemisuat.cynopsis.co";
        public static string tokenAddress = "https://crm-demo.cynopsis.co/oauth/token";
    }

    public class CustomerResp
    {
        public int id { get; set; } 
    }
    
}
