using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ADO
{
    public static class Dependencies
    {
        public static readonly string SERVICE_SUFFIX = "Logic";
        public static void SetDependencies(this IServiceCollection services, IConfiguration Configuration)
        {

            //services.AddDbContext<musicfyContext>(options =>
           // {
            //    options.UseSqlServer(Configuration.GetConnectionString("musicfy"));
            //});

            AddServices(services);

        }


        private static void AddServices(IServiceCollection services)
        {
            var types = typeof(Logic.VentaLogic).Assembly.GetTypes();
            var selectedTypes = types
                .Where(t => t.Namespace != null)
                .Where(t => !t.Name.Contains("<"))
                .Where(t => !t.IsAbstract && t.GetConstructors().Length > 0)
                .Where(t => t.Name.EndsWith(SERVICE_SUFFIX)).ToList();

            foreach (var s in selectedTypes)
            {
                services.AddScoped(s, s);
            }
        }
    }
}
