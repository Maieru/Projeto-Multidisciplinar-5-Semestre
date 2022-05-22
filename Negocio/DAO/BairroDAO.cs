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

        public List<BairroMedicaoViewModel> GetLatestMeasures()
        {
            var procedureName = "spGetLatestMesureFromBairros";
            var tabela = HelperDAO.ExecutaProcSelect(procedureName, null);

            var lista = new List<BairroMedicaoViewModel>();

            foreach (DataRow registro in tabela.Rows)
            {
                var bairroMedicao = new BairroMedicaoViewModel();


                bairroMedicao.Id = Convert.ToInt32(registro["Id"]);
                bairroMedicao.CEP = registro["CEP"].ToString();
                bairroMedicao.Descricao = registro["Descricao"].ToString();
                bairroMedicao.Latitude = Convert.ToDouble(registro["Latitude"]);
                bairroMedicao.Longitude = Convert.ToDouble(registro["Longitude"]);

                if (registro["ValorChuva"] == DBNull.Value)
                    bairroMedicao.ValorChuva = 0;
                else
                    bairroMedicao.ValorChuva = Convert.ToDouble(registro["ValorChuva"]);

                if (registro["ValorNivel"] == DBNull.Value)
                    bairroMedicao.ValorNivel = 0;
                else
                    bairroMedicao.ValorNivel = Convert.ToDouble(registro["ValorNivel"]);

                lista.Add(bairroMedicao);
            }

            return lista;
        }
    }
}
