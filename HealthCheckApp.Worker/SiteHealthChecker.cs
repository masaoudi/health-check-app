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
            _logger.LogInformation($"** D�BUT DE LA V�RIFICATION ** - Heure d�clench�e : {DateTime.Now}");
            // Ici, on ajoutera la logique pour v�rifier les sites...
            _logger.LogInformation($"** FIN DE LA V�RIFICATION **");
        }
    }
}
