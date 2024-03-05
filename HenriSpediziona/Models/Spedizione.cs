using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HenriSpediziona.Models
{
    public class Spedizione
    {
        public int IdSpedizione { get; set; }
        public int IdUtente { get; set; }
        public DateTime DataSpedizione { get; set; }
        public int Peso { get; set; }
        public string CittaDestinataria { get; set; }
        public string IndirizzoDestinatario { get; set; }
        public string NomeDestinatario { get; set; }
        public string CognomeDestinatario { get; set; }
        public decimal CostoSpedizione { get; set; }
        public DateTime? DataDiConsegna { get; set; }

        public virtual Utente Utente { get; set; }
        public virtual ICollection<StatoSpedizione> StatiSpedizione { get; set; }
    }
}