using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Polizia.Models
{
    public class Anagrafica
    {
        public int IdAnagrafica { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string CAP { get; set; }
        public string CF { get; set; }
    }
}