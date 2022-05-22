using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;

namespace Web_Application.Controllers
{
    public class LogWebController : GenericController<LogWebViewModel>
    {
        protected override void SetDAO() => DAO = new LogWebDAO();
        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = true;
            MinumumLevelRequired = EnumTipoUsuario.Tecnico;
        }
        protected override void SetIdGenerationConfig() => GeraProximoId = true;

        [HttpGet("api/SearchLog")]
        public IActionResult SearchMedicao(string searchType, string searchDataInicial, string searchDataFinal)
        {
            try
            {
                if (searchType == "-1")
                    searchType = null;

                var resultadoBusca = (DAO as LogWebDAO).Search(searchType, searchDataInicial, searchDataFinal);

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
