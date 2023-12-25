using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVH.HTiktok.Models
{
    public class OmoCreateJobParam
    {
        public string api_token { get; set; }
        public OmoCreateJobData data { get; set; }
    }

    public class OmoCreateJobData
    {
        public string type_job_id { get; set; }
        public string image_base64 { get; set; }
    }
}
