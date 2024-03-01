using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Polizia.Models
{
    public class ViolazioneSuperioreA10Punti
    {
        public decimal Importo { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public DateTime DataViolazione { get; set; }
        public int DecurtamentoPunti { get; set; }
    }
}