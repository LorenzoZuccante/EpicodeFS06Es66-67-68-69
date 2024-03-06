using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HenriSpediziona.Models
{
    public class Utente
    {
        public int IdUtente { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Azienda { get; set; }
        public string Nome { get; set; }
        public string Cod_Fisc { get; set; }
        public int? P_iva { get; set; }
        public bool IsAdmin { get; set; }
        public string Cognome { get; set; }

        public virtual ICollection<Spedizione> Spedizioni { get; set; }
    }
}