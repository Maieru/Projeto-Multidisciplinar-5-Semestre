using AlertSystem.Models;

namespace AlertSystem.DAO
{
    public class SubscriberAlertaDAO : GenericDAO<SubscriberAlertaViewModel>
    {
        protected override void SetTabela() => Tabela = "SubscriberAlerta";
    }
}
