using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GestorTareasKanban.Models
{
    public static class TaskStorage
    {
        private static readonly string FilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tareas.json");

        public static void Save(List<TaskData> tareas)
        {
            var json = JsonSerializer.Serialize(tareas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static List<TaskData> Load()
        {
            if (!File.Exists(FilePath))
                return new List<TaskData>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<TaskData>>(json)
                   ?? new List<TaskData>();
        }
    }
}
