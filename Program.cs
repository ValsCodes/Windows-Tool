using System.Diagnostics;

string[] options = ["Scan and Repair files", "Clean Temp files", "Viruses Check", "Drivers Checklist", "TCP Connections Check"];
Dictionary<string, bool> driversChecklist = new Dictionary<string, bool>
{
    { "CPU", false },
    { "GPU", false },
    { "Network", false },
    { "Storage", false },
    { "Hardware", false },
    { "Audio", false },
    { "Bios", false }
};

const string APP_AUTO_RUN_EXE = "Autoruns64.exe";
const string APP_TCP_VIEW_EXE = "tcpview64.exe";

while (true)
{
    PrintLogo();
    PrintOptions();

    byte number;
    bool option = byte.TryParse(Console.ReadLine(), out number);

    if (option && !(number < 0) && !(number > options.Length + 1))
    {
        switch (number)
        {
            case 1: RunSfcCommand(); break;
            case 2: ClearTempFiles(); break;
            case 3: RunApp(APP_AUTO_RUN_EXE); break;
            case 4: ShowDriversChecklist(); break;
            case 5: RunApp(APP_TCP_VIEW_EXE); break;

            default: return;
        }
    }

    Console.Clear();
}


void ShowDriversChecklist()
{
    List<KeyValuePair<string, bool>> checklist = new List<KeyValuePair<string, bool>>(driversChecklist);

    int selectedIndex = 0;
    ConsoleKey key;

    do
    {
        Console.Clear();
        Console.WriteLine("Driver's Checklist (Use UP/DOWN arrows to navigate, SPACE to toggle, ESC to quit):");

        for (int i = 0; i < driversChecklist.Count; i++)
        {
            var item = checklist[i];

            // Highlight the selected item
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"> {item.Key,8}: {(item.Value ? "[X]" : "[ ]")}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"  {item.Key,8}: {(item.Value ? "[X]" : "[ ]")}");
            }
        }

        // Read the user key input
        key = Console.ReadKey().Key;

        // Handle navigation and toggling
        if (key == ConsoleKey.UpArrow)
        {
            selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : checklist.Count - 1;
        }
        else if (key == ConsoleKey.DownArrow)
        {
            selectedIndex = selectedIndex < checklist.Count - 1 ? selectedIndex + 1 : 0;
        }
        else if (key == ConsoleKey.Spacebar)
        {
            // Toggle the state of the selected item
            var currentItem = checklist[selectedIndex];
            checklist[selectedIndex] = new KeyValuePair<string, bool>(currentItem.Key, !currentItem.Value);
        }

    } while (key != ConsoleKey.Escape);

    // Update the dictionary with the final values
    driversChecklist = new Dictionary<string, bool>();
    foreach (var item in checklist)
    {
        driversChecklist[item.Key] = item.Value;
    }
}

void PrintLogo()
{
    Console.WriteLine("\r\n __      __.__            .___                    ___________           .__   \r\n/  \\    /  \\__| ____    __| _/______  _  ________ \\__    ___/___   ____ |  |  \r\n\\   \\/\\/   /  |/    \\  / __ |/  _ \\ \\/ \\/ /  ___/   |    | /  _ \\ /  _ \\|  |  \r\n \\        /|  |   |  \\/ /_/ (  <_> )     /\\___ \\    |    |(  <_> |  <_> )  |__\r\n  \\__/\\  / |__|___|  /\\____ |\\____/ \\/\\_//____  >   |____| \\____/ \\____/|____/\r\n       \\/          \\/      \\/                 \\/                              \r\n");
}

void PrintOptions()
{
    Console.WriteLine("Select Option:");

    var i = 1;
    foreach (string option in options)
    {
        Console.WriteLine($"{i}. {option}");
        i++;
    }

    Console.WriteLine("\n0. Exit");
}

void RunSfcCommand()
{
    string command = "sfc /scannow";

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c {command}",
            Verb = "runas",
            UseShellExecute = true,
            CreateNoWindow = true
        }
    };

    try
    {
        process.Start();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to execute command as administrator: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("Process Finished!");
    }
}

void ClearTempFiles()
{
    try
    {
        string tempPath = Path.GetTempPath();

        Console.WriteLine($"Temporary folder: {tempPath}");

        string[] tempFiles = Directory.GetFiles(tempPath);
        string[] tempDirectories = Directory.GetDirectories(tempPath);

        foreach (var file in tempFiles)
        {
            try
            {
                File.Delete(file);
                Console.WriteLine($"Deleted file: {file}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file {file}: {ex.Message}");
            }
        }

        foreach (var directory in tempDirectories)
        {
            try
            {
                Directory.Delete(directory, true);
                Console.WriteLine($"Deleted directory: {directory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete directory {directory}: {ex.Message}");
            }
        }

        Console.WriteLine("Temporary files and directories cleared.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

void RunApp(string appExe)
{
    try
    {
        string fullExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appExe);

        Console.WriteLine($"Running executable: {fullExePath}");

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fullExePath,
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };

        process.Start();
        //process.WaitForExit();

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error running executable: {ex.Message}");
    }
}
