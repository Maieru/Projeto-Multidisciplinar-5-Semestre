using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Negocio.Models;

namespace Negocio.DAO
{
    public class DispositivoDAO : GenericDAO<DispositivoViewModel>
    {
        protected override void SetTabela() => Tabela = "Dispositivos";

        public virtual List<DispositivoViewModel> ListComBairro()
        {
            var procedureName = "spSelecionaDispositivosComBairro";
            var tabela = HelperDAO.ExecutaProcSelect(procedureName, null);
            List<DispositivoViewModel> lista = new List<DispositivoViewModel>();

            foreach (DataRow registro in tabela.Rows)
            {
                var dispositivoComNomeDoBairro = MontaModel(registro);

                dispositivoComNomeDoBairro.NomeBairro = registro["NomeBairro"].ToString();

                lista.Add(dispositivoComNomeDoBairro);
            }

            return lista;
        }

        public List<DispositivoViewModel> Search(string id, string descricao, string bairro)
        {
            var procedureName = ConstantesComuns.PROC_SEARCH + Tabela;

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("Id", id ?? ""),
                new SqlParameter("Descricao", descricao ?? ""),
                new SqlParameter("NomeBairro", bairro ?? "")
            });

            var lista = new List<DispositivoViewModel>();

            foreach (DataRow registro in tabela.Rows)
            {
                var dispositivo = MontaModel(registro);
                dispositivo.NomeBairro = registro["NomeBairro"].ToString();
                lista.Add(dispositivo);
            }

            return lista;
        }

        public List<MedicaoViewModel> GetLatestMedicoes()
        {
            try
            {
                var retorno = new List<MedicaoViewModel>();
                var tabela = HelperDAO.ExecutaProcSelect("spGetLatestMedicao", null);

                foreach (DataRow medicao in tabela.Rows)
                    retorno.Add(new MedicaoViewModel
                    {
                        Id = Convert.ToInt32(medicao["Id"]),
                        DataMedicao = Convert.ToDateTime(medicao["DataMedicao"]),
                        DispositivoId = Convert.ToInt32(medicao["DispositivoId"]),
                        ValorChuva = Convert.ToDouble(medicao["ValorChuva"]),
                        ValorNivel = Convert.ToDouble(medicao["ValorNivel"]),
                    });

                return retorno;

            }
            catch (Exception ex) { return null; }
        }
    }
}
