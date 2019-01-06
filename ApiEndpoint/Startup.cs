﻿namespace ApiEndpoint
{
    using AutoMapper;
    using DataEntity;
    using DataEntity.Model;
    using DataTransferObject;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NSwag;
    using NSwag.SwaggerGeneration.Processors.Security;
    using Provider;
    using Provider.Contracts;
    using Repository;
    using Repository.Contracts;
    using Repository.Operations;
    using Service;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            ConfigureDependencyInjectionService(services);

            services.AddCors();

            services.AddAutoMapper();

            ConfigureSwagger(services);

            ConfigureJsonReturnService(services);
        }

        private static void ConfigureJsonReturnService(IServiceCollection services)
            => services.AddMvc(options =>
                               {
                                   options.OutputFormatters.RemoveType<TextOutputFormatter>();
                                   options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                               })
                       .AddJsonOptions(options => // Resolves a self referencing loop when converting EF Entities to Json
                                       {
                                           options.SerializerSettings.ReferenceLoopHandling =
                                               ReferenceLoopHandling.Ignore;
                                       });

        private static IServiceCollection ConfigureSwagger(IServiceCollection services) =>
            services.AddSwaggerDocument(c =>
            {
                c.DocumentName = "apidocs";
                c.Title = "Sample API";
                c.Version = "v1";
                c.Description = "The sample API documentation description.";
                c.DocumentProcessors.Add(new SecurityDefinitionAppender("APIKey", new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Name = "APIKey",
                    In = SwaggerSecurityApiKeyLocation.Header,
                    Description = "APIKey"
                }));
                c.OperationProcessors.Add(new OperationSecurityScopeProcessor("APIKey"));
            });

        private static void ConfigureDependencyInjectionService(IServiceCollection services)
        {
            // Repositories Injection,
            services.AddScoped<IRepositoryFactory, UnitOfWork<DatabaseContext>>();
            services.AddScoped<IUnitOfWork, UnitOfWork<DatabaseContext>>();
            services.AddScoped<IUnitOfWork<DatabaseContext>, UnitOfWork<DatabaseContext>>();

            // Services Injection
            services.AddTransient(typeof(IGenericProvider<UsuarioEntity>), typeof(UsuarioProvider));

            services.AddTransient(typeof(IGenericProviderDto<LaboratorioDto, LaboratorioEntity>), typeof(LaboratorioProvider));
            services.AddTransient(typeof(IGenericProviderDto<FarmaciaDto, FarmaciaEntity>), typeof(FarmaciaProvider));
            services.AddTransient(typeof(IGenericProviderDto<MedicoDto, MedicoEntity>), typeof(MedicoProvider));
            services.AddTransient(typeof(IGenericProviderDto<PacienteDto, PacienteEntity>), typeof(PacienteProvider));

            services.AddTransient(typeof(IGenericProviderDto<TaskDto, TaskEntity>), typeof(TaskProvider));
            services.AddTransient(typeof(IGenericProviderDto<TaskListDto, TaskListEntity>), typeof(TaskListProvider));

            services.AddTransient(typeof(IParentChildrenProviderDto<FarmaciaDto, FarmaciaEntity>), typeof(LaboratorioFarmaciaProvider));

            services.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DatabaseContext context)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            ConfigureCors(app);

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc();
        }

        private static void ConfigureCors(IApplicationBuilder app) => app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }
}