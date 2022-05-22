using Negocio.Models;

namespace Negocio.DAO
{
    public class SubscriberAlertaDAO : GenericDAO<SubscriberAlertaViewModel>
    {
        protected override void SetTabela() => Tabela = "SubscriberAlerta";
    }
}
