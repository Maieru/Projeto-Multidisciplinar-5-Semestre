using Negocio.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Models
{
    public class BairroMedicaoViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        [Required(ErrorMessage = "O preenchimento da descrição é necessário")]
        [MaxLength(50, ErrorMessage = "A descrição pode conter no máximo 50 caracteres")]
        public string Descricao { get; set; }

        [DatabaseProperty]
        [Required(ErrorMessage = "O preenchimento da latitude é necessário")]
        [Range(-180, 180, ErrorMessage = "Valor inválido para latitude")]
        public double Latitude { get; set; }

        [DatabaseProperty]
        [Required(ErrorMessage = "O preenchimento da longitude é necessário")]
        [Range(-180, 180, ErrorMessage = "Valor inválido para longitude")]
        public double Longitude { get; set; }

        [DatabaseProperty]
        [Required(ErrorMessage = "O preenchimento do CEP é necessário")]
        [RegularExpression(@"^\b[0-9]{5}-[0-9]{3}", ErrorMessage = "CEP no formato inválido")]
        public string CEP { get; set; }

        [DatabaseProperty]
        public double? ValorChuva { get; set; }

        [DatabaseProperty]
        public double? ValorNivel { get; set; }
    }
}
