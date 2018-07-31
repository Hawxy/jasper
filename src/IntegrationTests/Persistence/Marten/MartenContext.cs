﻿using Servers;
using Xunit;

namespace IntegrationTests.Persistence.Marten
{
    [Collection("marten")]
    public abstract class MartenContext : IClassFixture<DockerFixture<MartenContainer>>
    {
        public MartenContext(DockerFixture<MartenContainer> fixture)
        {
        }
    }
}
