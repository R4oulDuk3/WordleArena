using TypeGen.Core.Generator;
using WordleArena.Api.Hubs.Messages.ClientToServer;

var options = new GeneratorOptions { BaseOutputDirectory = @"..\WebApp\src\lib\generate" }; // create the options object
var generator = new Generator(options); // create the generator instance
var assembly = typeof(GetUsernameResponse).Assembly;
generator.Generate(assembly); // generates the files