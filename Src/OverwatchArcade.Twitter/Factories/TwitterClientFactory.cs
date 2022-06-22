using Microsoft.Extensions.Configuration;
using Tweetinvi;

namespace OverwatchArcade.Twitter.Factories
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
                _configuration.GetValue<string>("Twitter:ConsumerKey") ?? throw new ArgumentNullException(_configuration.GetValue<string>("Twitter:ConsumerKey"), "Twitter ConsumerKey not found"),
                _configuration.GetValue<string>("Twitter:ConsumerSecret") ?? throw new ArgumentNullException(_configuration.GetValue<string>("Twitter:ConsumerSecret"), "Twitter ConsumerSecret not found"),
                _configuration.GetValue<string>("Twitter:TokenValue") ?? throw new ArgumentNullException(_configuration.GetValue<string>("Twitter:TokenValue"), "Twitter TokenValue not found"),
                _configuration.GetValue<string>("Twitter:TokenSecret") ?? throw new ArgumentNullException(_configuration.GetValue<string>("Twitter:TokenSecret"), "Twitter TokenSecret not found")
            );
        }
    }
}