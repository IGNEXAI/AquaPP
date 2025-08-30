using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using LiteDB;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Enrichers.CallerInfo;
using Serilog.Enrichers.CallStack;
using WaterUtilities.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddElsa(elsa =>
{
    // Configure Management layer to use EF Core.
    elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));

    // Configure Runtime layer to use EF Core.
    elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()));

    // Default Identity features for authentication/authorization.
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-signing-key"; // This key needs to be at least 256 bits long.
        identity.UseAdminUserProvider();
    });

    // Configure ASP.NET authentication/authorization.
    elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());

    // Expose Elsa API endpoints.
    elsa.UseWorkflowsApi();

    // Enable JavaScript workflow expressions.
    elsa.UseJavaScript();

    // Enable C# workflow expressions.
    elsa.UseCSharp();

    // Enable Liquid workflow expressions.
    elsa.UseLiquid();
     
    elsa.UseWorkflows();
    
    elsa.UseHttp();
    
    elsa.UseWebhooks(webhooks => webhooks.ConfigureSinks += options => 
        builder.Configuration.GetSection("Webhooks")
            .Bind(options)
    );

    // Use timer activities.
    elsa.UseScheduling();

    // Register custom activities from the application, if any.
    elsa.AddActivitiesFrom<Program>();

    // Register custom workflows from the application, if any.
    elsa.AddWorkflowsFrom<Program>();
});

// Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin() // For demo purposes only. Use a specific origin instead.
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("x-elsa-workflow-instance-id"))); // Required for Elsa Studio in order to support running workflows from the designer. Alternatively, you can use the `*` wildcard to expose all headers.

// Add Health Checks.
builder.Services.AddHealthChecks();

const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = Path.Join(path, "workflows-platform.db");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var liteDbPath = Path.Join(path, "");

builder.Services.AddSingleton<ILiteDatabase>(_ => new LiteDatabase(liteDbPath));

var logger = new LoggerConfiguration()
    .Enrich.WithCallerInfo(includeFileInfo:true, allowedAssemblies:  ["WaterUtilities"])
    .WriteTo.Console()
    .WriteTo.File("app-log.txt")
    .Enrich.FromLogContext()
    .Enrich.WithCallStack()
    .CreateLogger();

builder.Services.AddLogging(options =>
    options.AddSerilog(logger));

// Build the web application.
var app = builder.Build();

// Configure web application's middleware pipeline.
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseWorkflowsApi(); // Use Elsa API endpoints.
app.UseWorkflows();

app.Run();