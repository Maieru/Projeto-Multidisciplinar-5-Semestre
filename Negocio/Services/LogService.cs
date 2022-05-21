using Newtonsoft.Json;
using System;
using Negocio.DAO;
using Negocio.Enum;
using Negocio.Models;

namespace Negocio.Services
{
    public static class LogService
    {
        public static void GeraLogErro(Exception erro, int? id = null, string controller = null, string action = null)
        {
            try
            {
                var logDAO = new LogWebDAO();

                var logModel = new LogWebViewModel
                {
                    Id = logDAO.ProximoId(),
                    DataGeracao = DateTime.Now,
                    Callstack = erro.StackTrace,
                    TipoOperacao = EnumTipoLog.Erro,
                    Mensagem = erro.Message,
                    Detalhes = "Um erro ocorreu ao processar uma requisição"
                };

                if (controller != null)
                    logModel.Detalhes += ". Controller: " + controller + Environment.NewLine;

                if (action != null)
                    logModel.Detalhes += ". Action: " + action + Environment.NewLine;

                if (id != null)
                    logModel.Detalhes += ". Id de Referência: " + id;

                logDAO.Insert(logModel);
            }
            catch { }
        }

        public static void GeraLogErro<T>(Exception erro, T model = null, string controller = null, string action = null) where T : BaseDatabaseModel
        {
            try
            {
                var logDAO = new LogWebDAO();

                var logModel = new LogWebViewModel
                {
                    Id = logDAO.ProximoId(),
                    DataGeracao = DateTime.Now,
                    Callstack = erro.StackTrace,
                    TipoOperacao = EnumTipoLog.Erro,
                    Mensagem = erro.Message,
                    Detalhes = "Um erro ocorreu ao processar uma requisição"
                };

                if (controller != null)
                    logModel.Detalhes += ". Controller: " + controller + Environment.NewLine;

                if (action != null)
                    logModel.Detalhes += ". Action: " + action + Environment.NewLine;

                if (model != null)
                    logModel.Detalhes += ". Model: " + JsonConvert.SerializeObject(model);

                logDAO.Insert(logModel);
            }
            catch { }
        }
    }
}
