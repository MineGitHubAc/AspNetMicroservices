using EventBus.Messages.Common;
using MassTransit;
using Ordering.Api.EventBusConsumer;
using Ordering.Application;
using Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<BasketCheckoutConsumer>();
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("EventBusSettings:HostAddress"));
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c => { c.ConfigureConsumer<BasketCheckoutConsumer>(ctx); });
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
//HostExtensions.MigrateDatabase<OrderContext>((context, services) => {
//    var logger = builder.Services.BuildServiceProvider().GetService<ILogger<OrderContextSeed>>();
//    OrderContextSeed.SeedAsync(new OrderContext(), logger).Wait();
//});
app.Run();
