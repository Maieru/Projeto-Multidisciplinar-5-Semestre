using System.Data;
using System.Data.SqlClient;
using Negocio.Attributes;

namespace Negocio.Models
{
    /// <summary>
    /// Classe utilizada por todos os Models que são armazenados em banco de dados. 
    /// </summary>
    public abstract class BaseDatabaseModel
    {
        [DatabaseProperty]
        public virtual int Id { get; set; }
    }
}
