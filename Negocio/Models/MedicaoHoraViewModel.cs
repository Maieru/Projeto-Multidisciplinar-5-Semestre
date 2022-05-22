namespace Negocio.Models
{
    /// <summary>
    /// Esta classe somente deve ser utilizada no dashboard
    /// </summary>
    public class MedicaoHoraViewModel
    {
        public int Hora { get; set; }
        public double? ValorChuva { get; set; }
        public double? ValorNivel { get; set; }
    }
}
