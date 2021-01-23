namespace LinkShortener
{
    public class LinkItemUpdateDto
    {
        /// <summary>
        /// The url that the user will be relayed to
        /// </summary>
        public string Url { get; set; }

        public string AccessKey { get; set; }
    }
}