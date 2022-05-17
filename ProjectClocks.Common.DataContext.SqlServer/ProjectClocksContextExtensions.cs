using Microsoft.EntityFrameworkCore; // UseSqlServer
using Microsoft.Extensions.DependencyInjection; // IServiceCollection


namespace ProjectClocks.Common.EntityModels.SqlServer
{
    public static class ProjectClocksContextExtensions
    {
        /// <summary>
        /// Adds ProjectClocksContext to the specified IServiceCollection. Uses the SqlServer database provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">Set to override the default.</param>
        /// <returns>An IServiceCollection that can be used to add more services.</returns>
        public static IServiceCollection AddProjectClocksContext(this IServiceCollection services, 
        string connectionString = "Data Source=.;Initial Catalog=ProjectClocks;"
        + "Integrated Security=true;MultipleActiveResultsets=true;")
        {
            services.AddDbContext<ProjectClocksContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
