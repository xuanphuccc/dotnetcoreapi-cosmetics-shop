using web_api_cosmetics_shop.Data;
using Microsoft.EntityFrameworkCore;
using web_api_cosmetics_shop.Services.PromotionService;
using web_api_cosmetics_shop.Services.CategoryService;
using web_api_cosmetics_shop.Services.ProductOptionService;
using web_api_cosmetics_shop.Services.ProductService;
using web_api_cosmetics_shop.Services.ShoppingCartService;
using web_api_cosmetics_shop.Services.ShippingMethodService;
using web_api_cosmetics_shop.Services.OrderStatusService;
using web_api_cosmetics_shop.Services.PaymentTypeService;

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

// Add Repository
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductOptionService, ProductOptionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IShippingMethodService, ShippingMethodService>();
builder.Services.AddScoped<IOrderStatusService, OrderStatusService>();
builder.Services.AddScoped<IPaymentTypeService, PaymentTypeService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
