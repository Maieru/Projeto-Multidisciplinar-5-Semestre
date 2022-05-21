using AlertSystem.DAO;
using AlertSystem.Enum;
using AlertSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace AlertSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    var medicaoDAO = new MedicaoDAO();
                    var dispositivoDAO = new DispositivoDAO();
                    var bairroDAO = new BairroDAO();
                    var subscriberAlertaDAO = new SubscriberAlertaDAO();

                    var listaDispositivos = dispositivoDAO.List();
                    var listaDeUltimasMedicoes = dispositivoDAO.GetLatestMedicoes();
                    var usuariosCadastras = subscriberAlertaDAO.List();

                    foreach (var medicao in listaDeUltimasMedicoes)
                    {
                        var dispositivoDaMedicao = listaDispositivos.Where(d => d.Id == medicao.DispositivoId).FirstOrDefault();
                        var porcentagemCheia = medicao.ValorNivel / dispositivoDaMedicao.MedicaoReferencia;

                        if (porcentagemCheia > 0.25 && porcentagemCheia < 0.5)
                            FazGestaoEmail(usuariosCadastras, EnumTipoAlerta.AlertaAzul, dispositivoDaMedicao);
                        else if (porcentagemCheia < 0.75)
                            FazGestaoEmail(usuariosCadastras, EnumTipoAlerta.AlertaAmarelo, dispositivoDaMedicao);
                        else if (porcentagemCheia < 0.9)
                            FazGestaoEmail(usuariosCadastras, EnumTipoAlerta.AlertaLaranja, dispositivoDaMedicao);
                        else if (porcentagemCheia < 1)
                            FazGestaoEmail(usuariosCadastras, EnumTipoAlerta.AlertaVermelho, dispositivoDaMedicao);
                    }
                }
                catch (Exception erro)
                {

                }

                Thread.Sleep(600);
            }
        }

        public static void FazGestaoEmail (List<SubscriberAlertaViewModel> usuariosCadastrados, EnumTipoAlerta tipoAlerta, DispositivoViewModel dispositivoDaMedicao)
        {
            var bairroDAO = new BairroDAO();
            var subscriberAlertaDAO = new SubscriberAlertaDAO();
            foreach (var usuarioCadastro in usuariosCadastrados)
            {
                if (dispositivoDaMedicao.BairroId == usuarioCadastro.BairroId)
                {
                    if (usuarioCadastro.UltimaNotificacao != tipoAlerta && usuarioCadastro.Email != null)
                    {
                        var nomeBairro = bairroDAO.List()
                        .Where(b => b.Id == dispositivoDaMedicao.BairroId)
                        .Select(b => b.Descricao).FirstOrDefault();

                        EnviaEmailAlerta(usuarioCadastro, tipoAlerta, nomeBairro);

                        usuarioCadastro.UltimaNotificacao = tipoAlerta;

                        subscriberAlertaDAO.Update(usuarioCadastro);
                    }
                }
            }
        }
        public static void EnviaEmailAlerta(SubscriberAlertaViewModel usuarioCadastrado, EnumTipoAlerta tipoAlerta, string nomeBairro)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(EmailCredentials.EMAIL);
                message.To.Add(new MailAddress(usuarioCadastrado.Email));
                message.Subject = GeraAssuntoEmail(tipoAlerta, nomeBairro);
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = CriaMensagemEmail(tipoAlerta, nomeBairro); ;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(EmailCredentials.EMAIL, EmailCredentials.SENHA);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception) { }
        }
        public static string GeraAssuntoEmail (EnumTipoAlerta tipoAlerta, string bairro)
        {
            switch (tipoAlerta)
            {
                case EnumTipoAlerta.AlertaAzul:
                    return $"Alerta de Enchente Nível Azul no Bairro {bairro}";
                case EnumTipoAlerta.AlertaAmarelo:
                    return $"Alerta de Enchente Nível Amarelo no Bairro {bairro}";
                case EnumTipoAlerta.AlertaLaranja:
                    return $"Alerta de Enchente Nível Laranja no Bairro {bairro}";
                case EnumTipoAlerta.AlertaVermelho:
                    return $"Alerta de Enchente Nível Vermelho - Critico no Bairro {bairro}";
                default:
                    return string.Empty;
            }
        }
        public static string CriaMensagemEmail (EnumTipoAlerta tipoAlerta, string bairro)
        {
            switch (tipoAlerta)
            {
                case EnumTipoAlerta.AlertaAzul:
                    return $"Foi detectado que o nivel de água nos córregos do bairro {bairro} chegou a 25% da capacidade máxima";
                case EnumTipoAlerta.AlertaAmarelo:
                    return $"Foi detectado que o nivel de água nos córregos do bairro {bairro} chegou a 50% da capacidade máxima";
                case EnumTipoAlerta.AlertaLaranja:
                    return $"Foi detectado que o nivel de água nos córregos do bairro {bairro} chegou a 75% da capacidade máxima";
                case EnumTipoAlerta.AlertaVermelho:
                    return $"Foi detectado que o nivel de água nos córregos do bairro {bairro} chegou a 90% da capacidade máxima. O risco de enchetes é iminente !!!";
                default:
                    return string.Empty;
            }
        }
    }
}
