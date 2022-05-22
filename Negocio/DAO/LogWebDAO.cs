using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Negocio.Models;

namespace Negocio.DAO
{
    public class LogWebDAO : GenericDAO<LogWebViewModel>
    {
        protected override void SetTabela() => Tabela = "LogWeb";

        public List<LogWebViewModel> Search(string tipoLog, string dataInicial, string dataFinal)
        {
            var procedureName = ConstantesComuns.PROC_SEARCH + Tabela;

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("TipoLog", tipoLog ?? ""),
                new SqlParameter("DataInicial", dataInicial ?? ""),
                new SqlParameter("DataFinal", dataFinal ?? "")
            });

            var lista = new List<LogWebViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    }
}
