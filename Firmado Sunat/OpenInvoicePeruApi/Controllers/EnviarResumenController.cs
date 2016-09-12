﻿using System;
using System.Web.Http;
using OpenInvoicePeru.FirmadoSunat;
using OpenInvoicePeru.FirmadoSunat.Models;

namespace OpenInvoicePeruApi.Controllers
{
    [Authorize]
    public class EnviarResumenController : ApiController
    {
        public EnviarResumenResponse Post([FromBody]EnviarDocumentoRequest request)
        {
            var response = new EnviarResumenResponse();
            var serializador = new Serializador();
            var nombreArchivo = $"{request.Ruc}-{request.TipoDocumento}-{request.IdDocumento}";

            try
            {
                var tramaZip = serializador.GenerarZip(request.TramaXmlFirmado, nombreArchivo);

                var conexionSunat = new ConexionSunat(new ConexionSunat.Parametros
                {
                    Ruc = request.Ruc,
                    UserName = request.UsuarioSol,
                    Password = request.ClaveSol,
                    EndPointUrl = request.EndPointUrl
                });

                var resultado = conexionSunat.EnviarResumenBaja(tramaZip, $"{nombreArchivo}.zip");

                if (resultado.Item2)
                {
                    response.NroTicket = resultado.Item1;
                    response.Exito = true;
                }
                else
                {
                    response.MensajeError = resultado.Item1;
                    response.Exito = false;
                }
            }
            catch (Exception ex)
            {
                response.MensajeError = ex.Message;
                response.Pila = ex.StackTrace;
                response.Exito = false;
            }
            
            return response;
        }
    }
}
