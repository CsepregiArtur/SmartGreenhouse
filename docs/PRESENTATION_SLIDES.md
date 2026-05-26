# Documentație Scrisă Pentru Prezentare

## Slide 1 - Titlu

**Smart Greenhouse Manager**

- Nume echipă: `Csepregi Artur`
- Facultatea: UTCN
- Asistent: `Csepregi Artur`
- Anul: 2025-2026
- Membri echipă: `Csepregi Artur`

Sugestie de subtitlu:

`Sistem desktop pentru monitorizarea și administrarea unei sere inteligente`

## Slide 2 - Introducere

Text propus:

`Smart Greenhouse Manager este o aplicație desktop realizată în C#, WinForms și SQL Server, concepută pentru monitorizarea în timp real a parametrilor unei sere inteligente. Sistemul colectează și afișează valori de la senzori, generează alerte automate și oferă funcții de administrare pentru infrastructura de monitorizare.`

Problema rezolvată:

- monitorizare centralizată a serei;
- observarea rapidă a variațiilor de temperatură, umiditate, lumină și CO2;
- intervenție rapidă atunci când valorile depășesc pragurile permise.

Utilizatori:

- administratori;
- operatori.

## Slide 3 - Arhitectură

Titlu recomandat:

`Arhitectură pe 3 niveluri`

Conținut:

- nivel prezentare: `SmartGreenhouse.UI` - formulare WinForms;
- nivel business logic: `SmartGreenhouse.BLL` - simulare, reguli, alerte;
- nivel acces date: `SmartGreenhouse.DAL` - repository-uri și SQL;
- model domeniu: `SmartGreenhouse.Models`.

Text scurt:

`Am ales o arhitectură stratificată pentru a separa interfața de logică și de accesul la date. Astfel, codul este mai ușor de întreținut, extins și testat.`

Imagine recomandată:

- screenshot cu structura soluției din Solution Explorer.

## Slide 4 - Diagrame UML

Elemente de pus:

- imagine `Use Case`
- imagine `Class Diagram`

Explicații scurte:

- diagrama Use Case arată ce pot face administratorii, operatorii și utilizatorii de tip viewer;
- diagrama de clase evidențiază entitățile principale: `User`, `Sensor`, `SensorReading`, `Alert`, `Actuator`, `ActuatorCommand`;
- relațiile importante sunt între senzori, citiri, alerte și utilizatori.

Fișiere pregătite:

- `docs/uml/01-use-case.puml`
- `docs/uml/02-class-diagram.puml`

## Slide 5 - Baza de date

Text propus:

`Baza de date relațională a fost realizată în SQL Server și conține tabele pentru utilizatori, senzori, citiri, actuatoare, comenzi și alerte. Structura urmărește păstrarea istoricului măsurătorilor și susține extinderea aplicației spre control automat al serei.`

Tabele:

1. `Users`
2. `Sensors`
3. `SensorReadings`
4. `Actuators`
5. `ActuatorCommands`
6. `Alerts`

Relații importante:

- un utilizator poate crea mai mulți senzori;
- un senzor are mai multe citiri;
- un senzor poate genera mai multe alerte;
- un actuator poate primi mai multe comenzi.

Imagine recomandată:

- export din `docs/ER_DIAGRAM.md`.

## Slide 6 - Interfețe grafice

Capturi recomandate:

- formular login;
- dashboard principal;
- gestionare senzori;
- formular adăugare/editare senzor.

Explicații:

- `Login`: autentificare utilizator și validare credențiale;
- `Dashboard`: vizualizare valori în timp real și alerte;
- `Gestionare senzori`: CRUD pentru senzorii activi;
- `Editare senzor`: setarea pragurilor minime, maxime, warning și critical.

## Slide 7 - Funcționalități principale

- autentificare pe roluri;
- CRUD senzori;
- dashboard actualizat în timp real;
- simulare inteligentă a valorilor;
- generare și afișare alerte;
- stocare istoric citiri în SQL Server.

Text scurt:

`Aplicația pune accent pe monitorizarea continuă a serei și pe reacția rapidă la depășirea pragurilor critice. Administratorul poate gestiona senzorii, iar operatorul poate urmări valorile și alertele din dashboard.`

Notă sinceră pentru prezentare:

- partea de rapoarte și control actuatoare este pregătită parțial, dar nu este finalizată complet în interfață.

## Slide 8 - Delegate și Evenimente

Titlu recomandat:

`Delegate și Evenimente - partea WOW`

Mesaj:

`Am folosit delegate și evenimente pentru a separa logică de business de interfața grafică. Când simulatorul generează o nouă citire, dashboard-ul este notificat automat și actualizează valorile vizuale fără interogări repetate în UI.`

Exemplu de cod:

```csharp
public delegate void NewReadingEventHandler(object sender, NewReadingEventArgs e);
public event NewReadingEventHandler NewReadingGenerated;

_simulator.NewReadingGenerated += OnNewReadingGenerated;
_alertService.AlertGenerated += OnAlertGenerated;
```

Flux evenimente:

1. simulatorul generează o valoare;
2. citirea este salvată în baza de date;
3. serviciul de alerte verifică pragurile;
4. UI primește evenimentul și actualizează dashboard-ul.

Imagine recomandată:

- `docs/uml/03-sequence-alert-flow.puml`

## Slide 9 - Testare

Tabel sumar recomandat:

| Zonă | Nr. teste | Reușite | Observații |
|---|---:|---:|---|
| Login | 4 | 4 | funcțional |
| Dashboard și simulare | 6 | 6 | funcțional |
| CRUD senzori | 5 | 5 | funcțional |
| Funcții incomplete | 5 | 2 | rapoarte, splash, confirmare alerte |

Rezultate:

- total teste definite: 20
- trecute: 17
- neimplementate sau eșuate: 3
- procent reușită: aproximativ `85%`

Bug-uri găsite:

- lipsă `FormSplash`;
- lipsă `FormReports`;
- control pe roluri incomplet legat în dashboard.

Fișier suport:

- `docs/TEST_PLAN.md`

## Slide 10 - Demo

Conținut recomandat:

- variantă 1: link către video demo;
- variantă 2: demo live din aplicație.

Scenariu demo:

1. login cu `admin`;
2. deschidere dashboard;
3. observare actualizare valori senzori;
4. deschidere gestionare senzori;
5. adăugare sau editare senzor;
6. observare alerte în timp real.

Text scurt:

`În cadrul demo-ului prezentăm fluxul principal complet: autentificare, dashboard, simulare, CRUD senzori și alerte.`

## Slide 11 - Concluzii

Ce am învățat:

- organizarea unei aplicații pe straturi;
- modelarea unei baze de date relaționale;
- lucrul cu WinForms și evenimente;
- conectarea unei aplicații C# la SQL Server;
- structurarea unei aplicații reale de monitorizare.

Provocări:

- sincronizarea între UI și logică;
- proiectarea tabelelor și relațiilor;
- menținerea unei interfețe clare;
- integrarea tuturor componentelor într-un flux coerent.

Ce am îmbunătăți:

- formulare pentru rapoarte și actuatoare;
- grafice bazate pe date reale;
- confirmarea alertelor din UI;
- configurare centralizată pentru connection string și setări.

## Slide 12 - Mulțumiri + Întrebări

Text propus:

`Vă mulțumim pentru atenție!`

`Întrebări?`

Subsol opțional:

- nume echipă;
- facultate;
- anul universitar;
- date de contact.
