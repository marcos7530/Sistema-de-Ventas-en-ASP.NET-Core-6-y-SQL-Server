using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaVenta.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
//using SistemaVenta.DAL.Implementacion;
//using SistemaVenta.DAL.Interfaces;
//using SistemaVenta.BLL.Implementacion;
//using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.IOC
{
    public static class Dependencias
    {

        public static void InyectarDependecia(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DbventaContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("CadenaSQL")));

        }


    }
}
