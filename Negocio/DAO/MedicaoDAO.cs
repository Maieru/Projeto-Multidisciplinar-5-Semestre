using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Negocio.Models;

namespace Negocio.DAO
{
    public class MedicaoDAO : GenericDAO<MedicaoViewModel>
    {
        protected override void SetTabela() => Tabela = "Medicao";

        public List<MedicaoViewModel> Search(string dispositivoId, string dataInicial, string dataFinal)
        {
            var procedureName = ConstantesComuns.PROC_SEARCH + Tabela;

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("DispositivoId", dispositivoId ?? ""),
                new SqlParameter("DataInicial", dataInicial ?? ""),
                new SqlParameter("DataFinal", dataFinal ?? "")
            });

            var lista = new List<MedicaoViewModel>();

            foreach (DataRow registro in tabela.Rows)
                lista.Add(MontaModel(registro));

            return lista;
        }
    
        public List<MedicaoHoraViewModel> GetMedicoesUltimoDiaPorDispositivo(int dispositivoId)
        {
            var procedureName = "spSelecionaMedicaoUltimoDiaPorDispositivo";

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("dispositivoId", dispositivoId),
            });

            return RecuperaListaDeMedicoes(tabela);
        }
        public List<MedicaoHoraViewModel> GetMedicoesUltimoMesPorDispositivo(int dispositivoId)
        {
            var procedureName = "spSelecionaMedicaoUltimoMesPorDispositivo";

            var tabela = HelperDAO.ExecutaProcSelect(procedureName, new SqlParameter[]
            {
                new SqlParameter("dispositivoId", dispositivoId),
            });

            return RecuperaListaDeMedicoes(tabela);
        }
        
        private List<MedicaoHoraViewModel> RecuperaListaDeMedicoes(DataTable tabela)
        {
            var lista = new List<MedicaoHoraViewModel>();

            foreach (DataRow registro in tabela.Rows)
            {
                var medicaoHoraAtual = new MedicaoHoraViewModel();
                medicaoHoraAtual.Hora = Convert.ToInt32(registro["ParteDataMedicao"]);

                if (registro["MediaChuva"] != DBNull.Value)
                    medicaoHoraAtual.ValorChuva = Convert.ToDouble(registro["MediaChuva"]);
                else
                    medicaoHoraAtual.ValorChuva = null;

                if (registro["MediaNivel"] != DBNull.Value)
                    medicaoHoraAtual.ValorNivel = Convert.ToDouble(registro["MediaNivel"]);
                else
                    medicaoHoraAtual.ValorNivel = null;

                lista.Add(medicaoHoraAtual);
            }

            return lista;
        }
    }
}
