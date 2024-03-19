using DotNetAuth.Data;
using DotNetAuth.Hub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(10);

});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddAuthorization();
// builder.Services.AddAuthorizationBuilder()
//     .AddPolicy("api", p =>
// {
//     p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
//     p.RequireAuthenticatedUser()
//         .Build();
// });


// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set appropriate expiration time
        options.SlidingExpiration = true; // Extend expiration on activity
        // options.LoginPath = "/Account/Login"; // Set login page URL
        // options.LogoutPath = "/Account/Logout"; // Set logout page URL
    });
//
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowSpecificOrigin",
//         builder =>
//         {
//             builder.WithOrigins("*")
//                 .AllowAnyHeader()
//                 .AllowAnyMethod()
//                 .AllowCredentials(); // Allow credentials
//         });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:5173")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                ;
        });
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // // Password settings.
    // options.Password.RequireDigit = true;
    // options.Password.RequireLowercase = true;
    // options.Password.RequireNonAlphanumeric = true;
    // options.Password.RequireUppercase = true;
    // options.Password.RequiredLength = 6;
    // options.Password.RequiredUniqueChars = 1;
    //

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    // options.User.
});

builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<IdentityUser>();
app.MapHub<Gamehub>("/Hub");

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
