using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Presentation.Data;
using Presentation.Interfaces;
using Presentation.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AccountSqlDatabase")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection();
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
