
using LXP.Api.Interceptors;
using OfficeOpenXml;
using LXP.Data.Repository;
using LXP.Data.IRepository;

using LXP.Core.Services;
using LXP.Core.IServices;

using Serilog;
using LXP.Data.DBContexts;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using LXP.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the IQuizService and QuizService
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
// Register the IQuizRepository and QuizRepository
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizQuestionService, QuizQuestionService>();
builder.Services.AddScoped<IQuizQuestionRepository, QuizQuestionRepository>();
builder.Services.AddScoped<IBulkQuestionRepository, BulkQuestionRepository>();
builder.Services.AddScoped<IBulkQuestionService, BulkQuestionService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<LXPDbContext>(options => options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version())));
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();
