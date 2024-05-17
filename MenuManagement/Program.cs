using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MenuManagement.Data;
using System.Reflection;
using System.Text;
using MenuManagement.Controllers;
using MenuManagement.RabbitMQS;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Remove the duplicate line below
// builder.Services.AddControllersWithViews();
builder.Services.AddCors();
builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSingleton<IRabbitMQ, RabbitMQUnti>();
builder.Services.AddHostedService<RabbitMqService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://dev-feeu3ze3mjv64zbn.eu.auth0.com/";
    options.Audience = "https://www.eaau2024.com";
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// This approach should not be used in production. See https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
    db.Database.EnsureCreated();
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwagger();
// app.UseSwaggerUI(c =>
// {
//     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1");
//     c.RoutePrefix = "/grp-13/webmenuapi/StudyHealthAPI/swagger";
// });

// app.UsePathBase("/grp-13/webmenuapi");
app.UseSwagger();
app.UseSwaggerUI();

// app.MapGet("/", async context =>
// {
//     context.Response.Redirect("/grp-13/webmenuapi/swagger/index.html");
// });

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
