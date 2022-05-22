namespace Negocio.Models
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string erro)
        {
            Erro = erro;
        }

        public ErrorViewModel()
        {
        }

        public string Erro { get; private set; }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}