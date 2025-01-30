using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ridely.Api.Controllers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ridely.Api.OpenApi
{
    public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : 
        IConfigureNamedOptions<SwaggerGenOptions>
    {
        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach(var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }
        }

        private static OpenApiInfo CreateVersionInfo(ApiVersionDescription versionDescription)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"Ridely.Api v{versionDescription.ApiVersion}",
                Description = "V." + versionDescription.ApiVersion.ToString()
            };

            if (versionDescription.IsDeprecated)
                openApiInfo.Title += " This Api version has been deprecated";

            if(versionDescription.GroupName.Equals(SwaggerGroupNames.Admin, StringComparison.InvariantCultureIgnoreCase))
            {
                openApiInfo.Title = "Ridely.Api Admin";
                openApiInfo.Description = "Ridely Admin Endpoints";
            }

            return openApiInfo;
        }
    }
}
