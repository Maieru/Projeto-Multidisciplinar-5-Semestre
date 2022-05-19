using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MongoReader.Models;

namespace MongoReader.DAO
{
    public class DispositivoDAO : GenericDAO<DispositivoViewModel>
    {
        protected override void SetTabela() => Tabela = "Dispositivos";
    }
}
