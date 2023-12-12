using Newtonsoft.Json;
using System.Xml.Serialization;
[Serializable]
public class Figure
{
    public string Name { get; set; }
    public double Shirina { get; set; }
    public double Visota { get; set; }

    public Figure()
    {
    }

    public Figure(string name, double shirina, double visota)
    {
        Name = name;
        Shirina = shirina;
        Visota = visota;
    }
}

public class FileManager
{
    private string filePath;
    private List<Figure> figures;

    public FileManager(string filePath)
    {
        this.filePath = filePath;
        this.figures = new List<Figure>();

        if (File.Exists(filePath))
        {
            Load();
        }
        else
        {
            Console.WriteLine("Файл не существует.");
        }
    }

    private void LoadFromTxtFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i += 3)
        {
            string name = lines[i];
            double width = Convert.ToDouble(lines[i + 1]);
            double height = Convert.ToDouble(lines[i + 2]);
            figures.Add(new Figure(name, width, height));
        }
    }

    private void LoadFromJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        figures = JsonConvert.DeserializeObject<List<Figure>>(json);
    }

    private void LoadFromXmlFile(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            figures = (List<Figure>)serializer.Deserialize(fileStream);
        }
    }

    private void Load()
    {
        string fileExtension = Path.GetExtension(filePath).ToLower();

        if (fileExtension == ".txt")
        {
            LoadFromTxtFile(filePath);
        }
        else if (fileExtension == ".json")
        {
            LoadFromJsonFile(filePath);
        }
        else if (fileExtension == ".xml")
        {
            LoadFromXmlFile(filePath);
        }
        else
        {
            Console.WriteLine("Неподдерживаемый формат файла.");
        }
    }

    public void SaveToTxtFile(string filePath)
    {
        List<string> lines = new List<string>();
        foreach (var figure in figures)
        {
            lines.Add(figure.Name);
            lines.Add(figure.Shirina.ToString());
            lines.Add(figure.Visota.ToString());
        }
        File.WriteAllLines(filePath, lines);
    }

    public void SaveToJsonFile(string filePath)
    {
        string json = JsonConvert.SerializeObject(figures);
        File.WriteAllText(filePath, json);
    }

    public void SaveToXmlFile(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(fileStream, figures);
        }
    }

    public FileType ChooseFileType()
    {
        Console.WriteLine("Выберите тип файла для сохранения (1 - txt, 2 - json, 3 - xml):");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            return FileType.Txt;
        }
        else if (choice == "2")
        {
            return FileType.Json;
        }
        else if (choice == "3")
        {
            return FileType.Xml;
        }
        else
        {
            Console.WriteLine("Неверный выбор. Файл не будет сохранен.");
            return FileType.Unknown;
        }
    }

    public void Save(FileType fileType)
    {
        Console.WriteLine("Введите название файла для сохранения (без расширения):");
        string fileName = Console.ReadLine();

        if (string.IsNullOrEmpty(fileName))
        {
            Console.WriteLine("Название файла не может быть пустым. Файл не будет сохранен.");
            return;
        }
        string fileExtension = "." + fileType.ToString().ToLower();
        string savePath = Path.Combine(Path.GetDirectoryName(filePath), fileName + fileExtension);

        if (fileType == FileType.Txt)
        {
            SaveToTxtFile(savePath);
        }
        else if (fileType == FileType.Json)
        {
            SaveToJsonFile(savePath);
        }
        else if (fileType == FileType.Xml)
        {
            SaveToXmlFile(savePath);
        }
    }

    public void DisplayFigures()
    {
        if (figures != null)
        {
            foreach (var figure in figures)
            {
                Console.WriteLine($"Имя: {figure.Name}");
            }
        }
    }
}

public enum FileType
{
    Txt,
    Json,
    Xml,
    Unknown
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к файлу, который вы хотите открыть");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите формат в котором нужно сохранить файл: txt, json, xml - F1. Закрыть программу - Escape");
            fileManager.DisplayFigures();
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.F1)
            {
                FileType fileType = fileManager.ChooseFileType();
                if (fileType != FileType.Unknown)
                {
                    fileManager.Save(fileType);
                    Console.WriteLine("Файл успешно сохранен на вашем рабочем столе.");
                    Console.ReadKey();
                }
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}
