using BetGuardianAPI.Data;
using BetGuardianAPI.Repositories;
using BetGuardianAPI.Services;
using BetGuardianAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuração do Entity Framework com SQLite
builder.Services.AddDbContext<BetGuardianContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                     "Data Source=BetGuardian.db"));

// Configuração do HttpClient para APIs externas
builder.Services.AddHttpClient();

// Configuração dos repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAtividadeAlternativaRepository, AtividadeAlternativaRepository>();

// Configuração dos serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IAtividadeAlternativaService, AtividadeAlternativaService>();
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configuração do JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "BetGuardian_SecretKey_2024_Sprint4_FIAP";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BetGuardianAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BetGuardianUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BetGuardian API",
        Version = "v1",
        Description = "API para sistema de apoio a pessoas com problemas de apostas compulsivas - Sprint 4 FIAP",
        Contact = new OpenApiContact
        {
            Name = "Equipe BetGuardian - FIAP",
            Email = "betguardian@fiap.com.br"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Configuração para autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configuração para mostrar enums como strings
    c.UseInlineDefinitionsForEnums();
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração para serialização JSON
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BetGuardian API v1");
        c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
        c.DocumentTitle = "BetGuardian API - Documentação";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Garantir que o banco de dados seja criado e populado
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BetGuardianContext>();
    
    // Aplicar migrations automaticamente em desenvolvimento
    if (app.Environment.IsDevelopment())
    {
        try
        {
            context.Database.Migrate();
        }
        catch
        {
            // Se não conseguir fazer migrate (ex: InMemory), usa EnsureCreated
            context.Database.EnsureCreated();
        }
    }
    else
    {
        context.Database.EnsureCreated();
    }
    
    // Popular o banco com dados de exemplo se estiver vazio
    await SeedDataAsync(context);
}

app.Run();

/// <summary>
/// Método para popular o banco de dados com dados de exemplo
/// </summary>
static async Task SeedDataAsync(BetGuardianContext context)
{
    // Verificar se já existem usuários
    if (!context.Usuarios.Any())
    {
        var usuarios = new[]
        {
            new Usuario
            {
                Nome = "João Silva",
                Email = "joao.silva@email.com",
                Idade = 28,
                NivelRisco = NivelRisco.Medio,
                TotalApostas = 25,
                ValorGasto = 750.50m,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            },
            new Usuario
            {
                Nome = "Maria Santos",
                Email = "maria.santos@email.com",
                Idade = 35,
                NivelRisco = NivelRisco.Alto,
                TotalApostas = 60,
                ValorGasto = 2500.00m,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            },
            new Usuario
            {
                Nome = "Pedro Oliveira",
                Email = "pedro.oliveira@email.com",
                Idade = 42,
                NivelRisco = NivelRisco.Critico,
                TotalApostas = 120,
                ValorGasto = 8000.00m,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            },
            new Usuario
            {
                Nome = "Ana Costa",
                Email = "ana.costa@email.com",
                Idade = 24,
                NivelRisco = NivelRisco.Baixo,
                TotalApostas = 5,
                ValorGasto = 150.00m,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            },
            new Usuario
            {
                Nome = "Carlos Ferreira",
                Email = "carlos.ferreira@email.com",
                Idade = 31,
                NivelRisco = NivelRisco.Medio,
                TotalApostas = 35,
                ValorGasto = 1200.75m,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
            }
        };

        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();

        // Criar alguns alertas de exemplo
        var alertas = new[]
        {
            new Alerta
            {
                UsuarioId = 1,
                Mensagem = "Seu nível de risco está moderado. Considere reduzir a frequência das apostas.",
                Tipo = TipoAlerta.Aviso,
                DataCriacao = DateTime.UtcNow.AddDays(-2)
            },
            new Alerta
            {
                UsuarioId = 2,
                Mensagem = "Nível de risco alto detectado. Recomenda-se buscar apoio profissional.",
                Tipo = TipoAlerta.Critico,
                DataCriacao = DateTime.UtcNow.AddDays(-1)
            },
            new Alerta
            {
                UsuarioId = 3,
                Mensagem = "Você é mais forte do que imagina. Busque ajuda profissional imediatamente.",
                Tipo = TipoAlerta.Motivacional,
                DataCriacao = DateTime.UtcNow.AddHours(-6)
            },
            new Alerta
            {
                UsuarioId = 4,
                Mensagem = "Parabéns! Você está mantendo um bom controle sobre suas atividades.",
                Tipo = TipoAlerta.Motivacional,
                DataCriacao = DateTime.UtcNow.AddDays(-3)
            },
            new Alerta
            {
                UsuarioId = 5,
                Mensagem = "Informação: Existem várias atividades alternativas disponíveis para você.",
                Tipo = TipoAlerta.Informativo,
                DataCriacao = DateTime.UtcNow.AddHours(-12)
            }
        };

        context.Alertas.AddRange(alertas);
        await context.SaveChangesAsync();
    }
}

// Expor a classe Program para testes
public partial class Program { }
