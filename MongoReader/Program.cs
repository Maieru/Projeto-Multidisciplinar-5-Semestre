using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoReader.DAO;
using MongoReader.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using MongoReader.DAO;
using System.Globalization;

namespace MongoReader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<DispositivoViewModel> listaDispositivoCache = null;
            DateTime ultimaAtualizacaoDaLista = DateTime.Now;

            while (true)
            {
                try
                {
                    if (listaDispositivoCache == null || DateTime.Now.Subtract(ultimaAtualizacaoDaLista).Minutes > 5)
                    {
                        var dispositivoDAO = new DispositivoDAO();
                        listaDispositivoCache = dispositivoDAO.List();
                        ultimaAtualizacaoDaLista = DateTime.Now;
                    }

                    foreach(var dispositivo in listaDispositivoCache)
                    {
                        var ultimasMedicoes = MongoDAO.GetMedicoesUltimoMinuto(dispositivo.Id);
                        var medicaoDAO = new MedicaoDAO();

                        foreach (var medicaoMongo in ultimasMedicoes)
                        {
                            var medicaoViewModel = MedicaoMongoToMedicaoViewModel(medicaoMongo, dispositivo.Id);

                            medicaoDAO.Insert(medicaoViewModel);
                        }
                    }
                }
                catch (Exception erro)
                {

                }

                Thread.Sleep(60000);
            }
        }

        static MedicaoViewModel MedicaoMongoToMedicaoViewModel(MedicaoMongoObject medicaoMongo, int idDispositivo)
        {
            var retorno = new MedicaoViewModel();

            retorno.DispositivoId = idDispositivo;
            retorno.DataMedicao = medicaoMongo.recvTime;

            if (medicaoMongo.attrName == "ValorNivel")
                retorno.ValorNivel = Convert.ToDouble(medicaoMongo.attrValue);
            else if (medicaoMongo.attrName == "ValorChuva")
                retorno.ValorChuva = Convert.ToDouble(medicaoMongo.attrValue);

            return retorno;
        }
    }
}
