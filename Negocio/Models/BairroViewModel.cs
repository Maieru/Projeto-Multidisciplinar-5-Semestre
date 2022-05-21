using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Negocio.Attributes;

namespace Negocio.Models
{
    public class BairroViewModel : BaseDatabaseModel
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
    }
}
