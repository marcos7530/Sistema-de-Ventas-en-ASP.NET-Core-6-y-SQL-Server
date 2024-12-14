using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Implementacion
{
    //esta clase hereda de GenericRepository y de IVentaRepository
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        //establece una variable de solo lectura de tipo DbventaContext
        private readonly DbventaContext _dbContext;

        //constructor de la clase
        public VentaRepository(DbventaContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<IQueryable<Venta>> Consultar(Expression<Func<Venta, bool>> filtro = null)
        {
            throw new NotImplementedException();
        }

        
        public Task<Venta> Crear(Venta entidad)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> Editar(Venta entidad)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Eliminar(Venta entidad)
        {
            throw new NotImplementedException();
        }

        public Task<Venta> Obtener(Expression<Func<Venta, bool>> filtro)
        {
            throw new NotImplementedException();
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach(DetalleVenta dv in entidad.DetalleVenta)
                    { 
                    
                        Producto producto_encontrado=_dbContext.Productos.Where(p=>p.IdProducto==dv.IdProducto).First();

                        producto_encontrado.Stock=producto_encontrado.Stock-dv.Cantidad;
                        _dbContext.Productos.Update(producto_encontrado);
                    }
                    await _dbContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "venta").First();


                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);
                    entidad.NumeroVenta=numeroVenta;

                    await _dbContext.Venta.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;

                    transaction.Commit();
                }
               catch(Exception e)
                {
                    transaction.Rollback();
                    throw e;
                } 
                  
            }

            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> listaResumen = await _dbContext.DetalleVenta
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date && 
                dv.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return listaResumen;
        }
    }
}
