using System;
using System.ComponentModel.DataAnnotations;
using Negocio.Attributes;

namespace Negocio.Models
{
    public class DispositivoViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        [Required(ErrorMessage = "É necessário preencher a descrição do dispositivo")]
        [MaxLength(100, ErrorMessage = "A descrição deve ter no máximo 100 caracteres")]
        public string Descricao { get; set; }

        [DatabaseProperty]
        [Required(ErrorMessage = "É necessário preencher o bairro")]
        public int BairroId { get; set; }

        [DatabaseProperty]
        public DateTime DataAtualizacao { get; set; }

        [DatabaseProperty]
        [Required(ErrorMessage = "É necessário preencher a medição de referência")]
        [Range(0, 3000, ErrorMessage = "Os valor de medição de referência deve estar entre 0 e 3000 mm")]
        public double MedicaoReferencia { get; set; }


        #region Propriedades Utilizadas Apenas Em View

        public string NomeBairro { get; set; }

        #endregion
    }
}
