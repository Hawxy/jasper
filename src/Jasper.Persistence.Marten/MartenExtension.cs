using Jasper;
using Jasper.Configuration;
using Jasper.Messaging.Durability;
using Jasper.Persistence.Marten;
using Jasper.Persistence.Marten.Codegen;
using Jasper.Persistence.Marten.Persistence.Sagas;
using Jasper.Persistence.Postgresql;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace Jasper.Persistence.Marten
{
    public class MartenExtension : IJasperExtension
    {
        public void Configure(JasperOptions options)
        {
            options.Services.AddTransient<IEnvelopePersistence, PostgresqlEnvelopePersistence>();
            options.Services.AddSingleton(Options);

            options.CodeGeneration.Sources.Add(new MartenBackedPersistenceMarker());

            var frameProvider = new MartenSagaPersistenceFrameProvider();
            options.CodeGeneration.SetSagaPersistence(frameProvider);
            options.CodeGeneration.SetTransactions(frameProvider);

            options.Services.AddSingleton<IDocumentStore>(x =>
            {
                var documentStore = new DocumentStore(Options);
                return documentStore;
            });

            options.Handlers.GlobalPolicy<FineGrainedSessionCreationPolicy>();


            options.Services.AddScoped(c => c.GetService<IDocumentStore>().OpenSession());
            options.Services.AddScoped(c => c.GetService<IDocumentStore>().QuerySession());

            options.CodeGeneration.Sources.Add(new SessionVariableSource());

            options.Services.AddSingleton(s =>
            {
                return new PostgresqlSettings
                {
                    // Super hacky, look away!!!
                    ConnectionString = Options.Tenancy?.Default.CreateConnection().ConnectionString,
                    SchemaName = Options.DatabaseSchemaName
                };
            });

        }

        public StoreOptions Options { get; } = new StoreOptions();
    }
}
// ENDSAMPLE
