using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;

namespace Web_Application.Controllers
{
    public class MedicaoController : GenericController<MedicaoViewModel>
    {
        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = true;
            MinumumLevelRequired = EnumTipoUsuario.Tecnico;
        }

        protected override void SetDAO() => DAO = new MedicaoDAO();

        protected override void SetIdGenerationConfig() => GeraProximoId = true;

        [HttpGet("api/SearchMedicao")]
        public IActionResult SearchMedicao(string searchDispositivoId, string searchDataInicial, string searchDataFinal)
        {
            try
            {
                var resultadoBusca = (DAO as MedicaoDAO).Search(searchDispositivoId, searchDataInicial, searchDataFinal);

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
