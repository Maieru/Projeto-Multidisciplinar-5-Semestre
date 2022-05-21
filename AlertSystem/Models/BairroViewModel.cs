using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AlertSystem.Attributes;

namespace AlertSystem.Models
{
    public class BairroViewModel : BaseDatabaseModel
    {
        [DatabaseProperty]
        public string Descricao { get; set; }

        [DatabaseProperty]
        public double Latitude { get; set; }

        [DatabaseProperty]
        public double Longitude { get; set; }

        [DatabaseProperty]
        public string CEP { get; set; }
    }
}
