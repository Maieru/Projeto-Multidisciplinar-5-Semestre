using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using Negocio.Attributes;
using Negocio.Enum;
using Negocio.Models;

namespace Web_Application.Models
{
    public class UsuarioViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        public string UsuarioLogin { get; set; }

        [DatabaseProperty]
        public string Senha { get; set; }

        [DatabaseProperty]
        [MaxLength(100, ErrorMessage = "O tamanho máximo do campo de nome é 100 caracteres")]
        [Required(ErrorMessage = "O nome do usuário deve ser preenchido")]
        public string Nome { get; set; }

        [DatabaseProperty]
        public byte[] Imagem { get; set; }

        [DatabaseProperty]
        public EnumTipoUsuario TipoUsuario { get; set; }

        #region Propriedades Utilizadas Apenas Em View

        [Required(ErrorMessage = "O usuário deve ser preenchido")]
        [MinLength(6, ErrorMessage = "O usuário deve conter no minimo 6 caracteres")]
        public string UsuarioDesencriptografado { get; set; }

        [Required(ErrorMessage = "A senha deve ser preenchida")]
        [MinLength(6, ErrorMessage = "A senha deve conter no minimo 6 caracteres")]
        public string SenhaDesencriptografada { get; set; }

        // Imagem recebida do POST
        public IFormFile ImagemFile { get; set; }

        // Imagem utilizada no GET
        public string ImagemEmBase64
        {
            get
            {
                if (Imagem != null)
                    return Convert.ToBase64String(Imagem);
                else
                    return string.Empty;
            }

            set
            {
                if (value != null)
                    Imagem = Convert.FromBase64String(value);
                else
                    Imagem = null;
            }
        }


        #endregion
    }
}
