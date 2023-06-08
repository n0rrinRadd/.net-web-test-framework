using Microsoft.Expression.Encoder.ScreenCapture;
using System;
using System.Drawing;
using System.IO;

namespace TestFramework.Utils
{
    public static class ScreenCapture
    {
        public static string StartRecordingVideo(ScreenCaptureJob scj, int width, int height)
        {
            return StartRecordingVideo(scj, "ScreenCapture", width, height);
        }

        public static string StartRecordingVideo(ScreenCaptureJob scj, string scenarioTitle, int width, int height)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var environmentDir = new DirectoryInfo(baseDir);
            string dirPath = environmentDir.ToString() + "ScreenRecordings";
            string fileNamePath = dirPath + "\\" + scenarioTitle + "_" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss") + ".xesc";
            scj = new ScreenCaptureJob();
            scj.OutputScreenCaptureFileName = fileNamePath;
            scj.OutputPath = dirPath;
            scj.CaptureRectangle = new Rectangle(0, 0, width, height);
            scj.Start();
            return fileNamePath;
        }

        public static void EndRecordingVideo(ScreenCaptureJob scj)
        {
            scj.Stop();
            scj.Dispose();
        }

    }
}
