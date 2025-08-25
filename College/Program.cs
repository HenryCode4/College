using College.Configurations;
using College.Data;
using College.Data.Repository;
using CollegeApp.MyLogging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddLog4Net();
// Add services to the container.

//#region Serilog Settings
Log.Logger = new LoggerConfiguration().
    MinimumLevel.Debug()
    .WriteTo.File("Log/log.txt",
    rollingInterval: RollingInterval.Minute)
    .CreateLogger();

//use this line to override the built-in loggers
//builder.Host.UseSerilog();

//Use serilog alogn with built-in loggers
builder.Logging.AddSerilog();
//#endregion

builder.Services.AddDbContext<CollegeDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeAppDBConnection"));
});

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the bearer scheme. Enter Bearer [space] add your token in the text input. Example: Bearer swersdf877sdf",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Scheme = "oauth2",

                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddScoped<IMyLogger, LogToFile>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //Allow all origins, all methods, all headers
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    //options.AddPolicy("AllowALl", policy =>
    //{
    //    //Allow all origins, all methods, all headers
    //    policy.AllowAnyOrigin()
    //           .AllowAnyMethod()
    //           .AllowAnyHeader();
    //});

    options.AddPolicy("AllowOnlyLocalhost", policy =>
    {
        //Allow specific origin
        policy.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowOnlyGoogle", policy =>
    {
        //Allow specific origin
        policy.WithOrigins("https://google.com", "https://gmail.com", "https://drive.google.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowOnlyMicrosoft", policy =>
    {
        //Allow specific origin
        policy.WithOrigins("https://outlook.com", "https://microsoft.com", "https://onedrive.google.com")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });



});
var keyForGoogle = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretforGoogle"));
var keyForMicrosoft = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretforMicrosoft"));
var keyForLocal = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSecretforLocal"));
string GoogleAudience = builder.Configuration.GetValue<string>("GoogleAudience");
string MicrosoftAudience = builder.Configuration.GetValue<string>("MicrosoftAudience");
string LocalAudience = builder.Configuration.GetValue<string>("LocalAudience");
string GoogleIssuer = builder.Configuration.GetValue<string>("GoogleIssuer");
string MicrosoftIssuer = builder.Configuration.GetValue<string>("MicrosoftIssuer");
string LocalIssuer = builder.Configuration.GetValue<string>("LocalIssuer");

//JWT Authentication Configuration 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("LoginForGoogleUSers", options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyForGoogle),
        ValidateIssuer = true,
        ValidIssuer = GoogleIssuer,

        ValidateAudience = true,
        ValidAudience = GoogleAudience
    };
}).AddJwtBearer("LoginForMocrosoftUSers", options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyForMicrosoft),

        ValidateIssuer = true,
        ValidIssuer = MicrosoftIssuer,

        ValidateAudience = true,
        ValidAudience = MicrosoftAudience
    };
}).AddJwtBearer("LoginForLocalUSers", options =>
{
    //options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyForLocal),
        ValidateIssuer = true,
        ValidIssuer = LocalIssuer,

        ValidateAudience = true,
        ValidAudience = LocalAudience
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("api/testingendpoint",
        context => context.Response.WriteAsync("Test Response"))
        .RequireCors("AllowOnlyLocalhost");

    endpoints.MapControllers()
             .RequireCors("AllowAll");

    endpoints.MapGet("api/testendpoint2",
        context => context.Response.WriteAsync(builder.Configuration.GetValue<string>("JWTSecretforLocal")));

});

app.Run();
