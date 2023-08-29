using Common.Entities;
using Ecommerce.DL;
using Ecommerce.Types;
using ECommerce.BL;
using ECommerce.DL;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped(typeof(IBaseDL), typeof(BaseDL));
builder.Services.AddScoped(typeof(IBaseBL), typeof(BaseBL));
builder.Services.AddScoped<IUserDL, UserDL>();
builder.Services.AddScoped<IUserBL, UserBL>();
//builder.Services.AddSingleton<BaseEntityQuery>();
//builder.Services.AddSingleton<BaseEntityType>();
//builder.Services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
//var sp = builder.Services.BuildServiceProvider();
//builder.Services.AddSingleton<ISchema>(new EcommerceSchema(new FuncDependencyResolver(type => sp.GetService(type))));
// Add services to the container.

DatabaseContext.ConnectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddGraphQLServer()
//    .AddQueryType<Query>().AddType<UserType>().AddType<RoleType>();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

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

//app.MapGraphQL();

app.Run();
