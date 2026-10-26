using VotePreferentiel;
using VotePreferentiel.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<VoteProvider>();
builder.Services.AddSingleton<IVoteCounter, VoteCounter>();   // NEW
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();