using System.Collections.Generic;

namespace OutlookMatters.Core.Mattermost.Session
{
    public class CompositeCache : ICache
    {
        private readonly List<ICache> _caches = new List<ICache>();

        public void Add(ICache cache)
        {
            _caches.Add(cache);
        }

        public void Invalidate()
        {
            _caches.ForEach(c => c.Invalidate());
        }
    }
}