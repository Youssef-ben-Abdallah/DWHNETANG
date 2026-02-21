using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetDwhProject.Core.Contracts;
using NetDwhProject.Infrastructure;
using NetDwhProject.Infrastructure.Repositories;
var b=WebApplication.CreateBuilder(args);
b.Services.AddControllers();
b.Services.AddDbContext<OltpDbContext>(o=>o.UseSqlServer(b.Configuration.GetConnectionString("OLTPConnection")));
b.Services.AddDbContext<DwDbContext>(o=>o.UseSqlServer(b.Configuration.GetConnectionString("DWHConnection")));
b.Services.AddScoped<IAuthService,AuthService>();
b.Services.AddScoped<ICategoryRepository,CategoryRepository>();b.Services.AddScoped<ISubCategoryRepository,SubCategoryRepository>();b.Services.AddScoped<IProductRepository,ProductRepository>();b.Services.AddScoped<IOrderRepository,OrderRepository>();b.Services.AddScoped<IAnalyticsRepository,AnalyticsRepository>();
b.Services.AddCors(o=>o.AddPolicy("ng",p=>p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
var key=Encoding.UTF8.GetBytes(b.Configuration["Jwt:Key"]!);b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>o.TokenValidationParameters=new TokenValidationParameters{ValidateAudience=false,ValidateIssuer=false,ValidateIssuerSigningKey=true,IssuerSigningKey=new SymmetricSecurityKey(key)});
b.Services.AddAuthorization();b.Services.AddEndpointsApiExplorer();b.Services.AddSwaggerGen(c=>{c.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme{Name="Authorization",In=ParameterLocation.Header,Type=SecuritySchemeType.Http,Scheme="bearer",BearerFormat="JWT"});c.AddSecurityRequirement(new OpenApiSecurityRequirement{{new OpenApiSecurityScheme{Reference=new OpenApiReference{Type=ReferenceType.SecurityScheme,Id="Bearer"}},Array.Empty<string>()}});});
var app=b.Build();if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseCors("ng");app.UseAuthentication();app.UseAuthorization();app.MapControllers();using(var s=app.Services.CreateScope()){var db=s.ServiceProvider.GetRequiredService<OltpDbContext>();db.Database.Migrate();await SeedData.SeedAsync(db);}app.Run();
