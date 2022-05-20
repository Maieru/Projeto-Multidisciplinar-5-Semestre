using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Web_Application.DAO;
using Web_Application.Enum;
using Web_Application.Models;
using Web_Application.Services;

namespace Web_Application.Controllers
{
    public class BairroController : GenericController<BairroViewModel>
    {

        protected override void SetAutenticationRequirements()
        {
            AutenticationRequired = true;
            MinumumLevelRequired = EnumTipoUsuario.Tecnico;
        }
        protected override void SetIdGenerationConfig() => GeraProximoId = true;
        protected override void SetDAO() => DAO = new BairroDAO();

        [HttpGet("api/SearchBairro")]
        public IActionResult SearchBairro(string searchId, string searchDescricao, string searchCEP)
        {
            try
            {
                var resultadoBusca = (DAO as BairroDAO).Search(searchId, searchDescricao, searchCEP);

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

        [HttpGet("api/GetLatLong")]
        public async Task<IActionResult> GetLatLong(string CEP)
        {
            try
            {
                // Garante que o CEP será numérico
                if (CEP == null || !int.TryParse(CEP, out _) || CEP.Length < 5)
                    return null;

                CEP = CEP.Substring(0, 5);

                using (var httpClient = new HttpClient())
                {
                    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={CEP}&key={APIKeys.GOOGLE_API_KEY}&components=country:BR";

                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResposta = await response.Content.ReadAsStringAsync();

                        JObject resposta = JObject.Parse(stringResposta);

                        if (resposta["results"].Count() == 0)
                            return null;

                        return Json(new
                        {
                            Latitude = (string)resposta["results"][0]["geometry"]["location"]["lat"].ToString().Replace('.', ','),
                            Longitude = (string)resposta["results"][0]["geometry"]["location"]["lng"].ToString().Replace('.', ',')
                        });
                    }
                    else
                        return null;
                }
            }
            catch (Exception erro)
            {
                LogService.GeraLogErro(erro,
                                       controller: GetType().Name,
                                       action: MethodInfo.GetCurrentMethod()?.Name);

                return null;
            }
        }
    }
}
