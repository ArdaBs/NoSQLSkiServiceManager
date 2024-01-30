
# NoSQL Ski Service Manager

- [NoSQL Ski Service Manager](#nosql-ski-service-manager)
- [Projektstruktur](#projektstruktur)
  - [Überblick](#überblick)
  - [Systemvoraussetzungen](#systemvoraussetzungen)
  - [MongoDB Konfiguration](#mongodb-konfiguration)
  - [Projekt-Setup](#projekt-setup)
  - [Verwendung](#verwendung)
  - [Tests](#tests)

# Projektstruktur

Das NoSQL Ski Service Manager-Projekt ist sorgfältig in mehrere Verzeichnisse gegliedert, die für verschiedene Aspekte der Anwendung stehen:

- [`/Controllers`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Controllers): Dieses Verzeichnis enthält die Controller für die Web-API, die Anfragen an die Endpunkte steuern.

- [`/Models`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Models): Hier werden die Datenmodelle gespeichert, die die Struktur der in der Anwendung verwendeten Daten definieren.

- [`/Services`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Services): In diesem Ordner finden Sie die Services, die Geschäftslogik implementieren und zwischen Controllern und Datenmodellen vermitteln.

- [`/DTOs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/DTOs): Data Transfer Objects (DTOs) dienen zur Datenübertragung zwischen verschiedenen Schichten der Anwendung.

- [`/Middlewares`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Middlewares): Middleware-Komponenten, die zwischen der Anfrage und der Antwort innerhalb der HTTP-Anforderungspipeline operieren.

- [`/Profiles`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Profiles): Hier sind die AutoMapper-Profile abgelegt, welche die Konfiguration für das Objekt-Mapping definieren.

- [`/Interfaces`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/NoSQLSkiServiceManager/Interfaces): Dieses Verzeichnis beinhaltet Schnittstellen, die für die Definition von Verträgen innerhalb der Anwendung verwendet werden.

- [`/Docs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/Docs): Hier befindet sich die Dokumentation, einschliesslich relevanter Dokumente wie der `Postman Collection V2.1` und anderer Testprotokolle.

Schauen Sie im Ordner [`/Docs`](https://github.com/ArdaBs/NoSQLSkiServiceManager/tree/master/Docs) vorbei, um mehr über die Nutzung der Anwendung und die Konfiguration der Postman Collection zu erfahren.

## Überblick

Der NoSQL Ski Service Manager ist eine Backend-Anwendung, entwickelt mit .NET 8.0, die Serviceaufträge für Skiservices verwaltet. Dieses Projekt nutzt MongoDB als NoSQL-Datenbank und bietet eine RESTful API für die Interaktion mit Frontend-Anwendungen.

## Systemvoraussetzungen

- .NET 8.0 SDK
- MongoDB Server (Version 7.0 oder höher)
- Optional: Postman für API-Tests

## MongoDB Konfiguration

Verwenden Sie die `mongod.conf` Datei, um den MongoDB Server zu konfigurieren. Die Konfigurationsdatei befindet sich in der Regel unter `C:\Program Files\MongoDB\Server\7.0\bin\mongod.cfg`, wenn MongoDB für alle Benutzer installiert wurde.

Es ist mir bewusst, dass man in einer produktiven Umgebung dies nicht auskommentiert.

```yaml
# Sicherheitseinstellungen (auskommentiert für Entwicklungszwecke)
# security:
#   authorization: enabled
```

Stellen Sie sicher, dass der MongoDB-Service läuft und auf Port 27017 hört.

## Projekt-Setup

1. Klonen Sie das Repository in Ihr lokales System.
2. Stellen Sie sicher, dass MongoDB läuft und die Konfigurationen korrekt sind.
3. Öffnen Sie das Projekt in Ihrer bevorzugten IDE (z.B. Visual Studio oder VS Code).
4. Stellen Sie sicher, dass alle NuGet-Pakete installiert sind. Wenn NuGet-Pakete nicht automatisch heruntergeladen werden, führen Sie `dotnet restore` aus.
5. Starten Sie das Backend über den Befehl `dotnet run`. Die erforderlichen Collections und Daten werden automatisch initialisiert.

## Verwendung

Die API kann über Endpunkte wie `https://localhost:7095/api/ServiceOrder` oder `https://localhost:7095/api/Employee` angesprochen werden. Für die vollständige Liste der verfügbaren Endpunkte, sehen Sie sich die Controller im Projekt an.

## Tests

Zur Durchführung von API-Tests können Sie die bereitgestellte Postman-Collection verwenden. Diese Sammlung ist mit Collection-Variablen konfiguriert, was bedeutet, dass Sie die Tests flexibel und wiederholt ausführen können, solange das Backend aktiv ist. Importieren Sie einfach die Collection in Postman und führen Sie die Tests nach Bedarf durch.