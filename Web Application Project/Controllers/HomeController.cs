using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using Negocio.DAO;
using Negocio.Models;
using Negocio.Services;
using Web_Application.Services;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult About()
        {
            try
            {
                var usuarioEmCache = SessionService.RecuperaCache<UsuarioViewModel>(HttpContext, ConstantesComuns.USUARIO_SESSAO);

                if (usuarioEmCache == null)
                    return RedirectToAction("Index", "Login");

                if (usuarioEmCache != null)
                    ViewBag.Usuario = usuarioEmCache;

                return View();
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult Options()
        {
            try
            {
                var usuarioEmCache = SessionService.RecuperaCache<UsuarioViewModel>(HttpContext, ConstantesComuns.USUARIO_SESSAO);

                if (usuarioEmCache == null)
                    return RedirectToAction("Index", "Login");

                if (usuarioEmCache != null)
                    ViewBag.Usuario = usuarioEmCache;

                return View();
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}