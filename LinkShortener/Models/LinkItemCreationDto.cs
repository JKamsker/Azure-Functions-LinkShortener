namespace LinkShortener
{
    public class LinkItemCreationDto
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
    }
}