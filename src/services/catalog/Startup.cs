using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace CiK.Catalog {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddMvcCore ().AddVersionedApiExplorer (
                options => {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            services.AddMvc ();

            services.AddApiVersioning (o => o.ReportApiVersions = true);
            services.AddSwaggerGen (
                options => {
                    // resolve the IApiVersionDescriptionProvider service
                    // note: that we have to build a temporary service provider here because one has not been created yet
                    var provider = services.BuildServiceProvider ().GetRequiredService<IApiVersionDescriptionProvider> ();

                    // add a swagger document for each discovered API version
                    // note: you might choose to skip or document deprecated API versions differently
                    foreach (var description in provider.ApiVersionDescriptions) {
                        options.SwaggerDoc (description.GroupName, CreateInfoForApiVersion (description));
                    }
                    // integrate xml comments
                    // options.IncludeXmlComments (XmlCommentsFilePath);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseMvc ();
            app.UseSwagger (c => {
                if (!env.IsDevelopment ()) {
                    // TODO: temporary to hardcode `c` 
                    var basepath = "/c";
                    c.PreSerializeFilters.Add ((swaggerDoc, httpReq) => swaggerDoc.BasePath = basepath);
                    c.PreSerializeFilters.Add ((swaggerDoc, httpReq) => {
                        IDictionary<string, PathItem> paths = new Dictionary<string, PathItem> ();
                        foreach (var path in swaggerDoc.Paths) {
                            paths.Add (path.Key.Replace (basepath, "/"), path.Value);
                        }
                        swaggerDoc.Paths = paths;
                    });
                }
            });

            app.UseSwaggerUI (
                options => {
                    var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider> ();
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions) {
                        if (!env.IsDevelopment ()) {
                            options.SwaggerEndpoint ($"/c/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant ());
                        } else {
                            // TODO: temporary to hardcode `c` 
                            options.SwaggerEndpoint ($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant ());
                        }
                    }
                });
        }

        static string XmlCommentsFilePath {
            get {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof (Startup).GetTypeInfo ().Assembly.GetName ().Name + ".xml";
                return Path.Combine (basePath, fileName);
            }
        }

        static Info CreateInfoForApiVersion (ApiVersionDescription description) {
            var info = new Info () {
                Title = $"Sample API {description.ApiVersion}",
                Version = description.ApiVersion.ToString (),
                Description = "A sample application with Swagger, Swashbuckle, and API versioning.",
                Contact = new Contact () { Name = "Thang Chung", Email = "thangchung@ymail.com" },
                TermsOfService = "Shareware",
                License = new License () { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
            };

            if (description.IsDeprecated) {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}