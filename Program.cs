using web_api_cosmetics_shop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using web_api_cosmetics_shop.Services.PromotionService;
using web_api_cosmetics_shop.Services.CategoryService;
using web_api_cosmetics_shop.Services.ProductOptionService;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ShoppingCartService;
using web_api_cosmetics_shop.Services.ShippingMethodService;
using web_api_cosmetics_shop.Services.OrderStatusService;
using web_api_cosmetics_shop.Services.PaymentTypeService;
using web_api_cosmetics_shop.Services.PaymentMethodService;
using web_api_cosmetics_shop.Services.AddressService;
using web_api_cosmetics_shop.Services.WishlistService;
using web_api_cosmetics_shop.Services.ShopOrderService;
using web_api_cosmetics_shop.Services.ProviderService;
using System.Text;
using web_api_cosmetics_shop.Services.AdminService;
using web_api_cosmetics_shop.Services.UserService;
using web_api_cosmetics_shop.Services.RoleService;
using web_api_cosmetics_shop.Services.ReportService;
using web_api_cosmetics_shop.Services.UserReviewService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext Service
builder.Services.AddDbContext<CosmeticsShopContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("CosmeticsShop");
    options.UseSqlServer(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add Repository
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductOptionService, ProductOptionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IShippingMethodService, ShippingMethodService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<IPaymentTypeService, PaymentTypeService>();
builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IShopOrderService, ShopOrderService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IUserReviewService, UserReviewService>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
