var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//try creating db conn here


builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder.WithOrigins("http://weather-report-system.s3-website.us-east-2.amazonaws.com") // or the address where your Angular app is running http://localhost:4200 DB Might be on localhost now - change in secrets. 
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
