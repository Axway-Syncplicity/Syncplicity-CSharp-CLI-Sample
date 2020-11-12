namespace CSharpSampleApp.Entities.Events
{
    public class NewsFeedEvent
    {
        public string Id { get; set; }

        public string App { get; set; }

        public string Event { get; set; }

        public long TimeStamp { get; set; }

        public int Version { get; set; }

        public NewsFeedEventData Data { get; set; }
    }
}
