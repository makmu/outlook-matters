using System;

namespace OutlookMatters.Mattermost
{
    public class PostIdFromPermalinkFilter : IStringProvider
    {
        private readonly IStringProvider _baseProvider;

        public PostIdFromPermalinkFilter(IStringProvider baseProvider)
        {
            _baseProvider = baseProvider;
        }

        public string Get()
        {
            var uri = new Uri(_baseProvider.Get());
            return uri.Segments[3];
        }
    }
}