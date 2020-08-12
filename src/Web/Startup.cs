using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TakNotify;

namespace Web
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
            services.AddRazorPages();

            services
                .AddTakNotify()
                .AddProvider<SmtpProvider, SmtpProviderOptions>(options =>
                {
                    options.Server = Configuration.GetValue<string>("Smtp:Server");
                    options.Port = Configuration.GetValue<int>("Smtp:Port");
                    options.Username = Configuration.GetValue<string>("Smtp:Username");
                    options.Password = Configuration.GetValue<string>("Smtp:Password");
                    options.UseSSL = Configuration.GetValue<bool>("Smtp:UseSSL");
                    options.DefaultFromAddress = Configuration.GetValue<string>("Smtp:DefaultFromAddress");
                })
                .AddProvider<SendGridProvider, SendGridOptions>(options =>
                {
                    options.Apikey = Configuration.GetValue<string>("SendGrid:ApiKey");
                    options.DefaultFromAddress = Configuration.GetValue<string>("SendGrid:DefaultFromAddress");
                })
                .AddProvider<MailgunProvider, MailgunOptions>(options =>
                {
                    options.Apikey = Configuration.GetValue<string>("Mailgun:ApiKey");
                    options.DefaultFromAddress = Configuration.GetValue<string>("Mailgun:DefaultFromAddress");
                    options.Domain = Configuration.GetValue<string>("Mailgun:Domain");
                }, true)
                .AddProvider<AmazonSESProvider, AmazonSESOptions>(options =>
                {
                    options.AccessKey = Configuration.GetValue<string>("AmazonSES:AccessKey");
                    options.SecretKey = Configuration.GetValue<string>("AmazonSES:SecretKey");
                    options.RegionEndpoint = Configuration.GetValue<string>("AmazonSES:RegionEndpoint");
                    options.DefaultFromAddress = Configuration.GetValue<string>("AmazonSES:DefaultFromAddress");
                })
                .AddProvider<TwilioProvider, TwilioOptions>(options =>
                {
                    options.AccountSid = Configuration.GetValue<string>("Twilio:AccountSid");
                    options.AuthToken = Configuration.GetValue<string>("Twilio:AuthToken");
                    options.DefaultFromNumber = Configuration.GetValue<string>("Twilio:DefaultFromNumber");
                }, true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
