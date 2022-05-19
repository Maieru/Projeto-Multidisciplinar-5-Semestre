using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoReader.DAO
{
    public class MedicaoMongoObject
    {
        public ObjectId _id { get; set; }
        public DateTime recvTime { get; set; }
        public string attrName { get; set; }
        public string attrType { get; set; }
        public string attrValue { get; set; }
    }

    public static class MongoDAO
    {
        private static IMongoCollection<BsonDocument> GetTabelaDispositivo(int idDispositivo)
        {
            //mongodb://helix:H3l1xNG@{IP}:27000/?authMechanism=DEFAULT
            string connectionString = "mongodb://127.0.0.1:27017";
            var client = new MongoClient(connectionString);
            var banco = client.GetDatabase("sth_helixiot");
            return banco.GetCollection<BsonDocument>($"sth_/_urn:ngsi-Id:Station:{idDispositivo:000}_Station");
        }

        public static List<MedicaoMongoObject> GetMedicoesUltimoMinuto(int idDispositivo)
        {
            var tabela = GetTabelaDispositivo(idDispositivo);

            // Alterar o .ToString para o formato gerado no Helix 
            string horarioUltimoMinutoFormatado = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

            var filtro = Builders<BsonDocument>.Filter.Gte("recvTime", horarioUltimoMinutoFormatado);

            var registros = tabela.Find(filtro);

            var listaRetorno = new List<MedicaoMongoObject>();

            foreach (var registro in registros.ToList())
                listaRetorno.Add(BsonSerializer.Deserialize<MedicaoMongoObject>(registro));

            return listaRetorno;
        }
    }
}
