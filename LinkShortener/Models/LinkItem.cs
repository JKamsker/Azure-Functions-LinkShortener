using Newtonsoft.Json;

namespace LinkShortener
{
    public class LinkItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The link the user is relayed to
        /// </summary>
        public string Url { get; set; }

        public string AccessKey { get; set; }
    }
}