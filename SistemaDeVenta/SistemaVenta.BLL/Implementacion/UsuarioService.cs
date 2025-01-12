using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave= _utilidadesService.ConvertirSHA256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null){
                    string url_foto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = url_foto;
                }

                Usuario usuario_creado = await _repositorio.Crear(entidad);


                if (usuario_creado.IdUsuario==0)
                    throw new TaskCanceledException("No se puedo crear el usuario");
               

                if (UrlPlantillaCorreo != ""){
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("{correo}", usuario_creado.Correo).Replace("[clave]",clave_generada);


                    string html_correo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();



                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;

                            if (response.CharacterSet == null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            html_correo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();

                        }


                    }

                    if (html_correo != "")
                        await _correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta Creada", html_correo);

                }
                
                IQueryable<Usuario> query = await _repositorio.Consultar(u=>u.IdUsuario==usuario_creado.IdUsuario);
                usuario_creado = query.Include(rol => rol.IdRolNavigation).First();

                return usuario_creado;

            }
            catch(Exception ex)
            {
                throw;
            }


        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(x => x.Correo == entidad.Correo && x.IdUsuario != entidad.IdUsuario);
            if (usuario_existe != null)
            {
                throw new TaskCanceledException("El correo ya existe");
            }


            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);

                Usuario usuario_editar = queryUsuario.First();

                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;

                if (usuario_editar.NombreFoto=="")
                    usuario_editar.NombreFoto = NombreFoto;
                

                if (Foto!=null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto,"carpeta_usuario" , usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto=urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuario_editar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo modificar el usuario");

                Usuario usuario_editado = queryUsuario.Include(r=>r.IdRolNavigation).First();

                return usuario_editado;

            }
            catch 
            {
                throw;
            
            }




        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");
                    
                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);

                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                return true;

            }
            catch
            {
                throw;
            }
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
