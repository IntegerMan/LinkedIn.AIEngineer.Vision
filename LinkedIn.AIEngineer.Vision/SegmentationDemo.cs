using Azure.AI.Vision.Common;
using Azure.AI.Vision.ImageAnalysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedIn.AIEngineer.Vision; 

public class SegmentationDemo {

    public static void PerformBackgroundRemoval(VisionServiceOptions options, VisionSource imageSource) {
        ImageAnalysisOptions analysisOptions = new() {
            Features = ImageAnalysisFeature.None,
            Language = "en",
            GenderNeutralCaption = false,
            SegmentationMode = ImageSegmentationMode.BackgroundRemoval,
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
                File.WriteAllBytes("BackgroundRemoval.png", result.SegmentationResult.ImageBuffer.ToArray());
                break;
        }
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
