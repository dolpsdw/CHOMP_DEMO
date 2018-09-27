using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CHOMP_DEMO.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CHOMP_DEMO
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the authorization configuration
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, //issued with the correct issuer and audience details
                    ValidateAudience = true,
                    ValidIssuer = "chomp.chain",
                    ValidAudiences = new string[5] { "TG_Web","TG_Mobile", "Pkr_Web", "Pkr_Desk", "Pkr_Mob"},
                    ValidateLifetime = true, //Not expired
                    ValidateIssuerSigningKey = true, //signed with the same secret key that we’re using (from configuration)
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LARGESYMETRICCHUNKOFSTRINGMAYORTHAN128BITSOFLENGHTFORSHAREINSERVERS"))
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //DI config
            services.AddSingleton<ICacheManager, CacheManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication(); //Activate the authorization pipeline
            app.UseMvc();
        }
    }
}
