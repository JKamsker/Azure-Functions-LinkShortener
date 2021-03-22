namespace LinkShortener
{
    public class LinkItemAdminDto
    {
        //Optional: The path the user will see
        public string Id { get; set; }

        /// <summary>
        /// The url that the user will be relayed to
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Required if the link should be updated
        /// </summary>
        public string AccessKey { get; set; }

        //http://localhost:7071/api/link/0W7YHQEOOV
        public string ShortenedLink { get; set; }

        public LinkItemAdminDto SetHost(string host)
        {
            ShortenedLink = $"{host}/{Id}";
            return this;
        }
    }
}