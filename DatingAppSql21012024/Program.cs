//using DatingAppSql21012024.Data;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Extensions;
//using DatingAppSql21012024.Interfaces;
using DatingAppSql21012024.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddAplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);





// p'q funcione Identity
builder.Services.AddScoped<IUserStore<AppUser>, AppUserStore>();
builder.Services.AddIdentityCore<AppUser>(opciones =>
{
    opciones.Password.RequireDigit = false;
    opciones.Password.RequireLowercase = false;
    opciones.Password.RequireUppercase = false;
    opciones.Password.RequireNonAlphanumeric = false;
})/*.AddErrorDescriber<MensajesDeErrorIdentity>()*/;




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));




app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();

app.Run();
