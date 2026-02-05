using Berichthefte_WPF.Models;
using Berichthefte_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Services.Json
{
    public class JsonBerichtsheftStorage : IBerichtsheftStorage
    {
        private readonly JsonSerializerOptions _options =
       new JsonSerializerOptions
       {
           WriteIndented = true
       };
        public void SaveBerichtsheft(Berichtsheft bericht, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var json = JsonSerializer.Serialize(bericht, _options);
            File.WriteAllText(filePath, json);
        }

        public Berichtsheft LoadBerichtsheft(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Berichtsheft not found.");

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Berichtsheft>(json);
        }
    }
}
