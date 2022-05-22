using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Negocio.Models;

namespace Negocio.DAO
{
    public class BairroDAO : GenericDAO<BairroViewModel>
    {
        protected override void SetTabela() => Tabela = "Bairros";

        public List<BairroViewModel> Search(string id, string descricao, string CEP)
        {
            var procedureName = ConstantesComuns.PROC_SEARCH + Tabela;

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("Id", id ?? ""),
                new SqlParameter("Descricao", descricao ?? ""),
                new SqlParameter("CEP", CEP ?? "")
            });

            var lista = new List<BairroViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    }
}
