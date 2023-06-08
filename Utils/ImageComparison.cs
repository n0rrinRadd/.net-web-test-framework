﻿// Adaption of open source code found here https://github.com/IDeaSCo/ImageComparison
// .net based image comparison utillity which produces pixel matrix based comaprison
// Capable of detecting a single pixel difference between images

// TODO:
// Currently not efficient, loads the image each time it checks a byte, need to refactor this
// Code cleanup/refactor

using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public static class ImageComparison
{
    //the font to use for the DifferenceImages
    private static readonly Font DefaultFont = new Font("Arial", 8);
    public static int divFactor = 10;
    //the brushes to use for the DifferenceImages
    private static Brush[] brushes = new Brush[256];

    //the colormatrix needed to grayscale an image

    readonly static ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
       {
            new float[] {.3f, .3f, .3f, 0, 0},
            new float[] {.59f, .59f, .59f, 0, 0},
            new float[] {.11f, .11f, .11f, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
       });

    /// <summary>
    /// Gets the difference between two images as a percentage
    /// </summary>
    /// <param name="img1">The first image</param>
    /// <param name="img2">The image to compare to</param>
    /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
    /// <returns>The difference between the two images as a percentage</returns>
    public static float Differences(this Image img1, Image img2, byte threshold = 0)
    {
        byte[,] differences = img1.GetDifferences(img2);
        int diffPixels = 0;

        foreach (byte b in differences)
        {
            if (b > threshold) { diffPixels++; }
        }
        return diffPixels / 256f;
    }

    /// <summary>
    /// Gets an image which displays the differences between two images
    /// </summary>
    /// <param name="img1">The first image</param>
    /// <param name="img2">The image to compare with</param>
    /// <returns>an image which displays the differences between two images</returns>
    public static Bitmap GetDifferenceImage(this Image img1, Image img2)
    {
        //create a 16x16 tiles image with information about how much the two images differ
        int cellsize = 16;  //each tile is 16 pixels wide and high
        int width = img1.Width / divFactor, height = img1.Height / divFactor;
        byte[,] differences = img1.GetDifferences(img2);
        byte maxDifference = 255;

        Bitmap originalImage = new Bitmap(img1, width * cellsize + 1, height * cellsize + 1);
        Graphics g = Graphics.FromImage(originalImage);

        for (int y = 0; y < differences.GetLength(1); y++)
        {
            for (int x = 0; x < differences.GetLength(0); x++)
            {
                byte cellValue = differences[x, y];
                string cellText = null;
                cellText = cellValue.ToString();
                float percentageDifference = (float)differences[x, y] / maxDifference;
                if (cellValue > 0)
                {
                    g.DrawRectangle(Pens.DarkMagenta, x * cellsize, y * cellsize, cellsize, cellsize);
                }
            }
        }
        return originalImage;
    }

    public static void CreateDifferenceImage(Image img1, Image img2, string testTitle)
    {
        CreateDifferenceImage(img1, img2, testTitle, "{0:yyyy-MM-ddTHH-mm-ss}");
    }

    /// <summary>
    /// Created an image showing the difference between two images
    /// </summary>
    /// <param name="img1">The first image to compare</param>
    /// <param name="img2">The second image to compare</param>
    /// <param name="info">details to help describe method of comparison</param>
    public static void CreateDifferenceImage(Image img1, Image img2, string testTitle, string filename)
    {
        string dir = "Screenshots\\Diff";
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var environmentDir = new DirectoryInfo(baseDir);
        environmentDir.CreateSubdirectory(dir);

        string dirPath = environmentDir.ToString() + dir + "\\";
        string differencesFilename = string.Format(filename + "-Differences.png", DateTime.Now);
        img2.GetDifferenceImage(img1).Save(dirPath + differencesFilename);

        Debug.WriteLine(@"-> Unexpected difference(s) found in scenario - " + testTitle);
        Debug.WriteLine(@"-> Logging differences screenshot to: - file:///" + dirPath + differencesFilename);

    }

    /// <summary>
    /// Finds the differences between two images and returns them in a doublearray
    /// </summary>
    /// <param name="img1">The first image</param>
    /// <param name="img2">The image to compare with</param>
    /// <returns>the differences between the two images as a doublearray</returns>
    public static byte[,] GetDifferences(this Image img1, Image img2)
    {
        int width = img1.Width / divFactor, height = img1.Height / divFactor;
        Bitmap thisOne = (Bitmap)img1.Resize(width, height).GetGrayScaleVersion();
        Bitmap theOtherOne = (Bitmap)img2.Resize(width, height).GetGrayScaleVersion();

        byte[,] differences = new byte[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                differences[x, y] = (byte)Math.Abs(thisOne.GetPixel(x, y).R - theOtherOne.GetPixel(x, y).R);
            }
        }
        return differences;
    }

    /// <summary>
    /// Converts an image to grayscale
    /// </summary>
    /// <param name="original">The image to grayscale</param>
    /// <returns>A grayscale version of the image</returns>
    public static Image GetGrayScaleVersion(this Image original)
    {

        //create a blank bitmap the same size as original
        Bitmap newBitmap = new Bitmap(original.Width, original.Height);

        //get a graphics object from the new image
        Graphics g = Graphics.FromImage(newBitmap);

        //create some image attributes
        ImageAttributes attributes = new ImageAttributes();

        //set the color matrix attribute
        attributes.SetColorMatrix(ColorMatrix);

        //draw the original image on the new image
        //using the grayscale color matrix
        g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
           0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

        //dispose the Graphics object
        g.Dispose();

        return newBitmap;

    }

    /// <summary>
    /// Resizes an image
    /// </summary>
    /// <param name="originalImage">The image to resize</param>
    /// <param name="newWidth">The new width in pixels</param>
    /// <param name="newHeight">The new height in pixels</param>
    /// <returns>A resized version of the original image</returns>
    public static Image Resize(this Image originalImage, int newWidth, int newHeight)
    {
        Image smallVersion = new Bitmap(newWidth, newHeight);
        using (Graphics g = Graphics.FromImage(smallVersion))
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }

        return smallVersion;
    }

    public static int GetDifference(string image1, string image2)
    {
        return GetDifference(image1, image2, "TwoImages.");
    }
    /// <summary>
    /// Gets the difference between two images as a percentage
    /// </summary>
    /// <returns>The difference between the two images as a percentage</returns>
    /// <param name="image1">The first image to compare</param>
    /// <param name="image2">The second image to compare</param>
    /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 0.</param>
    /// <returns>The difference between the two images as a percentage</returns>
    public static int GetDifference(string image1, string image2, string filename)
    {
        // Ensure both images exist
        if (File.Exists(image1) && File.Exists(image2))
        {
            Image img1 = Image.FromFile(image1);
            Image img2 = Image.FromFile(image2);

            float differencePercentage = img1.Differences(img2, 0);

            if (differencePercentage > 0)
            {
                // take snapshot of difference
                CreateDifferenceImage(img1, img2, "TwoImages.", filename);
            }
            return (int)(differencePercentage * 100);
        }
        else return -1;
    }

    public static byte[] GetScreenshotByte(IWebDriver driver)
    {

        Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
        string screenshot = ss.AsBase64EncodedString;
        byte[] bytes = Convert.FromBase64String(screenshot);

        driver.Quit();

        return bytes;
    }

    /// <summary>
    /// Get difference between image and a page of a pdf which is converted into 
    /// an image and held in memory to compare
    /// </summary>
    /// <param name="image1">image to compare</param>
    /// <param name="pdf">pdf file to use</param>
    /// <param name="page">page of pdf to convert</param>
    /// <returns>Differences between an image and an image taken from a specified pdf page</returns>
    public static int GetDifference(string baseImage, string pdf, int page)
    {
        return -1;
    }
}