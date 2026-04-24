using BreadBank.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext <AppDbContext>();
builder.Services.AddScoped<Repository>();
builder.Services.AddScoped<Manager>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
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
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles(); // Позволяет открывать файлы из wwwroot

app.Run();
