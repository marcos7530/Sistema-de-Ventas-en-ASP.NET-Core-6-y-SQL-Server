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

                //esto es un diccionario que almacena el resultado de la consulta query y sirve para almacenar la configuracion del correo, donde la propiedad es la llave y el valor es el valor
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c =>c.Propiedad, elementSelector:c=>c.Valor);

                //esto es un objeto de tipo NetworkCredential que almacena las credenciales del correo, donde el usuario es el correo y la clave es la clave
                var credenciales = new NetworkCredential(Config["Correo"], Config["Clave"]);

                //esto es un objeto de tipo MailMessage que almacena el correo, donde el remitente es el correo, el asunto es el asunto, el cuerpo es el mensaje y es un cuerpo html
                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["Correo"], Config["alias"]),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true
                };

                //esto es un objeto de tipo MailAddress que almacena el correo destino
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
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
