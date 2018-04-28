// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using CiK.Security.Quickstart;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Hosting = Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CiK.Security
{
  public class Startup
  {
    public Hosting.IHostingEnvironment HostEnvironment { get; }
    public IConfiguration Configuration { get; }

    public Startup(Hosting.IHostingEnvironment environment, IConfiguration configuration)
    {
      HostEnvironment = environment;
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();

      services.Configure<IISOptions>(options =>
      {
        options.AutomaticAuthentication = false;
        options.AuthenticationDisplayName = "Windows";
      });

      var builder = services.AddIdentityServer(options =>
      {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
      })
          .AddTestUsers(TestUsers.Users);

      // in-memory, code config
      builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
      builder.AddInMemoryApiResources(Config.GetApis());
      builder.AddInMemoryClients(Config.GetClients());

      // in-memory, json config
      //builder.AddInMemoryIdentityResources(Configuration.GetSection("IdentityResources"));
      //builder.AddInMemoryApiResources(Configuration.GetSection("ApiResources"));
      //builder.AddInMemoryClients(Configuration.GetSection("clients"));

      //TODO: hard developer credentail because only for testing
      builder.AddDeveloperSigningCredential();
      /*if (Environment.IsDevelopment ()) {
          builder.AddDeveloperSigningCredential ();
      } else {
          throw new Exception ("need to configure key material");
      }*/

      services.AddAuthentication()
          .AddGoogle(options =>
          {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
            options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";
          });
    }

    public void Configure(IApplicationBuilder app)
    {
      if (HostEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      string basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
      if (!string.IsNullOrEmpty(basePath))
      {
        app.Use(async (context, next) =>
        {
          context.Request.PathBase = basePath;
          await next.Invoke();
        });
      }
      
      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.All
      });

      app.UseIdentityServer();
      app.UseStaticFiles();
      app.UseMvcWithDefaultRoute();
    }
  }
}