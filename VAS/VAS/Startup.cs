using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using VAS.Data;
using VAS.Data.Infrastructure;
using VAS.Data.Repositories;
using VAS.HangfireJob;
using VAS.Hubs;
using VAS.Model;
using VAS.Service;
using VAS.Utils;

namespace VAS
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ===== Add our DbContext ========
            services.AddDbContext<VASDbContext>();

            #region DI solutions
            //add for data
            services.AddScoped<IDbFactory, DbFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            //EventLog
            services.AddTransient<IDoctorBasicRepository, DoctorBasicRepository>();
            services.AddTransient<IDoctorBasicService, DoctorBasicService>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IRoomService, RoomService>();

            //SignalR Connection
            services.AddTransient<ISignalRConnectionRepository, SignalRConnectionRepository>();
            services.AddTransient<ISignalRConnectionService, SignalRConnectionService>();

            //Mail
            services.AddTransient<IMailService, MailService>();

            services.AddTransient<INurseRepository, NurseRepository>();
            services.AddTransient<INurseService, NurseService>();
            services.AddTransient<ISpecialityRepository, SpecialityRepository>();
            services.AddTransient<ISpecialityService, SpecialityService>();
            services.AddTransient<ISchedulingRepository, SchedulingRepository>();
            services.AddTransient<ISchedulingService, SchedulingService>();
            services.AddTransient<IBlockRepository, BlockRepository>();
            services.AddTransient<IBlockService, BlockService>();
            services.AddTransient<ITicketRepository, TicketRepository>();
            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IFamilyRepository, FamilyRepository>();
            services.AddTransient<IFamilyService, FamilyService>();
            services.AddTransient<ISpecialityDoctorRepository, SpecialityDoctorRepository>();
            services.AddTransient<ISpecialityDoctorService, SpecialityDoctorService>();

            #endregion

            #region Setup1
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            #endregion

            #region Identity
            services.AddAuthorization();
            var authBuilder = services.AddIdentityCore<MyUser>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
                o.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                o.Lockout.MaxFailedAccessAttempts = 5;
                o.Lockout.AllowedForNewUsers = true;

                // User settings.
                o.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                o.User.RequireUniqueEmail = false;
            });
            authBuilder = new IdentityBuilder(authBuilder.UserType, typeof(IdentityRole), authBuilder.Services);
            authBuilder.AddEntityFrameworkStores<VASDbContext>().AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            //    options.SlidingExpiration = true;
            //});

            services.AddIdentity<MyUser, IdentityRole>()
                .AddEntityFrameworkStores<VASDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<MyUser>, UserClaimsPrincipalFactory<MyUser, IdentityRole>>();

            //security key
            string securityKey = "qazedcVFRtgbNHYujmKIolp";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = securityKey,
                    ValidAudience = securityKey
                };

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (path.StartsWithSegments("/centerHub"))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            #region Swagger
            services.AddSwagger();
            #endregion

            #region Cors
            services.AddCors(options =>
            options.AddPolicy("AllowAll", builder => builder
                                    .WithOrigins("http://localhost:4300", "http://localhost:4200", "http://vas.hisoft.vn")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials()));
            #endregion

            #region Hangfire 
            services.AddHangfire(x => x.UseSqlServerStorage(@"Server=202.78.227.89;Database=vas-hangfire;user id=sa;password=an@0906782333;Trusted_Connection=True;Integrated Security=false;"));

            #endregion

            #region SignalR
            services.AddSignalR();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RoleManager<IdentityRole> roleManager, UserManager<MyUser> userManager, ITicketService _ticketService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }



            app.UseAuthentication();
            app.UseStaticFiles();

            #region Swagger
            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.GeneratorSettings.Title = "VAS API";

                settings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));

                settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("Bearer",
                    new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = SwaggerSecurityApiKeyLocation.Header
                    }));
            });
            #endregion

            #region Identity
            var task = RolesExtenstions.InitAsync(roleManager, userManager);
            task.Wait();
            #endregion

            #region MapsterMapper
            MapsterConfig map = new MapsterConfig();
            map.Run();
            #endregion

            #region Hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            TestBackgroud background = new TestBackgroud(_ticketService);
            //BackgroundJob.Enqueue(() => background.ChangStatusOnTicket());
            RecurringJob.AddOrUpdate(() => background.ChangeStatusOnTicket(), "0,30 7-23 * * *");
            #endregion

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            #region SignalR
            app.UseSignalR(r =>
            {
                r.MapHub<CenterHub>("/centerHub");
            });
            #endregion

            app.UseMvc();

        }
    }
}
