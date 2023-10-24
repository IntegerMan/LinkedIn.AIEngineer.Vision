using System.Reflection;
using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;

namespace LinkedIn.AIEngineer.Vision;

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
        string endpoint = configurationRoot["VisionEndpoint"]!;

        VisionServiceOptions options = new(endpoint, new AzureKeyCredential(visionKey));

        Uri imageUrl = new ("https://matteland.dev/img/ProfileMattE.94cd6573.png");
        using VisionSource imageSource = VisionSource.FromUrl(imageUrl);

        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.Caption | ImageAnalysisFeature.Text,
            Language = "en",
            GenderNeutralCaption = false,
            SegmentationMode = ImageSegmentationMode.None,
            ModelName = string.Empty, // default model
            ModelVersion = "latest",
        };

        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);

        ImageAnalysisResult result = analyzer.Analyze();

        Console.WriteLine($"Caption: {result.Caption.Content} ({result.Caption.Confidence:P2})");
    }
}