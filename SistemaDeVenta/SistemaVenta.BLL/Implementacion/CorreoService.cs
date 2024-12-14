using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class CorreoService : ICorreoService //esto es una clase que implementa la interfaz ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio; //esto es una variable de solo lectura de tipo IGenericRepository<Configuracion>

        public CorreoService(IGenericRepository<Configuracion> repositorio) //esto es el constructor de la clase
        {
            _repositorio = repositorio; //esto es igualar la variable _repositorio con el parametro repositorio y sirve para inyectar la dependencia
        }

        public async Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje)
        {
            try
            {
                //esto es una variable de tipo IQueryable<Configuracion> que almacena el resultado de la consulta
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c =>c.Propiedad, elementSelector:c=>c.Valor); 

                var credenciales = new NetworkCredential(Config["Correo"], Config["Clave"]);

                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["Correo"], Config["alias"]),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(new MailAddress(correoDestino));

                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                };

                clienteServidor.Send(correo);

            }
            catch
            {
                return false;
            }
        }
    }
}
