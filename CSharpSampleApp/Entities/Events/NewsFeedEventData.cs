namespace CSharpSampleApp.Entities.Events
{
    public class NewsFeedEventData
    {
        public int? TypeId { get; set; }

        public NewsFeedEventCompany Company { get; set; }

        public NewsFeedEventSource Source { get; set; }

        public NewsFeedEventFile File { get; set; }

        public NewsFeedEventFolder Folder { get; set; }

        public NewsFeedEventSyncpoint Syncpoint { get; set; }
    }
}
