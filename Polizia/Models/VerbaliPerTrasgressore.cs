using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Polizia.Models
{
    public class VerbaliPerTrasgressore
    {
        public string NomeTrasgressore { get; set; }
        public List<Verbale> Verbali { get; set; }
    }
}