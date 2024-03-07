using System.Collections.Generic;

namespace HenriSpediziona.Models
{
    public class TrackingViewModel
    {
        public bool IsAzienda { get; set; }
        public string CodiceFiscalePartitaIVA { get; set; }
        public int IdSpedizione { get; set; }
        public Utente Utente { get; set; }
        public List<StatoSpedizione> StatiSpedizione { get; set; } = new List<StatoSpedizione>();

        // Costruttore di default
        public TrackingViewModel()
        {
            StatiSpedizione = new List<StatoSpedizione>();
        }
    }
}
