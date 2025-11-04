using Microsoft.AspNetCore.JsonPatch;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DJualan.APIServer.Filters
{
    public class JsonPatchDocumentFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var patchParam = context.MethodInfo.GetParameters()
                .FirstOrDefault(p => p.ParameterType.IsGenericType &&
                                   p.ParameterType.GetGenericTypeDefinition() == typeof(JsonPatchDocument<>));

            if (patchParam != null)
            {
                // Clear the existing schema
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json-patch+json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "object",
                                    Required = new HashSet<string> { "op", "path" },
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["op"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Enum = new List<IOpenApiAny>
                                        {
                                            new OpenApiString("add"),
                                            new OpenApiString("remove"),
                                            new OpenApiString("replace"),
                                            new OpenApiString("move"),
                                            new OpenApiString("copy"),
                                            new OpenApiString("test")
                                        },
                                            Default = new OpenApiString("replace")
                                        },
                                        ["path"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Default = new OpenApiString("/name")
                                        },
                                        ["value"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Default = new OpenApiString("New Product Name")
                                        }
                                    }
                                }
                            },
                            Example = new OpenApiString(@"[
                            { ""op"": ""replace"", ""path"": ""/name"", ""value"": ""New Product Name"" }
                        ]")
                        }
                    }
                };
            }
        }
    }
}
