using Dummy_Sensor.DAO;
using Dummy_Sensor.Models;
using System;
using System.Threading;

namespace Dummy_Sensor
{
    internal class Program
    {
        private static Random randomGenerator = new Random();
        private static int registrosRestantes = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Este programa é utilizado para gerar registros de medição.");

            while (!PreparaAmbiente()) { Console.WriteLine("Preparando ambiente"); };

            while (registrosRestantes > 0)
            {
                GeraDados();
                Thread.Sleep(10);
            }
        }

        static bool PreparaAmbiente()
        {
            try
            {
                var dispositivoDAO = new DispositivoDAO();
                var bairroDAO = new BairroDAO();
                var medicaoDAO = new MedicaoDAO();

                if (bairroDAO.Consulta(999) == null)
                    bairroDAO.Insert(new BairroViewModel
                    {
                        Id = 999,
                        Descricao = "CEFSA",
                        CEP = "09850-550",
                        Latitude = -23.7387346,
                        Longitude = -46.595561
                    });

                if (dispositivoDAO.Consulta(999) == null)
                    dispositivoDAO.Insert(new DispositivoViewModel
                    {
                        Id = 999,
                        Descricao = "Dummy System",
                        BairroId = 999,
                        DataAtualizacao = DateTime.Now,
                        MedicaoReferencia = 100
                    });

                // 10000 registros são o suficiente
                registrosRestantes = 10000 - medicaoDAO.List().Count;

                return true;
            }
            catch { return false; }
        }

        static void GeraDados()
        {
            try
            {
                var medicaoDAO = new MedicaoDAO();

                medicaoDAO.Insert(new MedicaoViewModel
                {
                    DispositivoId = 999,
                    DataMedicao = DateTime.Now.AddDays(-randomGenerator.NextDouble() * 30),
                    ValorChuva = randomGenerator.NextDouble() * 100,
                    ValorNivel = randomGenerator.NextDouble() * 100
                });

                registrosRestantes -= 1;
            }
            catch
            {

            }
        }
    }
}
