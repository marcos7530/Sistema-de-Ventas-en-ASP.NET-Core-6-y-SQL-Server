using Firebase.Storage;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using Firebase.Auth;



namespace SistemaVenta.BLL.Implementacion
{
    public class FirebaseServicio : IFirebaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        //ESTO HACE QUE SE INYECTE EL REPOSITORIO DE CONFIGURACION
        public FirebaseServicio(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";

            try
            {


                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //optener la api_key de firebase para autenticar

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);


                var cancellation = new System.Threading.CancellationTokenSource();

                //esto es un objeto de tipo FirebaseStorage que almacena la ruta de firebase y las opciones de autenticacion
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;

            }
            catch
            {
                UrlImagen = "";
            }


            return UrlImagen;
        }

        public async Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo)
        {

            try
            {


                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                //optener la api_key de firebase para autenticar

                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));

                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);


                var cancellation = new System.Threading.CancellationTokenSource();

                //esto es un objeto de tipo FirebaseStorage que almacena la ruta de firebase y las opciones de autenticacion
                var task = new FirebaseStorage(
                    Config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(Config[CarpetaDestino])
                    .Child(NombreArchivo)
                    .DeleteAsync();

                 await task;

                return true;

            }
            catch
            {
                return true;
            }



        }


    }
}
