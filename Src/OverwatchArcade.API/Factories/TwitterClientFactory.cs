using Tweetinvi;

namespace OverwatchArcade.API.Factories
{
    public class TwitterClientFactory : ITwitterClientFactory
    {
        private readonly IConfiguration _configuration;

        public TwitterClientFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public ITwitterClient Create()
        {
            return new TwitterClient(
                _configuration.GetValue<string>("Twitter:ConsumerKey"),
                _configuration.GetValue<string>("Twitter:ConsumerSecret"),
                _configuration.GetValue<string>("Twitter:TokenValue"),
                _configuration.GetValue<string>("Twitter:TokenSecret")
            );
        }
    }
}