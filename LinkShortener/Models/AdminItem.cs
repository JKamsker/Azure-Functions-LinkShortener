using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace LinkShortener.Models
{
    public class AdminItem
    {
        [JsonProperty(PropertyName = "id")] 
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AdminKey { get; set; }
    }
}