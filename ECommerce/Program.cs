using Common.Entities;
using Ecommerce.BL;
using Ecommerce.DL;
using ECommerce.BL;
using ECommerce.Common.Entities;
using ECommerce.Common.Extension;
using ECommerce.DL;
using Microsoft.AspNetCore.Builder;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped(typeof(IBaseDL), typeof(BaseDL));
builder.Services.AddScoped(typeof(IBaseBL), typeof(BaseBL));
builder.Services.AddScoped<ITestDL, TestDL>();
builder.Services.AddScoped<ITestBL, TestBL>();
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

builder.Services.AddControllers().AddBaseControllerConfig();

//if (!builder.Environment.IsDevelopment())
//{
//    builder.Services.AddHttpsRedirection(options =>
//    {
//        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
//        options.HttpsPort = 443;
//    });
//}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapGraphQL();

app.Run();
