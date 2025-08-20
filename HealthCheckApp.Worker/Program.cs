using HealthCheckApp.Web.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // <-- Utilisez ceci pour le modèle isolé
    .ConfigureServices(services =>
    {
        // Configurez vos services ici
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Obtenez la chaîne de connexion depuis les paramètres de la Function
        string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString")!;

        // Ajoutez le DbContext. Utilisez SQL Server et passez la chaîne de connexion.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Ajoutez le service HttpClient (important pour faire les requêtes HTTP de check plus tard)
        services.AddHttpClient();
    })
    .Build();

host.Run();