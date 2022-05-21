using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AlertSystem.Models;

namespace AlertSystem.DAO
{
    public class BairroDAO : GenericDAO<BairroViewModel>
    {
        protected override void SetTabela() => Tabela = "Bairros";
    }
}
