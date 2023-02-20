using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

namespace MVVMToolkit.Messaging
{
    /// <summary>
    /// A <see langword="class"/> for request messages that can receive multiple replies, which can either be used directly or through derived classes.
    /// </summary>
    /// <typeparam name="T">The type of request to make.</typeparam>
    public class UniTaskCollectionRequestMessage<T> : IUniTaskAsyncEnumerable<T>
    {
        /// <summary>
        /// The collection of received replies. We accept both <see cref="Task{TResult}"/> instance, representing already running
        /// operations that can be executed in parallel, or <see cref="Func{T,TResult}"/> instances, which can be used so that multiple
        /// asynchronous operations are only started sequentially from <see cref="GetAsyncEnumerator"/> and do not overlap in time.
        /// </summary>
        private readonly List<TaskOrFunc> _responses = new();

        private struct TaskOrFunc
        {
            public UniTask<T> task;
            public Func<CancellationToken, UniTask<T>> func;
            public bool isTask;
        }

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> instance used to link the token passed to
        /// <see cref="GetAsyncEnumerator"/> and the one passed to all subscribers to the message.
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        /// <summary>
        /// Gets the <see cref="System.Threading.CancellationToken"/> instance that will be linked to the
        /// one used to asynchronously enumerate the received responses. This can be used to cancel asynchronous
        /// replies that are still being processed, if no new items are needed from this request message.
        /// Consider the following example, where we define a message to retrieve the currently opened documents:
        /// <code>
        /// public class OpenDocumentsRequestMessage : AsyncCollectionRequestMessage&lt;XmlDocument&gt; { }
        /// </code>
        /// We can then request and enumerate the results like so:
        /// <code>
        /// await foreach (var document in Messenger.Default.Send&lt;OpenDocumentsRequestMessage&gt;())
        /// {
        ///     // Process each document here...
        /// }
        /// </code>
        /// </summary>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        public void Reply(T response)
        {
            Reply(UniTask.FromResult(response));
        }

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="response"/> is <see langword="null"/>.</exception>
        public void Reply(UniTask<T> response)
        {
            ThrowIfNull(response);

            _responses.Add(new() { task = response, isTask = true });
        }

        private static void ThrowIfNull<TY>(TY obj)
        {
            if (Equals(obj, default(T)))
            {
                throw new NullReferenceException();
            }
        }

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="response"/> is <see langword="null"/>.</exception>
        public void Reply(Func<CancellationToken, UniTask<T>> response)
        {
            ThrowIfNull(response);

            _responses.Add(new() { func = response, isTask = false });
        }

        /// <summary>
        /// Gets the collection of received response items.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> value to stop the operation.</param>
        /// <returns>The collection of received response items.</returns>
        public async UniTask<IReadOnlyCollection<T>> GetResponsesAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.CanBeCanceled)
            {
                _ = cancellationToken.Register(_cancellationTokenSource.Cancel);
            }

            var results = new T[_responses.Count];

            var index = 0;
            await foreach (var response in this.WithCancellation(cancellationToken))
            {
                results[index] = response;
                index++;
            }

            return results;
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IUniTaskAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return UniTaskAsyncEnumerable.Create<T>(async (writer, token) =>
            {
                if (token.CanBeCanceled)
                {
                    _ = cancellationToken.Register(_cancellationTokenSource.Cancel);
                }

                foreach (var response in _responses)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (response.isTask)
                    {
                        await writer.YieldAsync(await response.task);
                        await UniTask.Yield();
                    }
                    else
                    {
                        await writer.YieldAsync(await response.func(token));
                        await UniTask.Yield();
                    }
                }
            }).GetAsyncEnumerator(cancellationToken);
        }
    }
}