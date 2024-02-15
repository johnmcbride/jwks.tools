using Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

//Add commands to the CommandApp
app.Configure(config => {
    config.AddCommand<BuildJWKSCommand>("buildjwks");
    config.AddCommand<BuildJWKCommand>("buildjwk");
    config.AddCommand<VerifyJWKSCommand>("verifyjwks");
});

return app.Run(args);
