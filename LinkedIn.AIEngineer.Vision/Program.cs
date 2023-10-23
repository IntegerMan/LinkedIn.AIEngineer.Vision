using System.Reflection;
using Microsoft.Extensions.Configuration;

internal class Program {
    private static void Main() {

        // Read settings from Settings.json and environment variable
        IConfigurationRoot configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Get the computer vision setting from the configuration
        string visionKey = configurationRoot["VisionKey"]!;

        Console.WriteLine("Vision Key: {0}", visionKey);
    }
}