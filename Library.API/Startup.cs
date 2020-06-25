using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Filters;
using Library.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;


namespace Library.API
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
            _ = services.AddMvc(config =>
              {
                  config.Filters.Add<JsonExceptionFilter>();
                  config.ReturnHttpNotAcceptable = true;
                  config.OutputFormatters.Add(new XmlSerializerOutputFormatter());//支持xml格式

                  // 配置缓存
                  config.CacheProfiles.Add(
                      "Default", new CacheProfile { Duration = 60}
                      );
                  config.CacheProfiles.Add(
                      "Never", new CacheProfile { Location = ResponseCacheLocation.None, NoStore = true }
                      );
              }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            //.AddXmlSerializerFormatters()
            ;
            //services.AddScoped<IAuthorRepository, AuthorMockRepository>();
            //services.AddScoped<IBookRepository, BookMockRepository>();


            // 添加仓储
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            // 添加Db
            services.AddDbContext<LibraryDbContext>(
                option => option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(Startup).Assembly.GetName().Name)));
            // 添加 Identity 服务，要在添加 services.AddAuthentication() 服务之前
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<LibraryDbContext>();
            // 添加AutoMapper
            services.AddAutoMapper(typeof(Startup));
            // 添加Filter
            services.AddScoped<CheckAuthorExistActionFilterAttribute>();
            // 添加响应缓存中间件
            services.AddResponseCaching(options =>
            {
                //options.SizeLimit = 100;
                options.UseCaseSensitivePaths = true;
                options.MaximumBodySize = 1024;
            });
            // 添加内存缓存中间件
            services.AddMemoryCache();
            // 添加 api 版本控制中间件
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                // 默认的查询字符串的版本号变量名是 api-version
                //options.ApiVersionReader = new QueryStringApiVersionReader("ver");
                // 通过请求消息头指定访问的api版本
                //options.ApiVersionReader = new HeaderApiVersionReader("api-version");

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new MediaTypeApiVersionReader(),
                    new QueryStringApiVersionReader("ver")
                    );
            });

            // 添加认证中间件
            var tokenSection = Configuration.GetSection("Security:Token");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = tokenSection["Issuer"],
                        ValidAudience = tokenSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSection["Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // 添加 Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Library API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            NLogBuilder.ConfigureNLog("nlog.config");// 配置Nlog
            app.UseHttpsRedirection();
            app.UseResponseCaching(); // 使用 Http 缓存中间件，放在 UseMvc() 之前
            app.UseAuthentication(); // 添加认证功能
            app.UseSwagger(); // 使用 Swagger
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
            });
            app.UseMvc();
        }
    }
}
