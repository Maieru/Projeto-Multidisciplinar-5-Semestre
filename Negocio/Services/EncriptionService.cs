using System;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Services
{
    public static class EncriptionService
    {
        /// <summary>
        /// Encripta uma determinada string utilizando SHA512.
        /// </summary>
        /// <param name="dado">String a ser encriptada</param>
        /// <returns>Retorna uma string correpondente ao dado encriptado. Retornará
        /// vazio caso o dado for vazio</returns>
        public static string EncriptaString(string dado)
        {
            if (dado == null)
                return null;

            using (var encriptador = new SHA512Managed())
            {
                var dadosBytes = Encoding.UTF8.GetBytes(dado);
                var dadoEncriptado = encriptador.ComputeHash(dadosBytes);
                return Convert.ToBase64String(dadoEncriptado);
            }
        }
    }
}
