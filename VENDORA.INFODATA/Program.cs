using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VENDORA.INFODATA.Data;
using VENDORA.INFODATA.Hubs;
using VENDORA.INFODATA.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


// 🔥 1. Tambahkan Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 2. Registrasi Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔥 3. Tambahkan Layanan SignalR
builder.Services.AddSignalR();

// 🔥 4. Tambahkan Layanan Autentikasi (JWT)
builder.Services.AddScoped<AuthService>();

// 🔐 Konfigurasi JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// 🌍 5. Konfigurasi CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://fresh-kristan-vendora-d56b0214.koyeb.app") // 🛑 Hanya izinkan frontend ini
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials(); // ✅ Harus ditambahkan agar `withCredentials: true` JWT di frontend bisa berjalan
                      });
});

var app = builder.Build();

// ✅ Konfigurasi Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);  // 🔥 Harus dipanggil sebelum endpoint
// app.UseHttpsRedirection();
app.UseAuthentication();   // 🔐 Pastikan ini sebelum Authorization
app.UseAuthorization();

app.UseExceptionHandler("/error");

app.UseEndpoints(endpoints => 
{
    endpoints.MapControllers(); // ✅ Ini WAJIB ada!
    endpoints.MapHub<ChatHub>("/hubs/chat");
    endpoints.MapHub<NotificationHub>("/hubs/notifications");
});

app.MapGet("/", () => "API is running!");

app.Run();
