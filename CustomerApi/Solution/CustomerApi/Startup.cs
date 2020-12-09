using AutoMapper;
using CustomerApi.Data.Database;
using CustomerApi.Data.Repository.v1;
using CustomerApi.Domain.Entities;
using CustomerApi.Messaging.Send.Options.v1;
using CustomerApi.Messaging.Send.Sender.v1;
using CustomerApi.Models.v1;
using CustomerApi.Service.v1.Command;
using CustomerApi.Service.v1.Query;
using CustomerApi.Validators.v1;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CustomerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));

            services.AddDbContext<CustomerContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton
            );

            services.AddMemoryCache();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers()
                .AddFluentValidation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Customer Api",
                    Description = "A simple API to create or update customers",
                    Contact = new OpenApiContact
                    {
                        Name = "Wolfgang Ofner",
                        Email = "WolfgangOfner@aon.at",
                        Url = new Uri("https://www.programmingwithwolfgang.com/")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var actionExecutingContext =
                        actionContext as ActionExecutingContext;

                    if (actionContext.ModelState.ErrorCount > 0
                        && actionExecutingContext?.ActionArguments.Count == actionContext.ActionDescriptor.Parameters.Count)
                    {
                        return new UnprocessableEntityObjectResult(actionContext.ModelState);
                    }

                    return new BadRequestObjectResult(actionContext.ModelState);
                };
            });

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IValidator<CreateCustomerModel>, CreateCustomerModelValidator>();
            services.AddScoped<IValidator<UpdateCustomerModel>, UpdateCustomerModelValidator>();

            services.AddScoped<ICustomerUpdateSender, CustomerUpdateSender>();

            services.AddScoped<IRequestHandler<CreateCustomerCommand, Customer>, CreateCustomerCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCustomerCommand, Customer>, UpdateCustomerCommandHandler>();
            services.AddScoped<IRequestHandler<GetCustomerByIdQuery, Customer>, GetCustomerByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllCustomerQuery, List<Customer>>, GetAllCustomerQueryHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CustomerContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            context.Database.EnsureCreated();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
