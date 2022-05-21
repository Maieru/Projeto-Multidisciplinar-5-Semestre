using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AlertSystem.Models;

namespace AlertSystem.DAO
{
    public class MedicaoDAO : GenericDAO<MedicaoViewModel>
    {
        protected override void SetTabela() => Tabela = "Medicao";
    }
}
