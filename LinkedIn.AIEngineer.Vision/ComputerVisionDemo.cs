using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System.Drawing;

namespace LinkedIn.AIEngineer.Vision;

public class ComputerVisionDemo
{
    public static void PerformImageAnalysisOCR(string endpoint, string visionKey, string imageFilePath)
    {
        AzureKeyCredential keyCredential = new(visionKey);
        VisionServiceOptions options = new(endpoint, keyCredential);

        using VisionSource imageSource = VisionSource.FromFile(imageFilePath);

        ImageAnalysisOptions analysisOptions = new()
        {
            Features = ImageAnalysisFeature.Text |
                       ImageAnalysisFeature.Tags,
        };

        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);
        ImageAnalysisResult result = analyzer.Analyze();

        if (result.Reason == ImageAnalysisResultReason.Error)
        {
            ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
            Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
            return;
        }

        // Read Text
        foreach (DetectedTextLine line in result.Text.Lines)
        {
            // Console.WriteLine(line.Content);
            
            foreach (DetectedTextWord word in line.Words)
            {
                Console.Write($"{word.Content} ({word.Confidence:P2}) ");
            }
            Console.WriteLine();
        }
    }

    public static void PerformImageAnalysisCropping(string endpoint, string visionKey, string imageFilePath)
    {
        AzureKeyCredential keyCredential = new(visionKey);
        VisionServiceOptions options = new(endpoint, keyCredential);

        using VisionSource imageSource = VisionSource.FromFile(imageFilePath);

        ImageAnalysisOptions analysisOptions = new()
        {
            Features = ImageAnalysisFeature.CropSuggestions,
            CroppingAspectRatios = new List<double>() { 0.75, 1, 1.8 },
        };

        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);
        ImageAnalysisResult result = analyzer.Analyze();

        if (result.Reason == ImageAnalysisResultReason.Error)
        {
            ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
            Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
            return;
        }

        // Crop Suggestions
        foreach (CropSuggestion suggestion in result.CropSuggestions)
        {
            Console.WriteLine($"{suggestion.BoundingBox} with aspect ratio of {suggestion.AspectRatio}");

            // Save the cropped image
            string croppedImagePath = $"cropped_{suggestion.AspectRatio}.png";
            using Bitmap image = new(imageFilePath);
            Rectangle rect = new(suggestion.BoundingBox.Left, suggestion.BoundingBox.Top, suggestion.BoundingBox.Width, suggestion.BoundingBox.Height);
            using Bitmap croppedImage = image.Clone(rect, image.PixelFormat);
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }

    public static void PerformImageAnalysis(string endpoint, string visionKey, string imageFilePath)
    {
        AzureKeyCredential keyCredential = new(visionKey);
        VisionServiceOptions options = new(endpoint, keyCredential);

        using VisionSource imageSource = VisionSource.FromFile(imageFilePath);

        ImageAnalysisOptions analysisOptions = new()
        {
            Features = ImageAnalysisFeature.Caption |
                       ImageAnalysisFeature.Tags |
                       ImageAnalysisFeature.Objects |
                       ImageAnalysisFeature.DenseCaptions |
                       ImageAnalysisFeature.People,
            CroppingAspectRatios = new List<double>() { 0.75, 1.0, 1.5, 1.8 },
            Language = "en",
            GenderNeutralCaption = false,
        };

        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);
        ImageAnalysisResult result = analyzer.Analyze();

        if (result.Reason == ImageAnalysisResultReason.Error)
        {
            ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
            Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
            return;
        }

        // Captions
        Console.WriteLine($"Caption: {result.Caption.Content} ({result.Caption.Confidence:P2})");

        // Tags
        foreach (ContentTag tag in result.Tags)
        {
            Console.WriteLine($"Tag {tag.Name} (Confidence {tag.Confidence:P2})");
        }

        // Objects
        foreach (DetectedObject obj in result.Objects)
        {
            Console.WriteLine($"Object {obj.Name} at {obj.BoundingBox} (Confidence {obj.Confidence:P2})");
        }

        // Person Detection
        foreach (DetectedPerson person in result.People)
        {
            Console.WriteLine($"Person detected at {person.BoundingBox} (Confidence {person.Confidence:P2})");
        }

        // Dense Captions
        foreach (ContentCaption denseCaption in result.DenseCaptions)
        {
            Console.WriteLine($"Dense Caption {denseCaption.Content} at {denseCaption.BoundingBox} (Confidence {denseCaption.Confidence:P2})");
        }
    }

    private static void DisplayAnalysisResults(ImageAnalysisResult result, string imageFilePath)
    {
        // High level status
        Console.WriteLine($"Result: {result.Reason} using model {result.ModelVersion}");
        if (result.ImageWidth.HasValue && result.ImageHeight.HasValue)
        {
            Console.WriteLine($"Image Dimensions: {result.ImageWidth} x {result.ImageHeight}px");
        }
        Console.WriteLine();

        // Captions
        Console.WriteLine($"Caption: {result.Caption.Content} ({result.Caption.Confidence:P2})");
        Console.WriteLine();

        // Dense Captions
        Console.WriteLine($"Detected {result.DenseCaptions.Count} dense caption(s)");
        foreach (ContentCaption denseCaption in result.DenseCaptions)
        {
            Console.WriteLine($"Dense Caption {denseCaption.Content} at {denseCaption.BoundingBox} (Confidence {denseCaption.Confidence:P2})");
        }
        Console.WriteLine();

        // Tags
        Console.WriteLine($"Detected {result.Tags.Count} tag(s)");
        foreach (ContentTag tag in result.Tags)
        {
            Console.WriteLine($"Tag {tag.Name} (Confidence {tag.Confidence:P2})");
        }
        Console.WriteLine();

        // Objects
        Console.WriteLine($"Detected {result.Objects.Count} object(s)");
        foreach (DetectedObject obj in result.Objects)
        {
            Console.WriteLine($"Object {obj.Name} at {obj.BoundingBox} (Confidence {obj.Confidence:P2})");
        }
        Console.WriteLine();

        // Person Detection
        Console.WriteLine($"Detected {result.People.Count} people");
        int index = 1;
        foreach (DetectedPerson person in result.People)
        {
            Console.WriteLine($"Person detected at {person.BoundingBox} (Confidence {person.Confidence:P2})");
        }
        Console.WriteLine();

        /*
        // Text Detection
        Console.WriteLine($"Text: {result.Text.Lines.Count} line(s) of text");
        index = 1;
        foreach (DetectedTextLine line in result.Text.Lines)
        {
            Console.WriteLine("Line: '" + line.Content + "'");

            foreach (Point polygon in line.BoundingPolygon)
            {
                Console.WriteLine($"Line Bounding Polygon: {polygon}");
            }

            int minX = line.BoundingPolygon.Select(p => p.X).Min();
            int maxX = line.BoundingPolygon.Select(p => p.X).Max();
            int minY = line.BoundingPolygon.Select(p => p.Y).Min();
            int maxY = line.BoundingPolygon.Select(p => p.Y).Max();

            // Crop the image based on the bounding polygon for the line
            Rectangle rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            using Bitmap image = new(imageFilePath);
            using Bitmap croppedImage = image.Clone(rect, image.PixelFormat);

            // Save the cropped image
            string croppedImagePath = $"line-{index++}.png";
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Console.WriteLine();

        // Crop Suggestions
        index = 1;
        Console.WriteLine($"Crop Suggestions: {result.CropSuggestions.Count} suggestion(s)");
        foreach (var suggestion in result.CropSuggestions)
        {
            Console.WriteLine($"{suggestion.BoundingBox} with aspect ratio of {suggestion.AspectRatio}");

            // Save the cropped image
            string croppedImagePath = $"crop-{index++}.png";
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Console.WriteLine();
        */
    }

}
