using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;


        public UsuarioService(
        IGenericRepository<Usuario> repositorio,
        IFirebaseService firebaseService,
        IUtilidadesService utilidadesService,
        ICorreoService correoService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query =await _repositorio.Consultar();
            return query.Include(rol=>rol.IdRolNavigation).ToList();
        }


        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
           Usuario usuario_existe = await _repositorio.Obtener(x => x.Correo == entidad.Correo);
            if (usuario_existe != null)
            {
                throw new TaskCanceledException("El correo ya se encuentra registrado");
            }

            try
            {

            }
            catch
            { 
            
            }


        }

        public Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(int IdUsuario)
        {
            throw new NotImplementedException();
        }



        public Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> ObtenerPorId(int IdUsuario)
        {
            throw new NotImplementedException();
        }








        public Task<bool> GuardarPerfil(Usuario entidad)
        {
            throw new NotImplementedException();
        }


        public Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva)
        {
            throw new NotImplementedException();
        }


        public Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            throw new NotImplementedException();
        }
    }
}
