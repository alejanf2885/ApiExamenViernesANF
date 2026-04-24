using ApiExamenViernesANF.Helpers;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<FileStorageHelper>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("*")
    );
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet(
    "/",
    context =>
    {
        context.Response.Redirect("/scalar");
        return Task.CompletedTask;
    }
);

app.UseHttpsRedirection();
app.UseCors("AllowAll");

string imagesPath = Path.Combine(builder.Environment.ContentRootPath, "Images");
Directory.CreateDirectory(imagesPath);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imagesPath),
        RequestPath = "/Images",
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Methods", "*");
        },
    }
);

app.MapControllers();
app.Run();
