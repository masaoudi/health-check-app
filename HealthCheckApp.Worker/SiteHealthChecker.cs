using HealthCheckApp.Web.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace HealthCheckApp.Worker
{
    public class SiteHealthChecker
    {
        // On injecte le DbContext et un logger via le constructeur
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SiteHealthChecker> _logger;
        private readonly HttpClient _httpClient;

        public SiteHealthChecker(ApplicationDbContext dbContext, ILogger<SiteHealthChecker> logger, HttpClient httpClient)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpClient = httpClient;
        }

        [Function("SiteHealthChecker")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer, ILogger log)
        {
            _logger.LogInformation($"** D�BUT DE LA V�RIFICATION ** - Heure d�clench�e : {DateTime.Now}");

            try
            {
                // 1. R�cup�rer tous les sites � surveiller depuis la base de donn�es
                var sitesToCheck = await _dbContext.MonitoredSites.ToListAsync();
                _logger.LogInformation($"Nombre de sites � v�rifier : {sitesToCheck.Count}");

                // 2. V�rifier chaque site de mani�re asynchrone
                var checkTasks = sitesToCheck.Select(CheckSiteStatus);
                await Task.WhenAll(checkTasks);

                // 3. Sauvegarder tous les changements dans la base de donn�es en une seule fois
                var changesSaved = await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"V�rification termin�e. {changesSaved} changement(s) sauvegard�(s).");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la v�rification des sites.");
            }

            _logger.LogInformation($"** FIN DE LA V�RIFICATION **");
        }

        private async Task CheckSiteStatus(MonitoredSite site)
        {
            var oldStatus = site.IsUp;
            site.LastChecked = DateTime.Now;

            try
            {
                // Configure la requ�te pour ne pas suivre les redirections et avoir un timeout
                var request = new HttpRequestMessage(HttpMethod.Head, site.Url);
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Timeout de 10s

                var response = await _httpClient.SendAsync(request, cts.Token);

                // Consid�re le site comme "Up" si le code de statut HTTP est r�ussi (2xx)
                site.IsUp = response.IsSuccessStatusCode;

                _logger.LogInformation($"Site '{site.Name}' ({site.Url}) : {(site.IsUp ? "EN LIGNE" : "HORS LIGNE")} - Code: {(int)response.StatusCode}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode != null)
            {
                // Capture les erreurs HTTP avec un code de statut (ex: 404, 500)
                site.IsUp = false;
                _logger.LogWarning($"Site '{site.Name}' ({site.Url}) : HORS LIGNE - Erreur HTTP: {ex.StatusCode}");
            }
            catch (Exception ex) when (ex is TaskCanceledException or TimeoutException)
            {
                // Capture les timeouts
                site.IsUp = false;
                _logger.LogWarning($"Site '{site.Name}' ({site.Url}) : HORS LIGNE - Timeout d�pass�.");
            }
            catch (Exception ex)
            {
                // Capture toute autre exception (URL mal form�e, etc.)
                site.IsUp = false;
                _logger.LogError(ex, $"Site '{site.Name}' ({site.Url}) : Erreur lors de la v�rification.");
            }

            // Si le statut a chang�, on enregistre le moment de la panne/remise en ligne
            if (oldStatus != site.IsUp)
            {
                site.LastDownTime = site.IsUp ? null : DateTime.Now; // Si �a vient de tomber, on note l'heure
                _logger.LogWarning($"Changement de statut pour '{site.Name}' : {oldStatus} -> {site.IsUp}");
            }
        }

        // Classe pour le trigger Timer (g�n�r�e automatiquement)
        public class MyInfo
        {
            public MyScheduleStatus ScheduleStatus { get; set; } = null!;
            public bool IsPastDue { get; set; }
        }

        public class MyScheduleStatus
        {
            public DateTime Last { get; set; }
            public DateTime Next { get; set; }
            public DateTime LastUpdated { get; set; }
        }
    }
}
