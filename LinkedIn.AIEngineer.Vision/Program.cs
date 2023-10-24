using System.Drawing;
using System.Reflection;
using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.Extensions.Configuration;

namespace LinkedIn.AIEngineer.Vision;

internal class Program {

    private static void Main() {
        IConfiguration configuration = GetSettings();

        // Get the computer vision setting from the configuration
        string visionKey = configuration["VisionKey"]!;
        string endpoint = configuration["VisionEndpoint"]!;

        VisionServiceOptions options = new(endpoint, new AzureKeyCredential(visionKey));

        string localImage = "C:\\Dev\\AccessibleAIBlog\\YTThumbs\\YT_PredictingHockeyPenalties.png";
        Uri imageUrl = new("https://matteland.dev/img/ProfileMattE.94cd6573.png");
        using VisionSource imageSource = VisionSource.FromFile(localImage); // VisionSource.FromUrl(imageUrl);

        ComputerVisionDemo.PerformImageAnalysis(options, imageSource);
        //SegmentationDemo.PerformBackgroundRemoval(options, imageSource);
        //SegmentationDemo.PerformForegroundMatting(options, imageSource);
    }

    private static IConfiguration GetSettings() =>
        new ConfigurationBuilder()
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
}