using Azure;
using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn.AIEngineer.Vision; 

public class SegmentationDemo {

    public static void PerformBackgroundRemoval(string endpoint, string visionKey, string imageFilePath) {
        AzureKeyCredential keyCredential = new(visionKey);
        VisionServiceOptions options = new(endpoint, keyCredential);

        using VisionSource imageSource = VisionSource.FromFile(imageFilePath);

        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.None,
            SegmentationMode = ImageSegmentationMode.ForegroundMatting,
        };
        using ImageAnalyzer analyzer = new(options, imageSource, analysisOptions);

        ImageAnalysisResult result = analyzer.Analyze();

        if (result.Reason == ImageAnalysisResultReason.Error)
        {
            ImageAnalysisErrorDetails errorDetails = ImageAnalysisErrorDetails.FromResult(result);
            Console.WriteLine($"Could not analyze image - Code {errorDetails.ErrorCode}: {errorDetails.Message}");
            return;
        }

        SegmentationResult segment = result.SegmentationResult;
        Console.WriteLine($"Analyzed image with a {segment.ImageWidth}x{segment.ImageHeight}px result");
        File.WriteAllBytes("Foreground.png", segment.ImageBuffer.ToArray());
    }

    public static void PerformForegroundMatting(VisionServiceOptions options, VisionSource imageSource) {
        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.None,
            Language = "en",
            GenderNeutralCaption = false,
            SegmentationMode = ImageSegmentationMode.ForegroundMatting,
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
                Console.WriteLine($"Analyzed image with a {result.SegmentationResult.ImageWidth}x{result.SegmentationResult.ImageHeight}px result");
                File.WriteAllBytes("ForegroundMatte.png", result.SegmentationResult.ImageBuffer.ToArray());
                break;
        }
    }
}
