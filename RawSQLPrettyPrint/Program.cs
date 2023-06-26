using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RawSQLPrettyPrint;
using RawSQLPrettyPrint.AST;
using RawSQLPrettyPrint.ConfigurationOptions;
using RawSQLPrettyPrint.Formatters;
using RawSQLPrettyPrint.Formatters.General;
using Serilog;
using Serilog.Formatting.Compact;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();
builder.Host.UseSerilog();

// Configure Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

// Configure endpoints and services
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1");
    });
}

app.UseSerilogRequestLogging();

app.MapGet("/api/sql", async context =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var rawSql = context.Request.Query["rawSql"].ToString();
    var httpClient = context.RequestServices.GetRequiredService<HttpClient>();

    if (string.IsNullOrEmpty(rawSql))
    {
        logger.LogError("Invalid input: Raw SQL is empty or null.");
        await context.Response.WriteAsync("Invalid input: Raw SQL is empty or null.");
        return;
    }

    // Fetch the AST from the remote API
    var astPayload = new
    {
        Dialect = "sqlserver",
        Script = rawSql
    };
    var astUrl = "https://sqlparsedev.azurewebsites.net/api/AbstractSyntaxTree";

    try
    {
        var response = await httpClient.PostAsJsonAsync(astUrl, astPayload);
        if (response.IsSuccessStatusCode)
        {
            // Read the response as JSON
            var astJson = await response.Content.ReadAsStringAsync();

            // Read the configuration from the JSON file
            var configuration = JsonConvert.DeserializeObject<SqlFormattingOptions>(File.ReadAllText("appsettings.json"));

            var formatters = new List<ISqlFormatter>();

            // Add UppercaseKeywordsFormatter based on WordCase option
            if (configuration.WordCase == WordCaseOption.Uppercase)
            {
                formatters.Add(new ReservedWordCapitalizationFormatter());
            }

            // Add AlignFirstWordOfClauseFormatter based on QueryAlignment option
            if (configuration.QueryAlignment != FirstWordOfClauseAlignmentOption.NoChange)
            {
               // formatters.Add(new AlignFirstWordOfClauseFormatter(configuration.QueryAlignment));
            }

            // Add ClauseElementPlacementFormatter based on ClauseElementPlacement option
            if (configuration.ClauseElementPlacement != ClauseElementPlacementOption.NoChange)
            {
               // formatters.Add(new ClauseElementPlacementFormatter(configuration.ClauseElementPlacement));
            }

            var formatterChain = new SqlFormatterChain(formatters);

            // Parse the AST hierarchy JSON into AstNode objects
            var astRoot = ParseAstHierarchy(astJson);

            // Apply the formatting
            //var formattedSql = formatterChain.Format(rawSql, astRoot);
            var formattedSql = "lll";

            logger.LogInformation("Received SQL query: {RawSql}", rawSql);

            await context.Response.WriteAsync(formattedSql);
        }
        else
        {
            var statusCode = (int)response.StatusCode;
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Failed to fetch the AST hierarchy.",
                Detail = "Failed to fetch the AST hierarchy. Please try again later."
            };

            logger.LogError("Failed to fetch the AST hierarchy. Status code: {StatusCode}", statusCode);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";
            await WriteJsonAsync(context.Response.Body, problemDetails);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while fetching the AST hierarchy.");

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred",
            Detail = ex.Message
        };

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await WriteJsonAsync(context.Response.Body, problemDetails);
    }
});

AstNode ParseAstHierarchy(string astJson)
{
    var jsonSettings = new JsonSerializerSettings
    {
        MissingMemberHandling = MissingMemberHandling.Error
    };

    var astHierarchy = JsonConvert.DeserializeObject<AstHierarchy>(astJson, jsonSettings);
    var astRoot = astHierarchy.Value.Children.FirstOrDefault();

    return ConstructAstNode(astRoot);
}

AstNode ConstructAstNode(AstNodeData astNodeData)
{
    var astNode = new AstNode
    {
        Name = astNodeData.Value,
        Type = astNodeData.Type,
        Children = new List<AstNode>()
    };

    if (astNodeData.Children != null)
    {
        foreach (var childNodeData in astNodeData.Children)
        {
            var childNode = ConstructAstNode(childNodeData);
            astNode.Children.Add(childNode);
        }
    }

    return astNode;
}

async Task WriteJsonAsync<TValue>(Stream responseStream, TValue value)
{
    await using var jsonWriter = new Utf8JsonWriter(responseStream, new JsonWriterOptions { Indented = true });
    System.Text.Json.JsonSerializer.Serialize(jsonWriter, value);
    await jsonWriter.FlushAsync();
}

app.Run();