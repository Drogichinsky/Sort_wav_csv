// Сортирует Wav файлы в папки, согласно именам csv-списков, в которых они описаны (e.g. 514a0723-cf331b65-863b4622-ee496,2023-01-27 13:55:40.167,12,5653,79XXXXXXXXX)
// Использование: SortWav.exe \\путь к wav файлам \\путь к csv
using System;
using System.IO;
//using System.Media;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;



string[] arguments = Environment.GetCommandLineArgs();
string pathwav = arguments[1];
string pathcsv = arguments[2];
string[] wavPaths = Directory.GetFiles(@pathwav, "*.wav");
string[] csvPaths = Directory.GetFiles(@pathcsv, "*.csv");
int i = 0;

//Pass the filepath and filename to the StreamWriter Constructor
StreamWriter sw = new StreamWriter("SortWav.log");
string csvsout = "csvsout.txt";
//Write a line of text
sw.WriteLine("Start");
string[,] mass = new string[2, 20000];

foreach (string filePath in csvPaths)
{
    string cuttedFPcsv = filePath.Substring(0, filePath.Length - 4);
    var FPdir = cuttedFPcsv.Split('\\').Last();
    string cuttedFP = pathwav + "\\" + FPdir;
    try
    {

        if (!Directory.Exists(cuttedFP))
        {
            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(cuttedFP);
        }
    }
    catch (IOException ioex)
    {
        Console.WriteLine(ioex.Message);
    }

    using (StreamReader reader = new StreamReader(@filePath))
    {
        //Console.WriteLine(filePath);
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            var delimitedLine = line.Split(';');
            mass[0, i] = delimitedLine[0];
            mass[1, i] = cuttedFP;
            Console.WriteLine(mass[0, i] + " " + mass[1, i]);
            i++;
        }
        Console.WriteLine("--------------------");
    }

}
string pattern = @"\w{8}\b[-]\w{8}\b[-]\w{8}\b[-]\w{5}";
Regex rg = new Regex(pattern);
string pattern2 = @"\d{11}";
Regex rg1 = new Regex(pattern2);
foreach (string filePath in wavPaths)
{
    MatchCollection matches1 = rg1.Matches(filePath);
    MatchCollection matches = rg.Matches(filePath);
    if (matches1.Count > 0)
    {
        foreach (Match match1 in matches1)
        {
            string phone = match1.Value;
            Console.WriteLine(match1.Value);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    string callid = match.Value;
                    for (i = 0; i < mass.GetLength(1) - 1;)
                    {
                        i++;
                        if (mass[0, i] == callid)
                        {
                            Console.WriteLine(i + " " + filePath + " " + mass[0, i] + " " + mass[1, i]);
                            sw.WriteLine(callid);
                            var delimitedFileName = filePath.Split('\\').Last();
                            string filetocheck = mass[1, i] + "\\" + delimitedFileName + phone;
                            Console.WriteLine("filetocheck: " + filetocheck);
                            if (File.Exists(filetocheck))
                                File.Delete(filetocheck);
                            File.Move(filePath, filetocheck);
                            Console.WriteLine("{0} was moved to {1}.", filePath, filetocheck);
                            sw.WriteLine("{0} was moved to {1}.", filePath, filetocheck);

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Нет файлов в списках");

            }
        }
    }


}

sw.Close();
