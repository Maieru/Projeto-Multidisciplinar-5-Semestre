using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Reflection;
using Web_Application.DAO;
using Web_Application.Enum;
using Web_Application.Models;
using Web_Application.Services;

namespace Web_Application.Controllers
{
    public abstract class GenericController<T> : Controller where T : BaseDatabaseModel
    {
        public GenericController()
        {
            SetDAO();
            SetAutenticationRequirements();
            SetIdGenerationConfig();
        }

        protected abstract void SetDAO();
        protected abstract void SetAutenticationRequirements();
        protected abstract void SetIdGenerationConfig();

        protected UsuarioViewModel UsuarioLogado { get; set; }
        protected GenericDAO<T> DAO { get; set; }
        protected bool GeraProximoId { get; set; }
        protected bool AutenticationRequired { get; set; } = true;
        protected EnumTipoUsuario MinumumLevelRequired { get; set; } = EnumTipoUsuario.Administrador;

        protected string NomeViewIndex { get; set; } = "Index";
        protected string NomeViewCreate { get; set; } = "Form";
        protected string NomeViewEdit { get; set; } = "Form";

        public virtual IActionResult Index()
        {
            try
            {
                var lista = DAO.List();
                return View(NomeViewIndex, lista);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro<T>(erro,
                                          controller: GetType().Name,
                                          action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public virtual IActionResult Create()
        {
            try
            {
                ViewBag.Operacao = "I";
                var model = Activator.CreateInstance(typeof(T)) as T;
                PreencheDadosParaView("I", model);
                return View(NomeViewCreate, model);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro<T>(erro,
                                          controller: GetType().Name,
                                          action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public virtual IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operacao = "A";

                var model = DAO.Consulta(id);
                if (model == null)
                    return RedirectToAction(NomeViewIndex);
                else
                {
                    PreencheDadosParaView("A", model);
                    return View(NomeViewEdit, model);
                }
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       id: id,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                DAO.Delete(id);
                return RedirectToAction(NomeViewIndex);
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       id: id,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }
        public virtual IActionResult Save(T model, string operacao)
        {
            try
            {
                ValidaDados(model, operacao);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operacao = operacao;
                    PreencheDadosParaView(operacao, model);

                    if (operacao == "I")
                        return View(NomeViewCreate, model);
                    else
                        return View(NomeViewEdit, model);
                }
                else
                {
                    if (operacao == "I")
                        DAO.Insert(model);
                    else
                        DAO.Update(model);

                    return RedirectToAction(NomeViewIndex);
                }
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       model: model,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return View("Error", new ErrorViewModel(erro.ToString()));
            }
        }

        protected virtual void PreencheDadosParaView(string Operacao, T model)
        {
            if (GeraProximoId && Operacao == "I")
                model.Id = DAO.ProximoId();
        }
        protected virtual void ValidaDados(T model, string operacao)
        {
            if (operacao == "I" && DAO.Consulta(model.Id) != null)
                ModelState.AddModelError("Id", "Código já está em uso!");
            if (operacao == "A" && DAO.Consulta(model.Id) == null)
                ModelState.AddModelError("Id", "Este registro não existe!");
            if (model.Id <= 0)
                ModelState.AddModelError("Id", "Id inválido!");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var usuarioEmCache = SessionService.RecuperaCache<UsuarioViewModel>(context.HttpContext, ConstantesComuns.USUARIO_SESSAO);

                if (usuarioEmCache != null)
                {
                    ViewBag.Usuario = usuarioEmCache;
                    UsuarioLogado = usuarioEmCache;
                }

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
    }
}
