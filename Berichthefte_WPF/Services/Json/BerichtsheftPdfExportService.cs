using Berichthefte_WPF.Models;
using Berichthefte_WPF.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Services.Json
{
    public class BerichtsheftPdfExportService : IPdfExportService
    {
        /// <summary>
        /// ExportToPdf() - Konvertiere Berichtsheft zu PDF und speichere in Datei
        ///
        /// 
        /// PARAMETER:
        /// • berichtsheft: Daten zum exportieren
        /// • outputPath: Wo soll PDF gespeichert werden
        /// 
        /// FLOW:
        /// 1. Validierung: berichtsheft != null?
        /// 2. QuestPDF License setzen (Community)
        /// 3. Document erstellen mit A4 Format
        /// 4. 5 Sektionen aufbauen (Header, Betrieb, Beschreibung, Schule, Footer)
        /// 5. GeneratePdf() - PDF in Datei schreiben
        /// </summary>
        public void ExportToPdf(Berichtsheft berichtsheft, string outputPath)
        {
            if (berichtsheft == null)
                throw new ArgumentNullException(nameof(berichtsheft));

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Content().Column(column =>
                    {
                        column.Spacing(0);

                        BuildHeader(column, berichtsheft);
                        BuildBetriebSection(column, berichtsheft);
                        BuildBeschreibungSection(column, berichtsheft);
                        BuildSchuleSection(column, berichtsheft);
                        BuildFooter(column, berichtsheft);
                    });
                });
            })
            .GeneratePdf(outputPath);
        }

        /// <summary>
        /// BuildHeader() - Erstelle Header-Sektion mit Trainee-Informationen
        /// 
        /// ZEIGT:
        /// • Name des Auszubildenden
        /// • Firma und Abteilung
        /// • Ausbildungsjahr
        /// • Ausbildungsnachweis-Nummer
        /// • Kalender-Woche und Zeitraum (Von/Bis)
        /// </summary>
        private void BuildHeader(ColumnDescriptor column, Berichtsheft berichtsheft)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);   
                    columns.RelativeColumn(3);   
                    columns.RelativeColumn(2);   
                });

                
                table.Cell().Border(1).Padding(5).Text($"Name: {berichtsheft.Trainee?.Name ?? string.Empty}").Bold();
                table.Cell().BorderTop(1).BorderBottom(1).BorderRight(1).Padding(5).Text("");
                table.Cell().Border(1).Padding(5).Text($"Ausbildungsnachweis Nr.: {berichtsheft.Zeitraum?.AusbildungsnachweisNr ?? 0}").Bold();

                
                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(5).Text($"Firma: {berichtsheft.Trainee?.Firma ?? string.Empty}").Bold();
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text($"Ausbildungsabteilung: {berichtsheft.Trainee?.Abteilung ?? string.Empty}").Bold();
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text($"Ausbildungsjahr: {berichtsheft.Trainee?.Ausbildungsjahr ?? 0}").Bold();

                
                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text($"Woche: {berichtsheft.Zeitraum?.KalenderWoche ?? 0}").Bold();

                
                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text($"vom: {FormatDate(berichtsheft.Zeitraum?.Von)}").Bold();

                
                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text("");
                table.Cell().BorderRight(1).BorderBottom(1).Padding(5).Text($"bis: {FormatDate(berichtsheft.Zeitraum?.Bis)}").Bold();
            });
        }

        /// <summary>
        /// BuildBetriebSection() - Erstelle Sektion für betriebliche Tätigkeiten
        /// 
        /// ZEIGT:
        /// • Liste aller betrieblichen Tätigkeiten
        /// • Aktivitätsbeschreibung mit Typ (Normal/Urlaub/Krank)
        /// • Zugehörige Stunden für jede Tätigkeit      
        /// </summary>
        private void BuildBetriebSection(ColumnDescriptor column, Berichtsheft berichtsheft)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(9);
                    columns.RelativeColumn(1);
                });

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4)
                    .Text("Betriebliche Tätigkeit (bitte Ausbildungsverlauf mit der zeitlichen und sachlichen Gliederung abgleichen):")
                    .Bold()
                    .FontSize(8);

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4)
                    .AlignCenter()
                    .Text("Stunden")
                    .Bold()
                    .FontSize(8);

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).MinHeight(210).Padding(6).Column(col =>
                {
                    if (berichtsheft.Betrieb != null && berichtsheft.Betrieb.Any())
                    {
                        foreach (var item in berichtsheft.Betrieb)
                        {
                            string text = $"- {item.Aktivitaet}";
                            if (item.Typ != BetrieblicheTaetigkeit.TaetigkeitTyp.Normal)
                                text += $" ({item.Typ})";

                            col.Item().PaddingBottom(4).Text(text);
                        }
                    }
                    else
                    {
                        col.Item().Text("");
                    }
                });

                table.Cell().BorderRight(1).BorderBottom(1).MinHeight(210).Padding(6).Column(col =>
                {
                    if (berichtsheft.Betrieb != null && berichtsheft.Betrieb.Any())
                    {
                        foreach (var item in berichtsheft.Betrieb)
                        {
                            col.Item().PaddingBottom(4).AlignCenter().Text(item.Stunden.ToString("0"));
                        }
                    }
                    else
                    {
                        col.Item().Text("");
                    }
                });
            });
        }

        /// <summary>
        /// BuildBeschreibungSection() - Erstelle Sektion für Arbeitsvorgangs-Beschreibung
        /// 
        /// ZEIGT:
        /// • Freitextfeld für Beschreibung eines Arbeitsvorgangs dieser Woche
        /// • Großes Textfeld mit Mindesthöhe 160px
        /// 
        /// </summary>
        private void BuildBeschreibungSection(ColumnDescriptor column, Berichtsheft berichtsheft)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(9);
                    columns.RelativeColumn(1);
                });

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4)
                    .Text("Beschreibung eines Arbeitsvorganges dieser Woche:")
                    .Bold()
                    .FontSize(8);

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4).Text("");

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).MinHeight(160).Padding(6)
                    .Text(berichtsheft.Beschreibung ?? string.Empty);

                table.Cell().BorderRight(1).BorderBottom(1).MinHeight(160).Padding(6).Text("");
            });
        }

        /// <summary>
        /// BuildSchuleSection() - Erstelle Sektion für Schulaktivitäten und Gesamtstunden
        /// 
        /// ZEIGT:
        /// • Liste aller Schulaktivitäten (Fach + Beschreibung)
        /// • Schulstunden-Summe
        /// • Gesamtstunden (Betrieb + Schule)
        /// </summary>
        private void BuildSchuleSection(ColumnDescriptor column, Berichtsheft berichtsheft)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(9);
                    columns.RelativeColumn(1);
                });

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4)
                    .Text("Berufsschule (Themen des Unterrichts):")
                    .Bold()
                    .FontSize(8);

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4).Text("");

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).MinHeight(130).Padding(6).Column(col =>
                {
                    if (berichtsheft.Schule != null && berichtsheft.Schule.Any())
                    {
                        foreach (var item in berichtsheft.Schule)
                        {
                            var line = $"- {item.Fach}";
                            if (!string.IsNullOrWhiteSpace(item.Beschreibung))
                                line += $": {item.Beschreibung}";

                            col.Item().PaddingBottom(3).Text(line);
                        }
                    }
                    else
                    {
                        col.Item().Text("");
                    }
                });

                table.Cell().BorderRight(1).BorderBottom(1).MinHeight(130).Padding(6)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text(berichtsheft.TotalSchulStunden.ToString("0"));
            });

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(9);
                    columns.RelativeColumn(1);
                });

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4)
                    .AlignRight()
                    .Text("Gesamtstunden")
                    .Bold();

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4)
                    .AlignCenter()
                    .Text(GetGesamtstunden(berichtsheft).ToString("0"))
                    .Bold();
            });
        }

        /// <summary>
        /// BuildFooter() - Erstelle Footer mit Unterschriftsfeldern
        /// 
        /// ZEIGT:
        /// • "Für die Richtigkeit" Label
        /// • Datum-Feld mit Signatur-Datum
        /// • Unterschriftslinie für Auszubildenden
        /// • Unterschriftslinie für Ausbilder
        /// </summary>
        private void BuildFooter(ColumnDescriptor column, Berichtsheft berichtsheft)
        {
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                });

                table.Cell().BorderLeft(1).BorderRight(1).BorderBottom(1).Padding(4).Text("Für die\nRichtigkeit").FontSize(8);

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4).Column(col =>
                {
                    col.Item().AlignCenter().Text(FormatSignatureDate(berichtsheft.TraineeSignature?.SignedAt));
                    col.Item().PaddingTop(6).LineHorizontal(0.5f);
                    col.Item().AlignCenter().Text("Datum").FontSize(8);
                });

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4).Column(col =>
                {
                    col.Item().Text("");
                    col.Item().PaddingTop(18).LineHorizontal(0.5f);
                    col.Item().AlignCenter().Text("Auszubildender").FontSize(8);
                });

                table.Cell().BorderRight(1).BorderBottom(1).Padding(4).Column(col =>
                {
                    col.Item().Text("");
                    col.Item().PaddingTop(18).LineHorizontal(0.5f);
                    col.Item().AlignCenter().Text("Ausbilder").FontSize(8);
                });
            });
        }

        /// <summary>
        /// FormatDate() - Formatiere DateTime zu deutschem Datumsformat
        /// 
        /// KONVERTIERT:
        /// • DateTime zu "dd.MM.yyyy" Format
        /// • Falls null: Leerer String
        /// </summary>
        private string FormatDate(DateTime? date)
        {
            return date?.ToString("dd.MM.yyyy") ?? string.Empty;
        }

        /// <summary>
        /// FormatSignatureDate() - Formatiere Signatur-Datum zu deutschem Format
        /// 
        /// KONVERTIERT:
        /// • Signatur DateTime zu "dd.MM.yyyy" Format
        /// • Falls null: Leerer String
        /// </summary>
        private string FormatSignatureDate(DateTime? date)
        {
            return date?.ToString("dd.MM.yyyy") ?? string.Empty;
        }

        /// <summary>
        /// GetGesamtstunden() - Berechne Summe aller Stunden (Betrieb + Schule)
        /// 
        /// BERECHNET:
        /// • Summe aller betrieblichen Stunden
        /// • Plus Schulstunden
        /// • Gibt Gesamtsumme zurück
        /// </summary>
        private double GetGesamtstunden(Berichtsheft berichtsheft)
        {
            double betrieb = berichtsheft.Betrieb?.Sum(x => x.Stunden) ?? 0;
            double schule = berichtsheft.TotalSchulStunden;
            return betrieb + schule;
        }
    }
}
