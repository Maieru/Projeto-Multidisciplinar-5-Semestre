using Web_Application.Models;

namespace Web_Application.DAO
{
    public class SubscriberAlertaDAO : GenericDAO<SubscriberAlertaViewModel>
    {
        protected override void SetTabela() => Tabela = "SubscriberAlerta";
    }
}
