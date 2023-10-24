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

        Uri imageUrl = new("https://matteland.dev/img/ProfileMattE.94cd6573.png");
        using VisionSource imageSource = VisionSource.FromUrl(imageUrl);

        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.Caption | 
                       ImageAnalysisFeature.Text | 
                       ImageAnalysisFeature.CropSuggestions | 
                       ImageAnalysisFeature.Tags | 
                       ImageAnalysisFeature.Objects | 
                       ImageAnalysisFeature.DenseCaptions | 
                       ImageAnalysisFeature.People,
            Language = "en",
            GenderNeutralCaption = false,
            SegmentationMode = ImageSegmentationMode.None,
            ModelName = string.Empty, // default model
            ModelVersion = "latest",
        };

        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);

        ImageAnalysisResult result = analyzer.Analyze();

        // High level status
        Console.WriteLine($"Result: {result.Reason} using model {result.ModelVersion}");
        if (result.ImageWidth.HasValue && result.ImageHeight.HasValue) {
            Console.WriteLine($"Image Dimensions: {result.ImageWidth} x {result.ImageHeight}px");
        }
        Console.WriteLine();

        // Captions
        Console.WriteLine($"Caption: {result.Caption.Content} ({result.Caption.Confidence:P2})");
        Console.WriteLine();

        // Dense Captions
        Console.WriteLine($"Detected {result.DenseCaptions.Count} dense caption(s)");
        foreach (ContentCaption denseCaption in result.DenseCaptions) {
            Console.WriteLine($"Dense Caption {denseCaption.Content} at {denseCaption.BoundingBox} (Confidence {denseCaption.Confidence:P2})");
        }
        Console.WriteLine();

        // Tags
        Console.WriteLine($"Detected {result.Tags.Count} tag(s)");
        foreach (ContentTag tag in result.Tags) {
            Console.WriteLine($"Tag {tag.Name} (Confidence {tag.Confidence:P2})");
        }
        Console.WriteLine();

        // Objects
        Console.WriteLine($"Detected {result.Objects.Count} object(s)");
        foreach (DetectedObject obj in result.Objects) {
            Console.WriteLine($"Object {obj.Name} at {obj.BoundingBox} (Confidence {obj.Confidence:P2})");
        }
        Console.WriteLine();

        // Person Detection
        Console.WriteLine($"Detected {result.People.Count} people");
        foreach (DetectedPerson person in result.People) {
            Console.WriteLine($"Person detected at {person.BoundingBox} (Confidence {person.Confidence:P2})");
        }
        Console.WriteLine();

        // Text Detection
        Console.WriteLine($"Text: {result.Text.Lines.Count} line(s) of text");
        foreach (DetectedTextLine line in result.Text.Lines) {
            Console.WriteLine("Line: '" + line.Content + "'");

            foreach (Point polygon in line.BoundingPolygon) {
                Console.WriteLine($"Line Bounding Box: {polygon}");
            }

            foreach (DetectedTextWord word in line.Words) {
                Console.WriteLine($"Word: {word.Content} (Confidence {word.Confidence:P2})");
                foreach (var polygon in word.BoundingPolygon) {
                    Console.WriteLine($"Word Bounding Box: {polygon}");
                }
            }
        }
        Console.WriteLine();

        // Crop Suggestions
        Console.WriteLine($"Crop Suggestions: {result.CropSuggestions.Count} suggestion(s)");
        foreach (var suggestion in result.CropSuggestions) {
            Console.WriteLine($"{suggestion.BoundingBox} with aspect ratio of {suggestion.AspectRatio}");
        }
        Console.WriteLine();
    }

    private static IConfiguration GetSettings() =>
        new ConfigurationBuilder()
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
}