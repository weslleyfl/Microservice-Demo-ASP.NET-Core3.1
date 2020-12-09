using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OrderApi.Data.Database;
using OrderApi.Data.Repository.v1;
using OrderApi.Domain;
using OrderApi.Messaging.Receive.Options.v1;
using OrderApi.Messaging.Receive.Receiver.v1;
using OrderApi.Models.v1;
using OrderApi.Service.v1.Command;
using OrderApi.Service.v1.Query;
using OrderApi.Service.v1.Services;
using OrderApi.Validators.v1;


namespace OrderApi
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
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection(RabbitMqConfiguration.RabbitMq));

            services.AddMemoryCache();

            services.AddDbContext<OrderContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton
             );

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers()
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    x.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                })
                .AddFluentValidation();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Order Api",
                    Description = "Uma API simples para criar ou pagar Ordens (Pedidos)",
                    Contact = new OpenApiContact
                    {
                        Name = "Weslley Lopes",
                        Email = "weslley@aon.at",
                        Url = new Uri("https://www.programmingwithwolfgang.com/")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // customizando os model validation errors response [ApiController] 
            // gives developers the control to write custom responses. 
            // https://www.c-sharpcorner.com/blogs/customizing-model-validation-response-resulting-as-http-400-in-net-core
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

            services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(ICustomerNameUpdateService).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IValidator<OrderModel>, OrderModelValidator>();

            services.AddScoped<IRequestHandler<GetPaidOrderQuery, List<Order>>, GetPaidOrderQueryHandler>();
            services.AddScoped<IRequestHandler<GetOrderByIdQuery, Order>, GetOrderByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetOrderByCustomerGuidQuery, List<Order>>, GetOrderByCustomerGuidQueryHandler>();
            services.AddScoped<IRequestHandler<CreateOrderCommand, Order>, CreateOrderCommandHandler>();
            services.AddScoped<IRequestHandler<PayOrderCommand, Order>, PayOrderCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateOrderCommand>, UpdateOrderCommandHandler>();
            services.AddScoped<ICustomerNameUpdateService, CustomerNameUpdateService>();

            // RabbitMq Configuration
            // The last thing I have to do is to register my CustomerFullNameUpdateReceiver 
            // class as a background service
            services.AddHostedService<CustomerFullNameUpdateReceiver>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, OrderContext context)
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
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
