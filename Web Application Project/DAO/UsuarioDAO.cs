using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Negocio.Models;
using Web_Application.Models;

namespace Negocio.DAO
{
    public class UsuarioDAO : GenericDAO<UsuarioViewModel>
    {
        protected override void SetTabela() => Tabela = "Usuario";

        public bool VerificaSeUsuarioJaEstaCadastrado(string usuarioEncriptografado)
        {
            var procedureName = "spVerificaUsuarioCadastrado";
            var retornoSp = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("@usuarioEncriptografado", usuarioEncriptografado)
            });

            return Convert.ToBoolean(retornoSp.Rows[0]["Retorno"]);
        }

        public bool VerificaSeUsuarioESenhaCorrespondem(ref UsuarioViewModel model)
        {
            var procedureName = "spVerificaUsuarioESenhaCorretos";
            var retornoSp = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("@usuarioEncriptografado", model.UsuarioLogin),
                new SqlParameter("@SenhaEncriptografada", model.Senha)
            });

            if (retornoSp == null || retornoSp.Rows.Count == 0)
                return false;
            else
            {
                model = MontaModel(retornoSp.Rows[0]);
                return true;
            }
        }

        protected override SqlParameter[] CriaParametros(UsuarioViewModel model)
        {
            var listaParametrosOriginais = base.CriaParametros(model).ToList();
            var parametroImagem = listaParametrosOriginais.Where(p => p.ParameterName == "Imagem").FirstOrDefault();

            // Isso se faz necessário graças a um erro onde não é possivel 
            // converter NVARCHAR(MAX) para VARBINARY(MAX)
            // TODO: Achar um jeito de consertar isso via banco, fazendo essa sobrescrita
            // inútil.
            // Detalhes do erro: https://stackoverflow.com/questions/18170985/null-value-in-a-parameter-varbinary-datatype
            if (parametroImagem.Value == DBNull.Value)
                parametroImagem.Value = SqlBinary.Null;

            // Necessário para converter o enum em um valor válido pro BD
            var parametroTipoUsuario = listaParametrosOriginais.Where(p => p.ParameterName == "TipoUsuario").FirstOrDefault();
            parametroTipoUsuario.Value = (int)model.TipoUsuario;

            return listaParametrosOriginais.ToArray();
        }
    }
}
