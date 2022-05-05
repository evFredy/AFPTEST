using Newtonsoft.Json;
using System.Text.RegularExpressions;

var ToProcess = ExtractData.GetFiles();

foreach (var item in ToProcess)
{
    var Result = ExtractData.GetDataFormFile(item);
    Console.WriteLine(JsonConvert.SerializeObject(Result, Formatting.Indented));
}

public static class ExtractData
{
    public static string[] GetFiles()
    {
        string[] Files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
        foreach (var file in Files)
        {
            Console.WriteLine(file);
        }
        return Files;
    }

    public static PersonalData GetDataFormFile(string FileName)
    {
        string text = File.ReadAllText(FileName);
        string[] lines = text.Split(Environment.NewLine);
        var Result = new PersonalData();
        foreach (string line in lines)
        {
            var m = Regex.Match(line, @":");
            if (m.Success)
            {
                var Value = line.Substring(m.Index + m.Length).Trim();
                if (IsDui(Value))
                {
                    Result.DUI = Value;
                }
                else if (IsFecha(Value))
                {
                    Result.FechaNacimiento = DateTime.ParseExact(Value, "dd/MM/yyyy", null);
                }
                else if (IsNombre(Value))
                {
                    if (string.IsNullOrWhiteSpace(Result.Nombres))
                    {
                        Result.Nombres = Value;
                    }
                    else
                    {
                        Result.Apellidos = Value;
                    }
                }
            }
        }
        return Result;
    }

    public static bool IsDui(string Value)
    {
        return Regex.IsMatch(Value, @"^\d+$");
    }

    public static bool IsNombre(string Value)
    {
        return Regex.IsMatch(Value, @"^[a-zA-Z ]+$");
    }

    public static bool IsFecha(string Value)
    {
        return Regex.IsMatch(Value, @"\d\d/\d\d/\d\d\d\d");
    }
}

public class PersonalData
{
    public string Nombres { get; set; }

    public string Apellidos { get; set; }

    public string DUI { get; set; }

    public DateTime FechaNacimiento { get; set; }
}