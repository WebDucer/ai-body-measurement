# Proposal: Projekt Grundgerüst

## Why

Erstellen einer plattformübergreifenden .NET MAUI App zur Aufzeichnung des Körpergewichts mit lokaler Datenspeicherung (Local First). Die App ermöglicht Nutzern eine einfache, mehrsprachige Lösung zur persönlichen Gewichtsverfolgung mit Verlaufsanzeige und Statistiken. Der Fokus liegt auf einer testgetriebenen, wartbaren Codebasis als Fundament für zukünftige Erweiterungen.

## What Changes

- Neue .NET MAUI App (App ID: `page.eugen.maui.ai.bodymeasurement`) für iOS, Android, Windows und macOS
- Mehrsprachigkeit (Deutsch und Englisch) mittels Lokalisierung
- Lokale Datenspeicherung (SQLite) für Gewichtsmessungen
- UI zur Erfassung von Gewichtsdaten (Wert, Datum, optionale Notizen)
- Verlaufsansicht mit chronologischer Darstellung aller Messungen
- Statistik-Dashboard mit Trends, Durchschnittswerten und Gewichtsveränderungen
- Export-Funktionalität für gespeicherte Daten (CSV, JSON)
- Unit Tests für App-Logik und Datenverarbeitung
- Integration Tests für kritische User-Flows

## Capabilities

### New Capabilities

- `weight-tracking`: Erfassung, Speicherung und Verwaltung von Körpergewichtsmessungen mit Datum, Zeitstempel und optionalen Notizen
- `data-visualization`: Anzeige des Gewichtsverlaufs als Liste und grafische Darstellung mit Filtermöglichkeiten
- `statistics-analytics`: Berechnung und Anzeige von Statistiken (Durchschnitt, Min/Max, Trends, Gewichtsveränderungen über Zeiträume)
- `data-export`: Export der Gewichtsdaten in verschiedene Formate (CSV, JSON) für externe Nutzung
- `localization`: Mehrsprachige Benutzeroberfläche mit Unterstützung für Deutsch und Englisch
- `local-storage`: Lokale Datenspeicherung mit SQLite für offline-first Nutzung ohne Cloud-Abhängigkeit

### Modified Capabilities

<!-- Keine bestehenden Capabilities werden modifiziert - dies ist ein neues Projekt -->

## Impact

**New Code:**
- .NET MAUI Projektstruktur mit Platform-spezifischen Implementierungen
- MVVM-Pattern mit ViewModels und Services
- SQLite Datenbank-Schema und Repository-Layer
- Lokalisierungs-Ressourcen (Deutsch/Englisch)
- Unit Test Projekt mit xUnit/NUnit
- CI/CD Pipeline für automatisierte Tests

**Dependencies:**
- .NET 10 SDK
- .NET MAUI Framework
- SQLite (Microsoft.Data.Sqlite oder sqlite-net-pcl)
- Lokalisierungs-Bibliotheken
- Charting-Library für Visualisierungen (z.B. Microcharts, LiveCharts2)
- Testing Frameworks (xUnit/NUnit, Moq)

**Platforms:**
- iOS (iPhone/iPad)
- Android (Smartphones/Tablets)
- Windows Desktop
- macOS Desktop

**Future Extensibility:**
- Architektur vorbereitet für zusätzliche Körpermessungen (BMI, Körperfett, Umfang, etc.)
- Modular aufgebaut für zukünftige Features (Cloud-Sync, Erinnerungen, Ziele)
