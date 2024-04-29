using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Agregado por Jhon
builder.Services.AddDistributedMemoryCache(); //paraa agregar en memoria el producto selecionado
builder.Services.AddSession(options =>  //Config JHON
{
    options.IdleTimeout = TimeSpan.FromHours(1);//parte de mis config JHON (Limite de tiempo )
}
    ); //El cierre

var app = builder.Build();

app.UseSession(); //config Jhon

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
