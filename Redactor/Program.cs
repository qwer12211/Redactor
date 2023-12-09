using System;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class FileManager
{
    private string filePath;

    public FileManager(string path)
    {
        filePath = path;
    }

    public Figure[] Load()
    {
        string extension = Path.GetExtension(filePath);

        if (extension == ".txt")
        {
            return LoadFromTxt();
        }
        else if (extension == ".json")
        {
            return LoadFromJson();
        }
        else if (extension == ".xml")
        {
            return LoadFromXml();
        }

        throw new NotSupportedException("Unsupported file format");
    }

    private Figure[] LoadFromTxt()
    {
        string[] lines = File.ReadAllLines(filePath);
        Figure[] figures = new Figure[lines.Length / 3];

        for (int i = 0, j = 0; i < lines.Length; i += 3, j++)
        {
            string name = lines[i];
            double width = Convert.ToDouble(lines[i + 1]);
            double height = Convert.ToDouble(lines[i + 2]);

            figures[j] = new Figure(name, width, height);
        }

        return figures;
    }

    private Figure[] LoadFromJson()
    {
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Figure[]>(json);
    }

    private Figure[] LoadFromXml()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure[]));

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            return (Figure[])serializer.Deserialize(fileStream);
        }
    }

    public void Save(Figure[] figures)
    {
        string extension = Path.GetExtension(filePath);

        if (extension == ".txt")
        {
            SaveAsTxt(figures);
        }
        else if (extension == ".json")
        {
            SaveAsJson(figures);
        }
        else if (extension == ".xml")
        {
            SaveAsXml(figures);
        }
        else
        {
            throw new NotSupportedException("Unsupported file format");
        }
    }

    private void SaveAsTxt(Figure[] figures)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var figure in figures)
            {
                writer.WriteLine(figure.Name);
                writer.WriteLine(figure.Width);
                writer.WriteLine(figure.Height);
            }
        }
    }

    private void SaveAsJson(Figure[] figures)
    {
        string json = JsonConvert.SerializeObject(figures, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }

    private void SaveAsXml(Figure[] figures)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Figure[]));

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(fileStream, figures);
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к файлу:");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);
        Figure[] figures = fileManager.Load();

        Console.WriteLine("Для сохранения файла нажмите F1. Для выхода - Escape.");

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.F1)
            {
                fileManager.Save(figures);
                Console.WriteLine("Файл успешно сохранен.");
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}
public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure() { }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}
