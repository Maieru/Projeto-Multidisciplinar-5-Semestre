using System.ComponentModel.DataAnnotations;
using AlertSystem.Attributes;
using AlertSystem.Enum;

namespace AlertSystem.Models
{
    public class SubscriberAlertaViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        public int BairroId { get; set; }

        [DatabaseProperty]
        [MaxLength(100, ErrorMessage = "O número de telefone deve ter no máximo 100 caracteres")]
        public string Telefone { get; set; }

        [DatabaseProperty]
        [RegularExpression(@"[\w]+@[\w]+.[\w]+", ErrorMessage = "Digite um email em um formato válido")]
        [MaxLength(200, ErrorMessage = "O email deve ter no máximo 200 caracteres")]
        public string Email { get; set; }

        [DatabaseProperty]
        public int UsuarioId { get; set; }

        [DatabaseProperty]
        public EnumTipoAlerta UltimaNotificacao { get; set; }
    }
}
