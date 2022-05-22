using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using Negocio.Models;
using Negocio.Services;
using Negocio.DAO;
using Web_Application.Services;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Submit(UsuarioViewModel model)
        {
            try
            {
                model.UsuarioLogin = EncriptionService.EncriptaString(model.UsuarioDesencriptografado);
                model.Senha = EncriptionService.EncriptaString(model.SenhaDesencriptografada);

                ValidaDados(ref model);
                if (ModelState.IsValid)
                {
                    SessionService.SalvaCache<UsuarioViewModel>(HttpContext,
                                                                ConstantesComuns.USUARIO_SESSAO,
                                                                model);

                    return RedirectToAction("Options", "Home");
                }
                else
                {
                    return View("index", model);
                }
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro<UsuarioViewModel>(erro,
                                                         model: model,
                                                         controller: GetType().Name,
                                                         action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        public IActionResult Logoff()
        {
            try
            {
                SessionService.SalvaCache<UsuarioViewModel>(HttpContext,
                                                            ConstantesComuns.USUARIO_SESSAO,
                                                            null);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro<UsuarioViewModel>(erro,
                                                         controller: GetType().Name,
                                                         action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        private void ValidaDados(ref UsuarioViewModel model)
        {
            ModelState.Clear();
            var usuarioDAO = new UsuarioDAO();

            if (model.UsuarioDesencriptografado == null)
            {
                ModelState.AddModelError("UsuarioDesencriptografado", "Usuário deve ser preenchido");
            }
            else if (!usuarioDAO.VerificaSeUsuarioJaEstaCadastrado(model.UsuarioLogin))
            {
                ModelState.AddModelError("UsuarioDesencriptografado", "Usuário não existe");
            }

            if (model.SenhaDesencriptografada == null)
            {
                ModelState.AddModelError("SenhaDesencriptografada", "Senha deve ser preenchida");
            }
            else if (!usuarioDAO.VerificaSeUsuarioESenhaCorrespondem(ref model))
            {
                ModelState.AddModelError("SenhaDesencriptografada", "Senha não corresponde ao usuário");
            }
        }
    }
}
