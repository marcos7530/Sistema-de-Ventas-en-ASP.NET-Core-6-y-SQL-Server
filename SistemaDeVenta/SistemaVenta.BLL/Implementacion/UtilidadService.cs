using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadService : IUtilidadesService
    {

        public string GenerarClave()
        {

            //esto es para generar una clave aleatoria
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);

            //esto retorna la clave generada
            return clave;
        }

        public string ConvertirSHA256(string texto)
        {
            //esto es para convertir a sha256 
            StringBuilder sb = new StringBuilder();

            //esto funciona para encriptar la contraseña 
            using (SHA256 sha256 = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = sha256.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            //esto retorna la contraseña encriptada 
            return sb.ToString();
        }

       
    }
}
