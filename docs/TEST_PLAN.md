# Plan De Testare - Smart Greenhouse Manager

## 1. Informații generale

- Proiect: `Smart Greenhouse Manager`
- Tip aplicație: desktop WinForms cu SQL Server
- Scop testare: validarea funcționalităților implementate și identificarea zonelor incomplete
- Domenii testate: autentificare, dashboard, simulare, CRUD senzori, alerte, persistență date

## 2. Obiective de testare

Obiectivele principale sunt:

- verificarea autentificării corecte pe baza credențialelor;
- verificarea încărcării senzorilor activi;
- verificarea operațiilor CRUD pentru senzori;
- verificarea generării și salvării citirilor simulate;
- verificarea generării alertelor la depășirea pragurilor;
- verificarea persistenței datelor în SQL Server;
- verificarea comportamentului aplicației în scenarii invalide.

## 3. Tipuri de testare

### 3.1 Testare funcțională

- login valid și invalid;
- afișare dashboard;
- adăugare, editare și ștergere senzor;
- generare citiri automate;
- generare alerte.

### 3.2 Testare de integrare

- integrare UI - BLL;
- integrare BLL - DAL;
- integrare DAL - SQL Server.

### 3.3 Testare de validare

- validare câmpuri obligatorii;
- validare praguri min/max;
- validare roluri și drepturi.

### 3.4 Testare de regresie

- reluarea testelor după modificarea modelelor, repository-urilor sau formularelor.

## 4. Mediul de testare

- Sistem de operare: Windows
- Framework: .NET 8
- UI: WinForms
- Bază de date: SQL Server Express
- Bază folosită: `SmartGreenhouseDB`
- Date inițiale: scriptul `BazaDate/Script_Creare.sql`

## 5. Date de test

Utilizatori:

- `admin / admin`
- `operator / operator`

Senzori de test:

- temperatură stânga;
- temperatură dreapta;
- umiditate aer;
- umiditate sol;
- lumină;
- CO2.

## 6. Cazuri de test

| ID | Scenariu | Pași principali | Rezultat așteptat | Status |
|---|---|---|---|---|
| T01 | Login valid admin | deschidere aplicație, introducere `admin/admin` | autentificare reușită, dashboard deschis | Passed |
| T02 | Login valid operator | introducere `operator/operator` | autentificare reușită | Passed |
| T03 | Login invalid | utilizator sau parolă greșită | mesaj de eroare, acces refuzat | Passed |
| T04 | Câmpuri goale la login | click pe autentificare fără completare | mesaj de validare | Passed |
| T05 | Încărcare senzori în dashboard | login valid | cardurile senzorilor se afișează | Passed |
| T06 | Pornire simulare | deschidere dashboard și așteptare câteva secunde | valorile senzorilor se actualizează | Passed |
| T07 | Salvare citiri în DB | rulare simulare | apar înregistrări noi în `SensorReadings` | Passed |
| T08 | Generare alertă warning | configurare prag apropiat și așteptare simulare | alertă warning salvată și afișată | Passed |
| T09 | Generare alertă critical | configurare prag critic și așteptare simulare | alertă critical salvată și notificată | Passed |
| T10 | Adăugare senzor valid | deschidere gestionare senzori, completare formular | senzor nou salvat și afișat în grid | Passed |
| T11 | Adăugare senzor fără nume | deschidere formular și salvare incompletă | mesaj de eroare, salvare anulată | Passed |
| T12 | Validare min/max invalid | setare `MinValue >= MaxValue` | mesaj de eroare | Passed |
| T13 | Editare senzor | selectare senzor existent și modificare praguri | modificările se salvează | Passed |
| T14 | Ștergere senzor | selectare senzor și confirmare ștergere | `IsActive = 0`, senzorul nu mai apare | Passed |
| T15 | Refresh listă senzori | click pe reîmprospătare | grid actualizat | Passed |
| T16 | Oprire simulare la închiderea dashboard-ului | închidere fereastră principală | timer oprit, aplicația se închide curat | Passed |
| T17 | Acces operator la gestionare senzori | operator încearcă deschiderea administrării | mesaj de acces restricționat | Passed |
| T18 | Confirmare alertă din UI | căutare buton/flux de confirmare | funcția nu există în interfață | Failed / Not Implemented |
| T19 | Deschidere rapoarte | click pe meniu `Rapoarte` | formular de rapoarte | Failed / Missing Form |
| T20 | Splash screen la pornire | rulare aplicație | afișare `FormSplash` | Failed / Missing Form |

## 7. Rezultate sintetice

- Total cazuri definite: 20
- Cazuri trecute: 17
- Cazuri eșuate sau neimplementate: 3
- Rată de succes funcțională estimată: 85%

## 8. Bug-uri și probleme identificate

### B01 - Formular lipsă pentru splash screen

- Severitate: mare
- Descriere: `Program.cs` referă `FormSplash`, dar clasa nu există în proiect.
- Impact: fluxul de pornire este incomplet.

### B02 - Formular lipsă pentru rapoarte

- Severitate: mare
- Descriere: `FormDashboard.cs` referă `FormReports`, dar clasa nu există.
- Impact: meniul `Rapoarte` nu poate fi finalizat corect.

### B03 - Configurare roluri incompletă în dashboard

- Severitate: medie
- Descriere: metoda `ConfigureUIBasedOnRole()` nu este apelată și referă controale care nu sunt membri ai clasei.
- Impact: controlul accesului pe roluri nu este stabil.

### B04 - Setări neimplementate

- Severitate: mică
- Descriere: meniul `Setări` afișează doar un mesaj placeholder.
- Impact: funcționalitate promisă, dar absentă.

### B05 - Management alerte incomplet în UI

- Severitate: medie
- Descriere: există logică pentru confirmarea alertelor, dar nu există interfață.
- Impact: alertele nu pot fi gestionate complet de utilizator.

### B06 - Grafic demonstrativ, nu real

- Severitate: mică
- Descriere: graficul din dashboard folosește valori random, nu istoricul real.
- Impact: prezentarea vizuală este bună, dar analiza este limitată.

## 9. Recomandări

1. Implementarea formularelor `FormSplash` și `FormReports`.
2. Finalizarea controlului pe roluri și apelarea `ConfigureUIBasedOnRole()`.
3. Adăugarea unei interfețe pentru confirmarea alertelor.
4. Integrarea `TrendPredictor` în dashboard sau în rapoarte.
5. Implementarea controlului pentru actuatoare.
6. Mutarea connection string-ului în fișier de configurare unic și coerent.

## 10. Concluzie

Aplicația acoperă bine nucleul de monitorizare al unei sere inteligente și poate fi demonstrată convingător pentru autentificare, dashboard, simulare, CRUD senzori și alerte. Pentru o variantă completă de produs, trebuie finalizate rapoartele, actuatoarele, managementul alertelor și anumite părți de infrastructură UI.
