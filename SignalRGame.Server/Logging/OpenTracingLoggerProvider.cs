using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Internal;

namespace SignalRGame.Logging
{
    /// <summary>
    /// The provider for the <see cref="OpenTracingLogger"/>.
    /// </summary>
    [ProviderAlias("OpenTracing")]
    internal class OpenTracingLoggerProvider : ILoggerProvider
    {
        private readonly ITracer _tracer;
        private readonly ConcurrentDictionary<string, OpenTracingLogger> _loggers;


        private readonly AsyncLocal<ISpan?> _currentSpan = new AsyncLocal<ISpan?>();

        public OpenTracingLoggerProvider(IGlobalTracerAccessor globalTracerAccessor)
        {
            _loggers = new ConcurrentDictionary<string, OpenTracingLogger>();

            // HACK: We can't use ITracer directly here because this would lead to a StackOverflowException
            // (due to a circular dependency) if the ITracer needs a ILoggerFactory.
            // https://github.com/opentracing-contrib/csharp-netcore/issues/14

            if (globalTracerAccessor == null)
                throw new ArgumentNullException(nameof(globalTracerAccessor));

            _tracer = globalTracerAccessor.GetGlobalTracer();
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, loggerName => new OpenTracingLogger(this, _tracer, categoryName)
            {
            });
        }

        public void Dispose()
        {
        }

        internal IDisposable BeginScope(string spanName)
        {
            var parent = _currentSpan.Value;
            var newSpan = _tracer.BuildSpan(spanName).IgnoreActiveSpan().AsChildOf(parent).Start();
            _currentSpan.Value = newSpan;

            return new SpanDisposed(() =>
            {
                if (_currentSpan.Value != newSpan)
                    _currentSpan.Value.Log(new Dictionary<string, object> { { "error", "failed to close span" } });
                newSpan.Finish();
                _currentSpan.Value = parent;
            });
        }

        private class SpanDisposed : IDisposable
        {
            private Action p;

            public SpanDisposed(Action p)
            {
                this.p = p;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        p();
                    }

                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
            }
            #endregion
        }
    }
}
