using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace fstonge.AspNetCore.Session.Distributed.Services
{
    public class EnforcedAsyncDistributedSession : ISession
    {
        private readonly DistributedSession _distributedSession;
        private bool _isLoadedAsync;

        public EnforcedAsyncDistributedSession(
            IDistributedCache cache,
            string sessionKey,
            TimeSpan idleTimeout,
            TimeSpan ioTimeout,
            Func<bool> tryEstablishSession,
            ILoggerFactory loggerFactory,
            bool isNewSessionKey)
        {
            _distributedSession = new DistributedSession(
                cache,
                sessionKey,
                idleTimeout,
                ioTimeout,
                tryEstablishSession,
                loggerFactory,
                isNewSessionKey);
        }

        public bool IsAvailable => _distributedSession.IsAvailable;

        public string Id => _distributedSession.Id;

        public IEnumerable<string> Keys => _distributedSession.Keys;

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            await _distributedSession.LoadAsync(cancellationToken);
            _isLoadedAsync = IsAvailable;
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _distributedSession.CommitAsync(cancellationToken);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (!_isLoadedAsync)
            {
                throw new InvalidOperationException("Not loaded asynchronously");
            }

            return _distributedSession.TryGetValue(key, out value);
        }

        public void Set(string key, byte[] value)
        {
            if (!_isLoadedAsync)
            {
                throw new InvalidOperationException("Not loaded asynchronously");
            }

            _distributedSession.Set(key, value);
        }

        public void Remove(string key)
        {
            if (!_isLoadedAsync)
            {
                throw new InvalidOperationException("Not loaded asynchronously");
            }

            _distributedSession.Remove(key);
        }

        public void Clear()
        {
            if (!_isLoadedAsync)
            {
                throw new InvalidOperationException("Not loaded asynchronously");
            }

            _distributedSession.Clear();
        }
    }
}