using System;
using System.Collections;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;
using MVVMToolkit.Messaging;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace MVVMToolkit.RuntimeTests
{
    public class MessagingTests
    {
        private static readonly Type[] MessengerTypes =
            { typeof(StrongReferenceMessenger), typeof(WeakReferenceMessenger) };

        [UnityTest]
        public IEnumerator UniTaskCollectionRequestMessageOkTest([ValueSource(nameof(MessengerTypes))] Type type) =>
            UniTask.ToCoroutine(async () =>
            {
                var messenger = (IMessenger)Activator.CreateInstance(type)!;
                object recipient1 = new();
                object recipient2 = new();
                object recipient3 = new();
                object recipient4 = new();


                async UniTask<int> GetNumberAsync()
                {
                    await UniTask.Delay(100);

                    return 4;
                }

                void Receive1(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(1);

                void Receive2(object recipient, AsyncNumbersCollectionRequestMessage m) =>
                    m.Reply(UniTask.FromResult(2));

                void Receive3(object recipient, AsyncNumbersCollectionRequestMessage m) => m.Reply(GetNumberAsync());

                void Receive4(object recipient, AsyncNumbersCollectionRequestMessage m) =>
                    m.Reply(_ => GetNumberAsync());

                messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient1, Receive1);
                messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient2, Receive2);
                messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient3, Receive3);
                messenger.Register<AsyncNumbersCollectionRequestMessage>(recipient4, Receive4);

                List<int> responses = new();

                await foreach (var response in messenger.Send<AsyncNumbersCollectionRequestMessage>())
                {
                    responses.Add(response);
                }

                CollectionAssert.AreEquivalent(responses, new[] { 1, 2, 4, 4 });

                GC.KeepAlive(recipient1);
                GC.KeepAlive(recipient2);
                GC.KeepAlive(recipient3);
                GC.KeepAlive(recipient4);
            });
    }

    public class AsyncNumbersCollectionRequestMessage : UniTaskCollectionRequestMessage<int>
    {
    }
}