using HealthCheckApp.Web.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication() // <-- Utilisez ceci pour le mod�le isol�
    .ConfigureServices(services =>
    {
        // Configurez vos services ici
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Obtenez la cha�ne de connexion depuis les param�tres de la Function
        string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString")!;

        // Ajoutez le DbContext. Utilisez SQL Server et passez la cha�ne de connexion.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Ajoutez le service HttpClient (important pour faire les requ�tes HTTP de check plus tard)
        services.AddHttpClient();
    })
    .Build();

host.Run();