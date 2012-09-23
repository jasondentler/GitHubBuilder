namespace GitHubBuilder.Model
{
    public class Comment
    {

        public int Id { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
        public User User { get; set; }

    }
}
