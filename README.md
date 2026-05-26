# Smart Greenhouse Manager

Aplicația `Smart Greenhouse Manager` este un proiect desktop WinForms pentru monitorizarea unei sere inteligente. Soluția urmărește colectarea, simularea și afișarea valorilor de la senzori, generarea de alerte la depășirea pragurilor și administrarea senzorilor din interfață.

Documentația de predare generată pentru acest proiect se află în folderul `docs/`.

## 1. Scopul aplicației

Sistemul oferă o platformă software pentru:

- autentificarea utilizatorilor pe roluri;
- monitorizarea senzorilor din seră;
- simularea automată a valorilor de temperatură, umiditate, lumină, umiditate sol și CO2;
- salvarea citirilor în baza de date;
- generarea de alerte automate pe baza pragurilor configurate;
- administrarea senzorilor prin operații CRUD.

## 2. Arhitectura soluției

Proiectul este organizat pe straturi:

- `SmartGreenhouse.UI` - interfața WinForms;
- `SmartGreenhouse.BLL` - logica de business, simulare și alerte;
- `SmartGreenhouse.DAL` - acces la baza de date SQL Server;
- `SmartGreenhouse.Models` - entitățile domeniului;
- `BazaDate` - scriptul SQL de creare și populare a bazei de date.

Modelul arhitectural este unul de tip layered architecture:

1. UI trimite cereri către BLL.
2. BLL aplică regulile de business și apelează DAL.
3. DAL execută interogări SQL în SQL Server.
4. Rezultatele sunt returnate către UI pentru afișare.

## 3. Funcționalități implementate în cod

### 3.1 Autentificare

Fișier relevant: `SmartGreenhouse.UI/Forms/FormLogin.cs`

Funcționalități existente:

- formular de login cu `username` și `password`;
- validare pentru câmpuri obligatorii;
- hash SHA256 pentru parolă înainte de verificare;
- verificare în baza de date prin `UserRepository.Authenticate`;
- actualizare `LastLogin` după autentificare reușită;
- pornirea dashboard-ului după login reușit.

Utilizatori de test definiți în scriptul SQL:

- `admin`
- `operator`

### 3.2 Dashboard principal

Fișier relevant: `SmartGreenhouse.UI/Forms/FormDashboard.cs`

Funcționalități implementate:

- afișarea unui dashboard principal după autentificare;
- afișarea utilizatorului curent;
- afișarea ceasului în timp real;
- încărcarea senzorilor activi din baza de date;
- generarea dinamică de carduri pentru fiecare senzor;
- afișarea valorii curente și a locației pentru fiecare senzor;
- actualizare vizuală a valorilor când simulatorul emite evenimente;
- listă de alerte recente;
- notificări toast pentru alerte;
- efecte vizuale și animații simple în UI.

### 3.3 Simulare valori senzori

Fișier relevant: `SmartGreenhouse.BLL/Services/SensorSimulator.cs`

Funcționalități implementate:

- timer de simulare configurat la 5 secunde implicit;
- generare valori inteligente pentru:
  - temperatură;
  - umiditate aer;
  - umiditate sol;
  - lumină;
  - CO2;
- variații zilnice și sezoniere;
- adăugare zgomot aleator pentru realism;
- salvarea automată a citirilor în `SensorReadings`;
- publicarea evenimentului `NewReadingGenerated`;
- pornire și oprire simulare.

### 3.4 Sistem de alerte

Fișiere relevante:

- `SmartGreenhouse.BLL/Services/AlertService.cs`
- `SmartGreenhouse.Models/Entities/Alert.cs`

Funcționalități implementate:

- verificare praguri de warning și critical;
- creare obiecte `Alert`;
- salvare alerte în baza de date;
- emitere eveniment `AlertGenerated`;
- emitere eveniment separat pentru alerte critice;
- obținere alerte neconfirmate;
- confirmare alertă prin `AcknowledgeAlert`.

### 3.5 Gestionare senzori

Fișiere relevante:

- `SmartGreenhouse.UI/Forms/FormSensorsManagement.cs`
- `SmartGreenhouse.UI/Forms/FormSensorEdit.cs`
- `SmartGreenhouse.DAL/Repositories/SensorRepository.cs`

Funcționalități implementate:

- afișare senzori într-un `DataGridView`;
- adăugare senzor nou;
- editare senzor existent;
- ștergere logică prin `IsActive = 0`;
- reîmprospătare listă;
- validare date introduse;
- configurare praguri min/max, warning și critical;
- asocierea senzorului cu utilizatorul care l-a creat.

### 3.6 Acces la date

Fișier relevant: `SmartGreenhouse.DAL/DatabaseHelper.cs`

Funcționalități implementate:

- deschidere conexiune SQL Server;
- executare interogări cu rezultat `DataTable`;
- executare `INSERT`, `UPDATE`, `DELETE`;
- executare `ExecuteScalar`;
- repository separat pentru utilizatori și senzori.

## 4. Structura bazei de date

Scriptul `BazaDate/Script_Creare.sql` creează următoarele tabele:

1. `Users`
2. `Sensors`
3. `SensorReadings`
4. `Actuators`
5. `ActuatorCommands`
6. `Alerts`

Obiecte suplimentare:

- index pe `SensorReadings(SensorId, Timestamp DESC)`;
- index pe `Alerts(Timestamp DESC)`;
- index pe `Alerts(IsAcknowledged)`;
- view `vw_LatestReadings`.

## 5. Funcționalități pregătite în cod, dar nelegate complet în aplicație

Acesta este punctul important pentru întrebarea „mai sunt chestii neimplementate?”.

### 5.1 Formulare referite, dar inexistente în proiect

În cod apar referințe către clase care nu există în repository:

- `FormSplash` este apelat din `SmartGreenhouse.UI/Program.cs`;
- `FormReports` este apelat din `SmartGreenhouse.UI/Forms/FormDashboard.cs`.

Consecință:

- aplicația nu este completă din punct de vedere al fluxului UI;
- lipsesc ecranul splash și zona de rapoarte.

### 5.2 Configurare UI pe roluri este incompletă

În `FormDashboard.cs` există metoda `ConfigureUIBasedOnRole()`, dar:

- nu este apelată din constructor;
- folosește `menuStrip` și `dgvSensors` ca și cum ar fi membri ai clasei;
- în implementarea actuală acestea nu există ca membri ai clasei în `FormDashboard`.

Consecință:

- restricțiile pe roluri nu sunt finalizate corect;
- rolul `Viewer` există în model, dar nu are un flux UI clar.

### 5.3 Rapoarte și istoric există doar la nivel de DAL/BLL

Există funcții utile:

- `SensorRepository.GetLatestReadings()`;
- `SensorRepository.GetReadingsHistory(...)`;
- `TrendPredictor.PredictNextValue(...)`;
- `TrendPredictor.GetTrend(...)`.

Dar nu există un ecran complet care să le consume.

Consecință:

- partea de rapoarte, istoric și predicții nu este finalizată în interfață.

### 5.4 Actuatoare modelate, dar fără implementare end-to-end

Există:

- entitățile `Actuator` și `ActuatorCommand`;
- tabelele `Actuators` și `ActuatorCommands`.

Nu există însă:

- repository dedicat pentru actuatoare;
- servicii de business pentru comenzi actuatoare;
- formulare UI pentru control actuatoare;
- integrare în dashboard.

Consecință:

- controlul activ al serei este doar planificat, nu implementat complet.

### 5.5 Setări doar ca placeholder

În dashboard, opțiunea `Setări` afișează doar mesajul:

- `Funcționalitate în dezvoltare.`

Consecință:

- meniul există, dar funcția nu este implementată.

### 5.6 Confirmarea alertelor nu are interfață

`AlertService` oferă metode pentru:

- citire alerte neconfirmate;
- confirmare alertă.

Dar în UI:

- nu există formular sau buton pentru `AcknowledgeAlert`.

Consecință:

- partea de management complet al alertelor nu este gata.

### 5.7 Graficul din dashboard este demonstrativ

Graficul desenat în `PnlCharts_Paint`:

- generează puncte random;
- nu folosește istoricul real din baza de date;
- nu afișează trendul calculat de `TrendPredictor`.

Consecință:

- componenta grafică este mai mult demonstrativă decât analitică.

### 5.8 Inconsistențe de configurare tehnică

Au fost observate și câteva nealiniări tehnice:

- `App.config` indică `.NET Framework 4.7.2`, dar proiectele sunt pe `net8.0` și `net8.0-windows`;
- `DatabaseHelper` folosește connection string hardcodat, deși există și unul în `App.config`;
- fișierul `SQL.txt` conține altă instanță SQL (`SQLEXPRESS01`) decât codul (`SQLEXPRESS`).

Consecință:

- configurarea locală poate necesita ajustări înainte de rulare stabilă.

## 6. Fluxul principal al aplicației

1. Utilizatorul pornește aplicația.
2. Se afișează formularul de login.
3. `UserRepository` verifică utilizatorul în baza de date.
4. La succes, se deschide dashboard-ul.
5. Dashboard-ul încarcă senzorii activi.
6. Simulatorul pornește și generează periodic citiri.
7. Citirile sunt salvate în `SensorReadings`.
8. `AlertService` verifică pragurile și salvează alerte.
9. UI primește evenimentele și actualizează cardurile și lista de alerte.
10. Administratorul poate intra în gestionarea senzorilor pentru CRUD.

## 7. Tehnologii folosite

- C#
- .NET 8
- WinForms
- SQL Server
- `Microsoft.Data.SqlClient`

## 8. Cum se pregătește proiectul pentru rulare

### 8.1 Baza de date

1. Deschide SQL Server Management Studio.
2. Rulează scriptul `BazaDate/Script_Creare.sql`.
3. Verifică existența bazei `SmartGreenhouseDB`.

### 8.2 Connection string

În forma actuală, proiectul folosește connection string-ul hardcodat din:

- `SmartGreenhouse.DAL/DatabaseHelper.cs`

Valoarea curentă:

`Data Source=localhost\\SQLEXPRESS;Initial Catalog=SmartGreenhouseDB;Integrated Security=True`

Dacă instanța locală este diferită, trebuie modificată această valoare.

### 8.3 Pornire aplicație

Entry point:

- `SmartGreenhouse.UI/Program.cs`

Observație:

- pentru rulare completă trebuie rezolvate și formularele lipsă menționate mai sus.

## 9. Artefacte de documentație generate

În folderul `docs/` au fost generate:

- `docs/uml/01-use-case.puml`
- `docs/uml/02-class-diagram.puml`
- `docs/uml/03-sequence-alert-flow.puml`
- `docs/ER_DIAGRAM.md`
- `docs/TEST_PLAN.md`
- `docs/PRESENTATION_SLIDES.md`

## 10. Concluzie

Proiectul are o bază bună pentru monitorizarea unei sere inteligente și include deja părțile esențiale de autentificare, CRUD senzori, simulare, alerte și afișare dashboard. În același timp, există mai multe funcționalități începute sau modelate, dar nefinalizate: splash screen, rapoarte, control actuatoare, confirmare alerte din UI, restricții complete pe roluri și integrarea reală a predicțiilor în interfață.

Pentru prezentare și predare, proiectul poate fi descris corect ca un sistem funcțional de monitorizare cu extensii de control și analiză aflate în lucru.
#   S m a r t G r e e n h o u s e  
 