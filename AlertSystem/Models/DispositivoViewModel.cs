using System;
using System.ComponentModel.DataAnnotations;
using AlertSystem.Attributes;

namespace AlertSystem.Models
{
    public class DispositivoViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        public string Descricao { get; set; }

        [DatabaseProperty]
        public int BairroId { get; set; }

        [DatabaseProperty]
        public DateTime DataAtualizacao { get; set; }

        [DatabaseProperty]
        public double MedicaoReferencia { get; set; }
    }
}
