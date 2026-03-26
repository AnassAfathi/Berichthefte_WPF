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
    /// <summary>
    /// JsonBerichtsheftStorage - Service für Speicherung und Laden von Berichtsheft-Daten
    /// 
    /// AUFGABE:
    /// • Speichert Berichtsheft-Objekte als JSON-Dateien
    /// • Lädt Berichtsheft-Objekte aus JSON-Dateien
    /// </summary>
    public class JsonBerichtsheftStorage : IBerichtsheftStorage
    {       
        private readonly JsonSerializerOptions _options =
           new JsonSerializerOptions
           {
               WriteIndented = true
           };

        /// <summary>
        /// SaveBerichtsheft() - Speichere Berichtsheft als JSON-Datei
        /// 
        /// ZWECK:
        /// • Konvertiere Berichtsheft-Objekt zu JSON-String
        /// • Erstelle Ordner falls nicht vorhanden
        /// • Schreibe JSON in Datei
        /// 
        /// PARAMETER:
        /// • bericht: Berichtsheft-Objekt mit allen Daten
        /// • filePath: Ziel-Pfad (z.B. "C:\Data\Reports\CurrentBerichtsheft.json")
        /// 
        /// FLOW:
        /// 1. Extrahiere Ordner aus filePath
        /// 2. Prüfe: Existiert Ordner?
        /// 3. Falls NEIN: Erstelle ihn (auch Parent-Ordner)
        /// 4. Serialisiere Berichtsheft zu JSON-String
        /// 5. Schreibe JSON in Datei
        /// 
        /// RESULTAT: JSON-Datei wurde erstellt/aktualisiert!
        /// </summary>
        public void SaveBerichtsheft(Berichtsheft bericht, string filePath)
        {          
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            var json = JsonSerializer.Serialize(bericht, _options);

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// LoadBerichtsheft() - Lade Berichtsheft aus JSON-Datei
        /// 
        /// ZWECK:
        /// • Lese JSON-Datei
        /// • Konvertiere JSON zu Berichtsheft-Objekt
        /// • Gib Objekt zurück
        /// 
        /// PARAMETER:
        /// • filePath: Pfad zur JSON-Datei (z.B. "C:\Data\Reports\CurrentBerichtsheft.json")
        /// 
        /// FLOW:
        /// 1. Prüfe: Existiert Datei?
        /// 2. Falls NEIN: Werfe FileNotFoundException
        /// 3. Falls JA: Lese JSON-String aus Datei
        /// 4. Deserialisiere JSON zu Berichtsheft-Objekt
        /// 5. Gib Objekt zurück
        /// 
        /// EXCEPTION:
        /// • FileNotFoundException falls Datei nicht existiert
        /// 
        /// RESULTAT: Berichtsheft-Objekt geladen!
        /// </summary>
        public Berichtsheft LoadBerichtsheft(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Berichtsheft not found.");
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Berichtsheft>(json);
        }
    }
}
