using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;

namespace Web_Application.Controllers
{
    public class DispositivoController : GenericController<DispositivoViewModel>
    {
        public override IActionResult Index()
        {
            try
            {
                var lista = (DAO as DispositivoDAO).ListComBairro();
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override IActionResult Save(DispositivoViewModel model, string operacao)
        {
            model.DataAtualizacao = DateTime.Now;

            return base.Save(model, operacao);
        }

        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = true;
            MinumumLevelRequired = EnumTipoUsuario.Tecnico;
        }

        protected override void SetDAO() => DAO = new DispositivoDAO();

        protected override void SetIdGenerationConfig() => GeraProximoId = true;

        protected override void PreencheDadosParaView(string Operacao, DispositivoViewModel model)
        {
            var daoBairro = new BairroDAO();
            ViewBag.ListaBairros = daoBairro.List();

            base.PreencheDadosParaView(Operacao, model);
        }

        protected override void ValidaDados(DispositivoViewModel model, string operacao)
        {
            var bairroDAO = new BairroDAO();

            if (bairroDAO.Consulta(model.BairroId) == null)
                ModelState.AddModelError("BairroId", "O bairro informado não existe");

            if (model.DataAtualizacao > DateTime.Now)
                ModelState.AddModelError("DataCriacao", "A data de criação deve ser menor que o horário atual");

            base.ValidaDados(model, operacao);
        }

        [HttpGet("api/SearchDispositivo")]
        public IActionResult SearchBairro(string searchId, string searchDescricao, string searchBairro)
        {
            try
            {
                var resultadoBusca = (DAO as DispositivoDAO).Search(searchId, searchDescricao, searchBairro);

                return PartialView("pvGrid", resultadoBusca);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
    }
}
