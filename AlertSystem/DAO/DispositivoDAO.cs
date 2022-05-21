using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AlertSystem.Models;

namespace AlertSystem.DAO
{
    public class DispositivoDAO : GenericDAO<DispositivoViewModel>
    {
        protected override void SetTabela() => Tabela = "Dispositivos";

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
