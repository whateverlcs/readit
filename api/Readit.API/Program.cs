using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Readit.API.Application.UseCases.AvaliacaoObra.Consultar;
using Readit.API.Application.UseCases.AvaliacaoObra.Editar;
using Readit.API.Application.UseCases.Login.FazerLogin;
using Readit.API.Application.UseCases.Registro;
using Readit.API.Filters;
using Readit.Core.Repositories;
using Readit.Core.Security.Cryptography;
using Readit.Core.Security.Tokens.Access;
using Readit.Core.Services;
using Readit.Data.Context;
using Readit.Data.Repositories;
using Readit.Data.Services;
using Readit.Infra.Logging;
using Readit.Infra.Security.Cryptography;
using Readit.Infra.Security.Tokens.Access;
using Readit.Infra.Services;
using System.Text;

const string AUTHENTICATION_TYPE = "Bearer";

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddSingleton<IConfiguration>(configuration);

builder.Services.AddScoped<IConfiguracaoService, ConfiguracaoService>();

builder.Services.AddDbContextFactory<ReaditContext>(options =>
{
    var connectionString = configuration.GetConnectionString("ConnectionString");
    options.UseSqlServer(connectionString);
});

//services
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IBcryptAlgorithm, BcryptAlgorithm>();
builder.Services.AddScoped<ILoggingService, SerilogLogger>();
builder.Services.AddScoped<IArquivoService, ArquivoService>();
builder.Services.AddScoped<ICapituloService, CapituloService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IEnumService, EnumService>();
builder.Services.AddScoped<IImagemService, ImagemService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUtilService, UtilService>();

//repositories
builder.Services.AddScoped<IAvaliacaoObraRepository, AvaliacaoObraRepository>();
builder.Services.AddScoped<IBookmarkRepository, BookmarkRepository>();
builder.Services.AddScoped<ICapituloRepository, CapituloRepository>();
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IGeneroRepository, GeneroRepository>();
builder.Services.AddScoped<IImagemRepository, ImagemRepository>();
builder.Services.AddScoped<IObraRepository, ObraRepository>();
builder.Services.AddScoped<IPaginaCapituloRepository, PaginaCapituloRepository>();
builder.Services.AddScoped<IPreferenciasRepository, PreferenciasRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVisualizacaoObraRepository, VisualizacaoObraRepository>();

//useCases
builder.Services.AddScoped<RegistroUsuarioUseCase>();
builder.Services.AddScoped<RealizarLoginUseCase>();
builder.Services.AddScoped<ConsultarRatingUseCase>();
builder.Services.AddScoped<EditarRatingUseCase>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(AUTHENTICATION_TYPE, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below;
                      Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = AUTHENTICATION_TYPE
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = AUTHENTICATION_TYPE
                },
                Scheme = "oauth2",
                Name = AUTHENTICATION_TYPE,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = configuration["Jwt:Key"];
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = symmetricKey
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();