using AutoMapper;
using NoSQLSkiServiceManager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NoSQLSkiServiceManager.DTOs.Request;
using NoSQLSkiServiceManager.DTOs.Requests;
using NoSQLSkiServiceManager.DTOs.Response;
using NoSQLSkiServiceManager.Middlewares;
using NoSQLSkiServiceManager.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(new MongoClient(builder.Configuration.GetConnectionString("MongoDbConnection")));

builder.Services.AddSingleton<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("JetStreamAPI");
});

builder.Services.AddSingleton(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    return new GenericService<Employee, EmployeeCreateDto, EmployeeUpdateDto, EmployeeResponseDto>(database, mapper, "EmployeeCollectionName");
});

builder.Services.AddSingleton(serviceProvider =>
{
    var database = serviceProvider.GetRequiredService<IMongoDatabase>();
    var mapper = serviceProvider.GetRequiredService<IMapper>();
    return new GenericService<ServiceOrder, CreateServiceOrderRequestDto, UpdateServiceOrderRequestDto, OrderResponseDto>(database, mapper, "ServiceOrders");
});


var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Serilog-Konfiguration
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// AutoMapper-Konfiguration
builder.Services.AddAutoMapper(typeof(MappingCode));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

// Inject the TokenService
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<ServiceOrderService>();


// Swagger, MVC und andere Dienste
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JetStreamBackend Arda Dursun", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Enter your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();



// HTTP-Anfrage-Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();