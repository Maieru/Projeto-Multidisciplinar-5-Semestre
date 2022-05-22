using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;
using Web_Application.Services;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class DashboardController : Controller
    {
        protected bool AutenticationRequired { get; set; } = true;
        protected EnumTipoUsuario MinumumLevelRequired { get; set; } = EnumTipoUsuario.Padrao;

        public IActionResult Index()
        {
            try
            {
                var dispositivoDAO = new DispositivoDAO();
                var bairroDAO = new BairroDAO();

                ViewBag.Dispositivos = dispositivoDAO.List().Select(d => new SelectListItem
                {
                    Text = d.Descricao,
                    Value = d.Id.ToString(),
                }).ToList();

                ViewBag.Bairros = bairroDAO.List().Select(b => new SelectListItem
                {
                    Text = b.Descricao,
                    Value = b.Id.ToString(),
                }).ToList();

                return View("Index");
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var usuarioEmCache = SessionService.RecuperaCache<UsuarioViewModel>(context.HttpContext, ConstantesComuns.USUARIO_SESSAO);

                if (usuarioEmCache != null)
                    ViewBag.Usuario = usuarioEmCache;

                if (AutenticationRequired)
                {
                    if (usuarioEmCache == null)
                        context.Result = RedirectToAction("Index", "Login");
                    else
                    {
                        if (usuarioEmCache.TipoUsuario >= MinumumLevelRequired)
                            base.OnActionExecuting(context);
                        else
                            context.Result = View("Unauthorized");
                    }
                }
                else
                    base.OnActionExecuting(context);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                context.Result = View("Error", new ErrorViewModel(erro.ToString()));
            }
        }


        [HttpGet("api/GetWaterLevelFromLastDayMeasuresFromDispositivo")]
        public IActionResult GetWaterLevelFromLastDayMeasuresFromDispositivo(int dispositivoId)
        {
            try
            {
                var listaMedicoesUltimoDia = GetLastDayMesures(dispositivoId);

                return Json(listaMedicoesUltimoDia.Select(m => new
                            {
                                label = m.Hora.ToString(),
                                y = m.ValorNivel
                            }).ToList());
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }

        [HttpGet("api/GetWaterLevelFromLastMonthMeasuresFromDispositivo")]
        public IActionResult GetWaterLevelFromLastMonthMeasuresFromDispositivo(int dispositivoId)
        {
            try
            {
                var listaMedicoesUltimoDia = GetLastMonthMesures(dispositivoId);

                return Json(listaMedicoesUltimoDia.Select(m => new
                {
                    label = m.Hora.ToString(),
                    y = m.ValorNivel
                }).ToList());
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }

        [HttpGet("api/GetRainValueFromLastDayMeasuresFromDispositivo")]
        public IActionResult GetRainValueFromLastDayMeasuresFromDispositivo(int dispositivoId)
        {
            try
            {
                var listaMedicoesUltimoDia = GetLastDayMesures(dispositivoId);

                return Json(listaMedicoesUltimoDia.Select(m => new
                {
                    label = m.Hora.ToString(),
                    y = m.ValorChuva
                }).ToList());
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }

        [HttpGet("api/GetRainValueFromLastMonthMeasuresFromDispositivo")]
        public IActionResult GetRainValueFromLastMonthMeasuresFromDispositivo(int dispositivoId)
        {
            try
            {
                var listaMedicoesUltimoDia = GetLastMonthMesures(dispositivoId);

                return Json(listaMedicoesUltimoDia.Select(m => new
                {
                    label = m.Hora.ToString(),
                    y = m.ValorChuva
                }).ToList());
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }

        [HttpGet("api/GetMapData")]
        public IActionResult GetMapData()
        {
            try
            {
                var bairroDAO = new BairroDAO();
                var listaMedicoesPorBairro = bairroDAO.GetLatestMeasures();

                var mapData = listaMedicoesPorBairro.Select(m => new
                {
                    descricao = m.Descricao,
                    center = new
                    {
                        lat = m.Latitude,
                        lng = m.Longitude
                    },
                    valorNivel = m.ValorNivel
                }).ToList();

                return Json(mapData);
            }
            catch (Exception erro)
            {

                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }
        private List<MedicaoHoraViewModel> GetLastDayMesures(int idDispositivo)
        {
            var medicaoDAO = new MedicaoDAO();
            var listaMedicoesUltimoDia = medicaoDAO.GetMedicoesUltimoDiaPorDispositivo(idDispositivo);

            // Essa pilha vai ajudar a preencher as lacunas de hora e organiza-las,
            // de forma que 
            var aux = new Stack<MedicaoHoraViewModel>();

            // Código para preencher eventuais lacunas na hora
            for (int i = 0; i < 24; i++)
            {
                if (!listaMedicoesUltimoDia.Any(m => m.Hora == DateTime.Now.AddHours(i * -1).Hour))
                    aux.Push(new MedicaoHoraViewModel { Hora = DateTime.Now.AddHours(i * -1).Hour, ValorChuva = 0, ValorNivel = 0 });
                else
                    aux.Push(listaMedicoesUltimoDia.FirstOrDefault(m => m.Hora == DateTime.Now.AddHours(i * -1).Hour));
            }

            return aux.ToList();
        }
        private List<MedicaoHoraViewModel> GetLastMonthMesures(int idDispositivo)
        {
            var medicaoDAO = new MedicaoDAO();
            var listaMedicoesUltimoDia = medicaoDAO.GetMedicoesUltimoMesPorDispositivo(idDispositivo);

            // Essa pilha vai ajudar a preencher as lacunas de hora e organiza-las,
            // de forma que 
            var aux = new Stack<MedicaoHoraViewModel>();

            // Código para preencher eventuais lacunas na hora
            for (int i = 0; i < 30; i++)
            {
                if (!listaMedicoesUltimoDia.Any(m => m.Hora == DateTime.Now.AddDays(i * -1).Day))
                    aux.Push(new MedicaoHoraViewModel { Hora = DateTime.Now.AddDays(i * -1).Day, ValorChuva = 0, ValorNivel = 0 });
                else
                    aux.Push(listaMedicoesUltimoDia.FirstOrDefault(m => m.Hora == DateTime.Now.AddDays(i * -1).Day));
            }

            return aux.ToList();
        }
    }
}
