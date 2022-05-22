using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Reflection;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;
using Negocio.Services;
using Web_Application.Models;

namespace Web_Application.Controllers
{
    public class UsuarioController : GenericController<UsuarioViewModel>
    {

        protected override void SetDAO() => DAO = new UsuarioDAO();
        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = false;
            MinumumLevelRequired = EnumTipoUsuario.Padrao;
        }
        protected override void SetIdGenerationConfig() => GeraProximoId = false;

        public override IActionResult Index()
        {
            if (UsuarioLogado == null)
                return RedirectToAction("Index", "Login");
            else if (UsuarioLogado.TipoUsuario >= EnumTipoUsuario.Administrador)
                return base.Index();
            else
                return View("Unauthorized");
        }

        public override IActionResult Save(UsuarioViewModel model, string operacao)
        {
            try
            {
                PreencheCamposEncriptados(model);

                if (model.Imagem == null || model.ImagemFile != null)
                    model.Imagem = ImagemService.ConvertImageToByte(model.ImagemFile);

                return base.Save(model, operacao);
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

        public override IActionResult Edit(int id)
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

        private void PreencheCamposEncriptados(UsuarioViewModel model)
        {
            if (model.UsuarioDesencriptografado != null)
                model.UsuarioLogin = EncriptionService.EncriptaString(model.UsuarioDesencriptografado);
            
            if (model.SenhaDesencriptografada != null)
                model.Senha = EncriptionService.EncriptaString(model.SenhaDesencriptografada);
        }

        protected override void ValidaDados(UsuarioViewModel model, string operacao)
        { 
            if (operacao == "I" &&
                model.UsuarioLogin != null &&
                (DAO as UsuarioDAO).VerificaSeUsuarioJaEstaCadastrado(model.UsuarioLogin))
                ModelState.AddModelError("UsuarioDesencriptografado", "Esse usuário já existe");

            // Limita o tamanho da imagem em 500 kB
            if (model.ImagemFile != null && model.ImagemFile.Length / 1024 > 500)
                ModelState.AddModelError("ImagemFile", "A imagem não deve possuir mais do que 500 kB de tamanho");
        
            if (operacao == "A")
            {
                // Nesse caso, somente os valores encriptografados serão salvos
                ModelState.Remove("UsuarioDesencriptografado");
                ModelState.Remove("SenhaDesencriptografada");

                if (model.UsuarioLogin == null)
                    ModelState.AddModelError("Usuario", "O login de usuário foi alterado");

                if (model.Senha == null)
                    ModelState.AddModelError("Senha", "A senha do usuário foi alterada");
            }
        }

        protected override void PreencheDadosParaView(string Operacao, UsuarioViewModel model)
        {
            if (model.Imagem != null)
            {
                var stream = new MemoryStream(model.Imagem);
                model.ImagemFile = new FormFile(stream, 0, stream.Length, "imagemUsuariu", "imagemUsuariu");
            }

            base.PreencheDadosParaView(Operacao, model);
        }
    }
}
