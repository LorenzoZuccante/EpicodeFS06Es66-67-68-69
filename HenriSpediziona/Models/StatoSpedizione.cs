using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HenriSpediziona.Models
{
    public class StatoSpedizione
    {
        public int IdStato { get; set; }
        public int IdSpedizione { get; set; }
        public string StatoConsegna { get; set; }
        public string PosizionePacco { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataOraAggiornamento { get; set; }

        public virtual Spedizione Spedizione { get; set; }
    }
}