using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;

namespace MVVMToolkit.Messaging
{
    public class UniTaskRequestMessage<T>
    {
        private UniTask<T> _response;

        public UniTask<T> Response
        {
            get
            {
                if (!HasReceivedResponse)
                {
                    ThrowInvalidOperationExceptionForNoResponseReceived();
                }

                return _response;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a response has already been assigned to this instance.
        /// </summary>
        public bool HasReceivedResponse { get; private set; }

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Response"/> has already been set.</exception>
        public void Reply(T response)
        {
            Reply(UniTask.FromResult(response));
        }

        /// <summary>
        /// Replies to the current request message.
        /// </summary>
        /// <param name="response">The response to use to reply to the request message.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="response"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Response"/> has already been set.</exception>
        public void Reply(UniTask<T> response)
        {
            if (response.Equals(default(UniTask<T>)))
            {
                throw new NullReferenceException();
            }

            if (HasReceivedResponse)
            {
                ThrowInvalidOperationExceptionForDuplicateReply();
            }

            HasReceivedResponse = true;

            _response = response;
        }

        /// <inheritdoc cref="UniTask{T}.GetAwaiter"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UniTask<T>.Awaiter GetAwaiter()
        {
            return Response.GetAwaiter();
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when a response is not available.
        /// </summary>
        [DoesNotReturn]
        private static void ThrowInvalidOperationExceptionForNoResponseReceived()
        {
            throw new InvalidOperationException("No response was received for the given request message.");
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when <see cref="Reply(T)"/> or <see cref="Reply(UniTask{T})"/> are called twice.
        /// </summary>
        [DoesNotReturn]
        private static void ThrowInvalidOperationExceptionForDuplicateReply()
        {
            throw new InvalidOperationException("A response has already been issued for the current message.");
        }
    }
}