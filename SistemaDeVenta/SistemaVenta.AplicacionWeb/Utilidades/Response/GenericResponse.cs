namespace SistemaVenta.AplicacionWeb.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        //esto es un generico y se puede usar en cualquier parte del proyecto


        public bool Estado { get; set; }

        public string? Mensaje { get; set; }

        public TObject? Objeto { get; set; }

        public List<TObject> ListaObjeto { get; set; }



    }
}
