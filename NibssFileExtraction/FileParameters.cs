using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NibssFileExtraction
{
    public class FileParameters
    {
        [JsonProperty("Product")]
        public string Product { get; set; }
        [JsonProperty("Direction")]
        public string Direction { get; set; }
        [JsonProperty("File")]
        public string File { get; set; }
    }

}
