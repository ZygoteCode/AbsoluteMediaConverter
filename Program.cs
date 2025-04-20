using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class Program
{
    public static void Main()
    {
        Console.Title = "AbsoluteMediaConverter | Made by https://github.com/ZygoteCode/";

        if (!Directory.Exists("inputs"))
        {
            Directory.CreateDirectory("inputs");

            Console.WriteLine("No media files have been inserted in the \"inputs\" folder.");
            Console.WriteLine("Press the ENTER key in order to exit from the program.");

            Console.ReadLine();
            return;
        }

        if (Directory.Exists("outputs"))
        {
            Directory.Delete("outputs", true);
        }

        if (!File.Exists("custom_ffmpeg_command.txt"))
        {
            File.WriteAllText("custom_ffmpeg_command.txt", "-i %INPUT_FILE% -c:v copy -c:a aac -f %EXTENSION% %OUTPUT_FILE%");
        }

        Directory.CreateDirectory("outputs");
        string option = "";

        while (option != "1" && option != "2" && option != "3" && option != "4")
        {
            Console.WriteLine("What type of you media do you want to convert?\r\n\r\n[1] Video\r\n[2] Audio\r\n[3] Image\r\n[4] Custom FFMpeg command (file \"ffmpeg_custom_command.txt\")");
            Console.Write("> ");
            option = Console.ReadLine();

            if (option != "1" && option != "2" && option != "3" && option != "4")
            {
                Console.WriteLine("Unrecognized answer. Please, try again.");
            }
        }

        string extension = "";

        while (extension == "")
        {
            Console.WriteLine("What extension do you want in output?");
            Console.Write("> ");
            extension = Console.ReadLine().Replace(".", "").Replace(",", "").Replace(" ", "").Replace('\t'.ToString(), "").ToLower();

            if (extension == "")
            {
                Console.WriteLine("Unrecognized extension. Please, try again.");
            }
        }

        Console.WriteLine("Converting your media files, please wait a while.");

        foreach (string file in Directory.GetFiles("inputs"))
        {
            new Thread(() =>
            {
                string inputFullPath = Path.GetFullPath(file);
                string outputFullPath = Path.GetFullPath("outputs") + "\\" + Path.GetFileNameWithoutExtension(inputFullPath) + "." + extension;

                if (option == "1")
                {
                    RunFFMpeg($"-i \"{inputFullPath}\" -c:v copy -c:a aac -f {extension} \"{outputFullPath}\"");
                }
                else if (option == "2")
                {
                    RunFFMpeg($"-i \"{inputFullPath}\" -vn -f {extension} \"{outputFullPath}\"");
                }
                else if (option == "3")
                {
                    RunFFMpeg($"-i \"{inputFullPath}\" -f {extension} \"{outputFullPath}\"");
                }
                else if (option == "4")
                {
                    string customCommand = File.ReadAllText("custom_ffmpeg_command.txt");

                    customCommand = customCommand.Replace("%INPUT_FILE%", "\"" + inputFullPath + "\"");
                    customCommand = customCommand.Replace("%OUTPUT_FILE%", "\"" + outputFullPath + "\"");
                    customCommand = customCommand.Replace("%EXTENSION%", "\"" + extension + "\"");

                    RunFFMpeg(customCommand);
                }
            }).Start();
        }

        while (true)
        {
            int inputs = Directory.GetFiles("inputs").Length;
            int outputs = Directory.GetFiles("outputs").Length;

            if (inputs == outputs)
            {
                Console.WriteLine("Succesfully converted all of your media files! The results are in the \"outputs\" folder.");
                Console.WriteLine("Press the ENTER key to exit from the program.");

                Console.ReadLine();
                return;
            }
        }
    }

    private static void RunFFMpeg(string arguments)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg.exe",
            Arguments = $"-threads {Environment.ProcessorCount} {arguments}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        }).WaitForExit();
    }
}
