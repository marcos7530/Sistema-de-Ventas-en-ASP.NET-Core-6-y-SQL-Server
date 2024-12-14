using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface ICorreoService
    {

        //esto es un metodo asincrono que recibe tres parametros y retorna un booleano
        //y sirve para enviar un correo
        Task<bool> EnviarCorreo(string correoDestino, string asunto, string mensaje);



    }
}
