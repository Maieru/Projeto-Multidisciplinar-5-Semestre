using System;
using System.ComponentModel.DataAnnotations;
using Negocio.Attributes;
using Negocio.Enum;

namespace Negocio.Models
{
    public class LogWebViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        public DateTime DataGeracao { get; set; }

        [DatabaseProperty]
        public string Mensagem { get; set; }

        [DatabaseProperty]
        public string Callstack { get; set; }

        [DatabaseProperty]
        public EnumTipoLog TipoOperacao { get; set; }

        [DatabaseProperty]
        public string Detalhes { get; set; }
    }
}
