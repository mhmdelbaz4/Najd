using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NajdAPI.Data;
using NajdAPI.Enums;
using NajdAPI.IRepos;
using NajdAPI.Repos;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NajdDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("myPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Bearer-Token");
    });
});

builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped<UsersRepo>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication("generalSchema")
                .AddJwtBearer("generalSchema" ,options =>
                                {
                                    options.TokenValidationParameters = new TokenValidationParameters()
                                    {
                                        ValidateIssuer = false,
                                        ValidateAudience = false,
                                        ValidateLifetime = true,
                                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecurityKey-general")!))
                                    };
                                });

builder.Services.AddAuthentication("customSchema")
                .AddJwtBearer("customSchema", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SecurityKey-custom")!))
                    };
                });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policy.GeneralPolicy.ToString(), policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("generalSchema"));

    options.AddPolicy(Policy.CustomPolicy.ToString(), policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("customSchema"));
});
builder.Services.AddSwaggerGen();          // Configures Swagger

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("myPolicy");

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
