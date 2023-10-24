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

    public static void PerformImageAnalysis(VisionServiceOptions options, VisionSource imageSource) {
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

        switch (result.Reason) {
            case ImageAnalysisResultReason.Error:
                ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
                Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
                break;

            case ImageAnalysisResultReason.Analyzed:
                DisplayAnalysisResults(result);
                break;
        }
    }

    private static void DisplayAnalysisResults(ImageAnalysisResult result) {
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
        Console.WriteLine($"Crop Suggestions: {result.CropSuggestions.Count} suggestion(s)");
        foreach (var suggestion in result.CropSuggestions) {
            Console.WriteLine($"{suggestion.BoundingBox} with aspect ratio of {suggestion.AspectRatio}");
        }
        Console.WriteLine();
    }

}
