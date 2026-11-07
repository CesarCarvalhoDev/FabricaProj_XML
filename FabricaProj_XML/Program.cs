using FabricaProj_XML.Repositories.Interfaces;
using FabricaProj_XML.Repositories.Implementations;
using FabricaProj_XML.Services.Interfaces;
using FabricaProj_XML.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Registro das dependências para Injeção de Dependência
builder.Services.AddScoped<IFileSystemXmlRepository, FileSystemXmlRepository>();
builder.Services.AddScoped<IXmlParseService, XmlParseService>();
builder.Services.AddScoped<INfceService, NfceService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
