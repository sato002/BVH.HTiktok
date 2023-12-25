using BVH.HTiktok.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BVH.HTiktok
{
    public class OmoCaptchaProxy
    {
        private static string CAPTCHA_API_Key = "T6lzm61oHFNlnRCG0ymcYEiDWDrWzhHU6pU3pdG1YQoTGdR7hhpreVah14MgsvB2GLe8NmEAldQLd4VX";
        static HttpClient _httpClient = new HttpClient();

        public static OmoResult CreateJob(string base64Img, string type)
        {
            var body = new OmoCreateJobParam()
            {
                api_token = CAPTCHA_API_Key,
                data = new OmoCreateJobData()
                {
                    type_job_id = type,
                    image_base64 = base64Img
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var result = _httpClient.PostAsync("https://omocaptcha.com/api/createJob", content).Result;
            string resContent = result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<OmoResult>(resContent);
        }

        public static OmoResult GetResultJob(int jobId)
        {
            var body = new OmoGetJobResultParam()
            {
                api_token = CAPTCHA_API_Key,
                job_id = jobId
            };

            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var result = _httpClient.PostAsync("https://omocaptcha.com/api/getJobResult", content).Result;
            return JsonConvert.DeserializeObject<OmoResult>(result.Content.ReadAsStringAsync().Result);
        }
    }
}
