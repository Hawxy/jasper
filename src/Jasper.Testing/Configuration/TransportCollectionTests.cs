using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jasper.Configuration;
using Jasper.Messaging;
using Jasper.Messaging.Runtime.Routing;
using Jasper.Messaging.Transports;
using Jasper.Messaging.Transports.Local;
using Jasper.Messaging.Transports.Sending;
using Jasper.Messaging.Transports.Stub;
using Jasper.Messaging.Transports.Tcp;
using Jasper.Util;
using NSubstitute;
using Shouldly;
using TestingSupport;
using Xunit;

namespace Jasper.Testing.Configuration
{
    public class TransportCollectionTests
    {
        [Fact]
        public void add_transport()
        {
            var transport = Substitute.For<ITransport>();
            transport.Protocol.Returns("fake");

            var collection = new TransportCollection {transport};

            collection.ShouldContain(transport);


        }

        [Fact]
        public void try_to_get_endpoint_from_invalid_transport()
        {
            var collection = new TransportCollection();
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                collection.TryGetEndpoint("wrong://server".ToUri());
            });
        }

        [Fact]
        public void stub_is_registered_by_default()
        {
            new TransportCollection()
                .OfType<StubTransport>()
                .Count().ShouldBe(1);
        }

        [Fact]
        public void tcp_is_registered_by_default()
        {
            new TransportCollection()
                .OfType<TcpTransport>()
                .Count().ShouldBe(1);
        }

        [Fact]
        public void local_is_registered_by_default()
        {
            new TransportCollection()
                .OfType<LocalTransport>()
                .Count().ShouldBe(1);
        }

        [Fact]
        public void retrieve_transport_by_scheme()
        {
            new TransportCollection()
                .TransportForScheme("tcp")
                .ShouldBeOfType<TcpTransport>();
        }

        [Fact]
        public void retrieve_transport_by_type()
        {
            new TransportCollection()
                .Get<LocalTransport>()
                .ShouldNotBeNull();
        }

        [Fact]
        public void all_endpoints()
        {
            var collection = new TransportCollection();
            collection.ListenAtPort(2222);
            collection.PublishAllMessages().ToStub("one");

            // 3 default local queues + the 2 added here
            collection.AllEndpoints()
                .Length.ShouldBe(5);
        }

        [Fact]
        public void publish_mechanism_with_multiple_subscribers()
        {
            var collection = new TransportCollection();
            collection.Publish(x =>
            {
                x.MessagesFromNamespace("One");
                x.MessagesFromNamespace("Two");

                x.ToPort(3333);
                x.ToPort(4444);
            });

            var endpoint3333 = collection.TryGetEndpoint("tcp://localhost:3333".ToUri());
            var endpoint4444 = collection.TryGetEndpoint("tcp://localhost:4444".ToUri());

            endpoint3333.Subscriptions[0]
                .ShouldBe(new Subscription{Scope = RoutingScope.Namespace, Match = "One"});

            endpoint3333.Subscriptions[1]
                .ShouldBe(new Subscription{Scope = RoutingScope.Namespace, Match = "Two"});

            endpoint4444.Subscriptions[0]
                .ShouldBe(new Subscription{Scope = RoutingScope.Namespace, Match = "One"});

            endpoint4444.Subscriptions[1]
                .ShouldBe(new Subscription{Scope = RoutingScope.Namespace, Match = "Two"});

        }

        [Fact]
        public void create_transport_type_if_missing()
        {
            var collection = new TransportCollection();
            var transport = collection.Get<FakeTransport>();

            collection.Get<FakeTransport>()
                .ShouldBeSameAs(transport);
        }

        public class FakeTransport : ITransport
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public string Protocol { get; } = "fake";
            public Uri ReplyUri { get; }
            public ISendingAgent BuildSendingAgent(Uri uri, IMessagingRoot root, CancellationToken cancellation)
            {
                throw new NotImplementedException();
            }

            public void Initialize(IMessagingRoot root)
            {
                throw new NotImplementedException();
            }

            public Endpoint ListenTo(Uri uri)
            {
                throw new NotImplementedException();
            }

            public void StartSenders(IMessagingRoot root, ITransportRuntime runtime)
            {
                throw new NotImplementedException();
            }

            public void StartListeners(IMessagingRoot root, ITransportRuntime runtime)
            {
                throw new NotImplementedException();
            }

            public void Initialize(IMessagingRoot root, ITransportRuntime runtime)
            {
                throw new NotImplementedException();
            }

            public ISender CreateSender(Uri uri, CancellationToken cancellation, IMessagingRoot root)
            {
                throw new NotImplementedException();
            }

            public Endpoint GetOrCreateEndpoint(Uri uri)
            {
                throw new NotImplementedException();
            }

            public Endpoint TryGetEndpoint(Uri uri)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Endpoint> Endpoints()
            {
                throw new NotImplementedException();
            }
        }
    }
}
