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


        string localImage = "LabCoat.jpg";
        //string imageUrl = "https://lh3.googleusercontent.com/pw/ADCreHfzQBYRsHjh9QGxwkXQRgyDRTfwO7tk0f01c-RShSpWDW3HUtG3kcnMgfrCnccdlj05vKH4j2ooffbH63i-ps5Z9LYkdrgi0coSyrwbZkTzCXZ0AxFPkbaYYhQ0KIYQrrqHw4ql4qhQ-eWOmKBfxHQggY8MZAG5PGD1j91T8AsrPeNZfzMAPW9fZU0-9KZsEj7blnwG1-i21MQc1KpPdiPtCyzXHAKjIEUC43DaYjiFVEHSilCH-MkA0Mpkmu_-BHSdbzhhSr7adN9HefjzN6s-9zbumbM4ULjviugEve56QUNd3m-BypAd0B3I26JkrALtcccrICR60CyRCewMqvczSvlYvqLBYoVScRoB-fyuaKyGy8ByKPukH76v1r6arrxa7TdjcN1FXwKwWkanIvZ98YjPMnhsoxF1dGeCMvyb18P_ZRbQtNtf6J2-53anBZueriSMRIfNkwCukLNY68jYV7sSYhKrwJwgIZoxCrlYnXTDAQoGujkYfij7W85yEUQH-c2HLDMCN-Uwc4BaQ-pMVz-uO--GjsBtm90Bifa2myIvakH4U3w-g52pv-0D50GypF8cKH-52ZZkRyxVDwSlimBdtKHdViVlfHYLcxq7V3BCEvLvfn4FpIZXEJ-AUsvbnjVR86E6oXyzjZQV4uutdYumlg3ecQ3itKKtlY5QzHr7n9Rc1U_FwOLa3E8arrCeyA9fDFxpxj9f4G_R0M_nxtmFn_gVaJI2WmSW_KiYG8QWKNqMKTypTSOIWeICSTZHygdENpKnCqga1-cxV0Jou0Qq3NcLuRW5NAu-szaC1-vhEtnhHrIGg84GcK_FTfVIVfz5obWbT_RBb4XT2milGGJobT3gzY0HK4UjzoWDzz7JSS0HmFay7zKvkFC8w4dmLVz9dMR4Ga2_4edY4F-sblz4sPmfy0_S3dxM7SA=w600-h800-s-no?authuser=0";
        using VisionSource imageSource = VisionSource.FromFile(localImage); // VisionSource.FromUrl(imageUrl);

        // Load the image into memory
        //ComputerVisionDemo.PerformImageAnalysis(endpoint, visionKey, "LabCoat.jpg");
        //ComputerVisionDemo.PerformImageAnalysisOCR(endpoint, visionKey, "Handwriting.jpg");
        //ComputerVisionDemo.PerformImageAnalysisCropping(endpoint, visionKey, "Sprinkler.jpg");
        SegmentationDemo.PerformBackgroundRemoval(endpoint, visionKey, localImage);
        //SegmentationDemo.PerformForegroundMatting(options, imageSource);
    }

    private static IConfiguration GetSettings() =>
        new ConfigurationBuilder()
            .AddJsonFile("Settings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
}