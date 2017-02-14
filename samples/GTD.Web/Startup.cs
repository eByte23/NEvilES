using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;
using StructureMap;
using NEvilES.Pipeline;
using GTD.Web.Domain;
using GTD.Web.Commands;
using NEvilES;
using System.Data;

namespace GTD.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            return ConfigureIoC(services);
        }

        public IServiceProvider ConfigureIoC(IServiceCollection services)
        {
            var container = new Container();

            container.Configure(x =>
            {
                x.Scan(s =>
                {
                    s.AssemblyContainingType<Domain.Client>();
                    s.AssemblyContainingType<Events.ClientCreated>();
                    s.AssemblyContainingType<NewClient>();
                    s.AssemblyContainingType<ICommandProcessor>();

                    s.ConnectImplementationsToTypesClosing(typeof(IProcessCommand<>));
                    s.ConnectImplementationsToTypesClosing(typeof(IHandleStatelessEvent<>));
                    s.ConnectImplementationsToTypesClosing(typeof(IHandleAggregateCommandMarker<>));
                    s.ConnectImplementationsToTypesClosing(typeof(INeedExternalValidation<>));
                    s.ConnectImplementationsToTypesClosing(typeof(IProject<>));
                    s.ConnectImplementationsToTypesClosing(typeof(IProjectWithResult<>));

                    s.WithDefaultConventions();
                    s.SingleImplementationsOfInterface();
                });

                x.For<ICommandProcessor>().Use<PipelineProcessor>();
               // x.For<IRepository>().Use<InMemoryEventStore>();
               // x.For<IReadModel>().Use<TestReadModel>();

                x.For<CommandContext>().Use("CommandContext", s => new CommandContext(new CommandContext.User(Guid.NewGuid(), 666), Guid.NewGuid(), Guid.NewGuid(), new CommandContext.User(Guid.NewGuid(), 007), ""));
                //x.For<IDbConnection>().Use("Connection", s => new SqlConnection(s.GetInstance<IConnectionString>().ConnectionString));
                x.For<IDbConnection>().Use("Connection", s => new NpgsqlConnection(s.GetInstance<IConnectionString>().ConnectionString));
                //User ID=root;Password=myPassword;Host=localhost;Port=5432;Database=myDataBase;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;

                x.For<IDbTransaction>().Use("Transaction", s => s.GetInstance<IDbConnection>().BeginTransaction());
            });
                return container.GetInstance<IServiceProvider>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
