namespace CSharpSampleApp.Entities.Events
{
    public class NewsFeedEventSource
    {
        public NewsFeedEventUser User { get; set; }

        public NewsFeedEventMachine Machine { get; set; }
    }
}