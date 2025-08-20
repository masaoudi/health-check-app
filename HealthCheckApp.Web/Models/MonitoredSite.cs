namespace HealthCheckApp.Web.Models
{
    public class MonitoredSite
    {
        public int Id { get; set; } // Clé primaire
        public string Url { get; set; } = string.Empty; // L'URL à surveiller
        public string Name { get; set; } = string.Empty; // Un nom convivial donné par l'user
        public int CheckIntervalSeconds { get; set; } = 600; // Par défaut: 10 min (600 sec)

        // Suivi de l'état
        public bool IsUp { get; set; } = true; // True si le site est up, False si down
        public DateTime LastChecked { get; set; }
        public DateTime? LastDownTime { get; set; }

        // Clé étrangère vers l'utilisateur (on fera simple pour commencer)
        public string? UserEmail { get; set; } // On utilisera l'email comme identifiant pour l'instant
    }
}
