using College.Configurations;
using College.Data;
using College.Data.Repository;
using CollegeApp.MyLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
builder.Services.AddSwaggerGen();

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
        context => context.Response.WriteAsync(builder.Configuration.GetValue<string>("JWTSecret")));

});

app.Run();
