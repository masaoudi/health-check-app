using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace HealthCheckApp.Worker
{
    public class SiteHealthChecker
    {
        private readonly ILogger _logger;

        public SiteHealthChecker(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SiteHealthChecker>();
        }

        [Function("SiteHealthChecker")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"** DÉBUT DE LA VÉRIFICATION ** - Heure déclenchée : {DateTime.Now}");
            // Ici, on ajoutera la logique pour vérifier les sites...
            _logger.LogInformation($"** FIN DE LA VÉRIFICATION **");
        }
    }
}
