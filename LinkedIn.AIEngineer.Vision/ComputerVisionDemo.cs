using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn.AIEngineer.Vision; 

public class ComputerVisionDemo {

    public static void PerformImageAnalysis(VisionServiceOptions options, VisionSource imageSource, string imageFilePath) {
        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.Caption |
                       ImageAnalysisFeature.Text |
                       ImageAnalysisFeature.CropSuggestions |
                       ImageAnalysisFeature.Tags |
                       ImageAnalysisFeature.Objects |
                       ImageAnalysisFeature.DenseCaptions |
                       ImageAnalysisFeature.People,
            CroppingAspectRatios = new List<double>() { 0.75, 1.0, 1.5, 1.8},
            Language = "en",
            GenderNeutralCaption = false,
            SegmentationMode = ImageSegmentationMode.None,
            ModelName = string.Empty, // default model
            ModelVersion = "latest",
        };
        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);

        ImageAnalysisResult result = analyzer.Analyze();

        switch (result.Reason) {
            case ImageAnalysisResultReason.Error:
                ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
                break;

            case ImageAnalysisResultReason.Analyzed:
                DisplayAnalysisResults(result, imageFilePath);
                break;
        }
    }

    private static void DisplayAnalysisResults(ImageAnalysisResult result, string imageFilePath) {
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

            // Crop the image based on obj.BoundingBox
            using Bitmap image = new(imageFilePath);
            using Bitmap croppedImage = image.Clone(obj.BoundingBox, image.PixelFormat);

            // Save the cropped image
            string croppedImagePath = $"cropped-{obj.Name.Replace(" ", "")}.png";
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Console.WriteLine();

        // Person Detection
        Console.WriteLine($"Detected {result.People.Count} people");
        int index = 1;
        foreach (DetectedPerson person in result.People) {
            Console.WriteLine($"Person detected at {person.BoundingBox} (Confidence {person.Confidence:P2})");

            // Crop the image based on person.BoundingBox
            using Bitmap image = new(imageFilePath);
            using Bitmap croppedImage = image.Clone(person.BoundingBox, image.PixelFormat);

            // Save the cropped image
            string croppedImagePath = $"person-{index++}.png";
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Console.WriteLine();

        // Text Detection
        Console.WriteLine($"Text: {result.Text.Lines.Count} line(s) of text");
        index = 1;
        foreach (DetectedTextLine line in result.Text.Lines) {
            Console.WriteLine("Line: '" + line.Content + "'");


            foreach (Point polygon in line.BoundingPolygon) {
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

            /*
            foreach (DetectedTextWord word in line.Words) {
                Console.WriteLine($"Word: {word.Content} (Confidence {word.Confidence:P2})");
                foreach (var polygon in word.BoundingPolygon) {
                    Console.WriteLine($"Word Bounding Box: {polygon}");
                }
            }
            */
        }
        Console.WriteLine();

        // Crop Suggestions
        index = 1;
        Console.WriteLine($"Crop Suggestions: {result.CropSuggestions.Count} suggestion(s)");
        foreach (var suggestion in result.CropSuggestions) {
            Console.WriteLine($"{suggestion.BoundingBox} with aspect ratio of {suggestion.AspectRatio}");

            // Crop the image based on suggestion.BoundingBox
            using Bitmap image = new(imageFilePath);
            using Bitmap croppedImage = image.Clone(suggestion.BoundingBox, image.PixelFormat);

            // Save the cropped image
            string croppedImagePath = $"crop-{index++}.png";
            croppedImage.Save(croppedImagePath, System.Drawing.Imaging.ImageFormat.Png);
        }
        Console.WriteLine();
    }

}
