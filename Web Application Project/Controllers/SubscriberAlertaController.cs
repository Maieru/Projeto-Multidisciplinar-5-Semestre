using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;

namespace Web_Application.Controllers
{
    public class SubscriberAlertaController : GenericController<SubscriberAlertaViewModel>
    {
        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = true;
            MinumumLevelRequired = EnumTipoUsuario.Padrao;
        }

        protected override void SetDAO() => DAO = new SubscriberAlertaDAO();

        protected override void SetIdGenerationConfig() => GeraProximoId = true;

        protected override void PreencheDadosParaView(string Operacao, SubscriberAlertaViewModel model)
        {
            var daoBairro = new BairroDAO();
            ViewBag.ListaBairros = daoBairro.List();
            base.PreencheDadosParaView(Operacao, model);
        }

        public override IActionResult Index()
        {
            try
            {
                var alertaCadastrado = DAO.List().Where(a => a.UsuarioId == UsuarioLogado.Id).FirstOrDefault();

                if (alertaCadastrado != null)
                    return base.Edit(alertaCadastrado.Id);
                else
                    return base.Create();

            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Save(SubscriberAlertaViewModel model, string operacao)
        {
            if (UsuarioLogado != null)
                model.UsuarioId = UsuarioLogado.Id;
            else
                model.UsuarioId = 0;
            return base.Save(model, operacao);
        }
    }
}
