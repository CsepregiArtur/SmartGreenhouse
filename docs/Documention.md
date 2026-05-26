**Documentație**

# Copertă

## Proiect Informatica Industrială

# Smart Greenhouse Manager

Facultatea: UTCN  
Asistent: Conf.dr.ing. Teodora Sanislav  
Anul: 2026

## Echipa

- Manager: [Nume]
- Proiectant BD: [Nume]
- Programator 1: [Nume]
- Programator 2: [Nume]
- Tester: [Nume]

---

# Cuprins

1. Introducere  
2. Arhitectura aplicației  
3. Diagrame UML  
4. Baza de date  
5. Interfețe grafice  
6. Funcționalități implementate  
7. Testare  
8. Concluzii  

---

# 1. Introducere

## 1.1 Contextul proiectului

Proiectul `Smart Greenhouse Manager` a fost realizat în cadrul disciplinei Informatica Industrială și are ca obiectiv dezvoltarea unei aplicații software pentru monitorizarea unei sere inteligente. Într-un astfel de sistem, informațiile oferite de senzori sunt esențiale pentru menținerea unui mediu optim de creștere pentru plante. Temperatura, umiditatea, lumina, umiditatea solului și concentrația de CO2 trebuie urmărite constant pentru a evita pierderile și pentru a îmbunătăți randamentul culturilor.

În practică, monitorizarea manuală a acestor parametri este ineficientă, consumă timp și poate duce la întârzieri în luarea deciziilor. De aceea, aplicația propusă urmărește centralizarea informațiilor într-o interfață grafică unică, ușor de utilizat, care să permită personalului să vadă rapid starea serei și să identifice anomaliile.

## 1.2 Scopul aplicației

Scopul principal al aplicației este realizarea unui sistem software desktop care:

- permite autentificarea utilizatorilor;
- afișează informații despre senzorii instalați în seră;
- simulează citiri periodice pentru senzori;
- salvează aceste citiri în baza de date;
- generează alerte atunci când valorile ies din intervalele normale;
- oferă funcții de administrare pentru senzori.

## 1.3 Problema rezolvată

Aplicația rezolvă problema monitorizării centralizate a unei sere inteligente. În loc ca operatorul sau administratorul să verifice manual fiecare dispozitiv, toate informațiile importante sunt colectate și prezentate într-un singur dashboard. Sistemul ajută la detectarea rapidă a depășirii pragurilor critice și reduce timpul de reacție în situații problematice.

## 1.4 Categoria de utilizatori

Sistemul este gândit pentru următoarele categorii de utilizatori:

- `Administrator` - gestionează senzorii și are acces la funcțiile de administrare;
- `Operator` - monitorizează dashboard-ul și alertele;
- `Viewer` - rol definit în modelul aplicației, dar neintegrat complet în interfață.

## 1.5 Tehnologii utilizate

Proiectul a fost dezvoltat folosind:

- limbajul `C#`;
- platforma `.NET 8`;
- `Windows Forms` pentru interfața grafică;
- `SQL Server` pentru baza de date;
- `Microsoft.Data.SqlClient` pentru conectarea la baza de date.

Aceste tehnologii au fost alese deoarece permit realizarea unei aplicații desktop robuste, ușor de împărțit pe straturi și potrivite pentru aplicații industriale sau educaționale.

---

# 2. Arhitectura aplicației

## 2.1 Prezentare generală

Aplicația urmează o arhitectură stratificată, ceea ce înseamnă că responsabilitățile sunt împărțite pe mai multe niveluri. Această abordare face codul mai clar, mai ușor de întreținut și mai simplu de extins.

Structura soluției este formată din următoarele proiecte:

- `SmartGreenhouse.UI`
- `SmartGreenhouse.BLL`
- `SmartGreenhouse.DAL`
- `SmartGreenhouse.Models`
- `BazaDate`

## 2.2 Nivelul de prezentare - UI

Proiectul `SmartGreenhouse.UI` conține interfața grafică realizată în WinForms. Aici se află formularele prin care utilizatorul interacționează cu aplicația.

Formulare identificate în proiect:

- `FormLogin` - autentificarea utilizatorului;
- `FormDashboard` - dashboard principal cu senzori și alerte;
- `FormSensorsManagement` - administrarea senzorilor;
- `FormSensorEdit` - formular pentru adăugare și editare senzor.

Acest strat are rolul de a:

- prelua date de la utilizator;
- afișa informațiile primite din straturile inferioare;
- reacționa la evenimentele generate de logică;
- oferi o experiență vizuală modernă, cu efecte grafice și feedback vizual.

## 2.3 Nivelul de business logic - BLL

Proiectul `SmartGreenhouse.BLL` conține logica de business a aplicației. Aici sunt implementate regulile după care funcționează sistemul.

Clase importante:

- `SensorSimulator`
- `AlertService`
- `TrendPredictor`

Aceste clase se ocupă de:

- generarea de valori simulate pentru senzori;
- analiza citirilor în raport cu pragurile configurate;
- generarea alertelor;
- oferirea unei baze pentru predicții și analiză de trend.

## 2.4 Nivelul de acces la date - DAL

Proiectul `SmartGreenhouse.DAL` gestionează comunicarea cu baza de date SQL Server.

Componente principale:

- `DatabaseHelper`
- `UserRepository`
- `SensorRepository`

Responsabilități:

- deschiderea conexiunii la SQL Server;
- executarea interogărilor SQL;
- inserarea, actualizarea și ștergerea datelor;
- maparea rezultatelor din tabele către obiecte din model.

## 2.5 Nivelul de model - Models

Proiectul `SmartGreenhouse.Models` definește entitățile de bază ale sistemului:

- `User`
- `Sensor`
- `SensorReading`
- `Alert`
- `Actuator`
- `ActuatorCommand`

Aceste clase descriu datele manipulate de sistem și relațiile dintre ele.

## 2.6 Fluxul de lucru între straturi

Fluxul principal al aplicației este următorul:

1. utilizatorul pornește aplicația;
2. UI afișează formularul de login;
3. `UserRepository` verifică utilizatorul în baza de date;
4. după autentificare se deschide dashboard-ul;
5. dashboard-ul încarcă senzorii activi;
6. `SensorSimulator` generează periodic citiri;
7. citirile sunt salvate de `SensorRepository`;
8. `AlertService` verifică depășirea pragurilor și creează alerte;
9. UI primește evenimente și actualizează cardurile și lista de alerte.

## 2.7 Avantajele arhitecturii alese

Această arhitectură oferă mai multe beneficii:

- separare clară a responsabilităților;
- cod mai ușor de extins;
- întreținere mai simplă;
- reutilizare mai bună a componentelor;
- posibilitatea de a adăuga funcționalități noi fără rescriere majoră.

---

# 3. Diagrame UML

## 3.1 Rolul diagramelor UML

Diagramele UML ajută la descrierea sistemului din mai multe perspective. Ele sunt utile atât pentru proiectare, cât și pentru prezentarea structurii aplicației.

Pentru acest proiect au fost generate trei diagrame:

1. diagramă Use Case;
2. diagramă de clase;
3. diagramă de secvență pentru fluxul de alerte.

Fișierele pregătite pentru acest proiect sunt:

- [01-use-case.puml](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/docs/uml/01-use-case.puml)
- [02-class-diagram.puml](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/docs/uml/02-class-diagram.puml)
- [03-sequence-alert-flow.puml](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/docs/uml/03-sequence-alert-flow.puml)

## 3.2 Diagrama Use Case

Diagrama Use Case descrie interacțiunea dintre utilizatori și sistem. Actorii principali sunt:

- `Administrator`
- `Operator`
- `Viewer`

Use case-uri principale:

- autentificare;
- vizualizare dashboard;
- monitorizare valori senzori;
- vizualizare alerte;
- gestionare senzori;
- adăugare, editare și ștergere senzor;
- pornire simulare;
- generare automată de alerte;
- vizualizare rapoarte.

Această diagramă evidențiază faptul că administratorul are cele mai multe drepturi, în timp ce operatorul este orientat în principal spre monitorizare.

## 3.3 Diagrama de clase

Diagrama de clase prezintă structura logică a aplicației și relațiile dintre obiectele principale.

Clasele centrale din model:

- `User`
- `Sensor`
- `SensorReading`
- `Alert`
- `Actuator`
- `ActuatorCommand`

Clase de infrastructură:

- `DatabaseHelper`
- `UserRepository`
- `SensorRepository`
- `SensorSimulator`
- `AlertService`
- `TrendPredictor`

Clase din interfață:

- `FormLogin`
- `FormDashboard`
- `FormSensorsManagement`
- `FormSensorEdit`

Relații importante:

- un utilizator poate crea mai mulți senzori;
- un senzor are mai multe citiri;
- un senzor poate genera mai multe alerte;
- un actuator poate primi mai multe comenzi;
- dashboard-ul depinde de simulator, servicii și repository-uri.

## 3.4 Diagrama de secvență

Diagrama de secvență descrie pașii prin care o citire simulată ajunge să fie afișată în dashboard și, dacă este cazul, să genereze o alertă.

Fluxul ilustrat:

1. utilizatorul deschide dashboard-ul;
2. dashboard-ul pornește simularea;
3. simulatorul citește lista de senzori din baza de date;
4. pentru fiecare senzor se generează o valoare;
5. citirea este salvată în `SensorReadings`;
6. `AlertService` verifică pragurile;
7. dacă este depășit un prag, alerta este salvată;
8. dashboard-ul primește evenimente și actualizează interfața.

Această diagramă evidențiază folosirea delegatelor și a evenimentelor pentru comunicarea dintre logică și interfață.

---

# 4. Baza de date

## 4.1 Rolul bazei de date

Baza de date are rolul de a stoca informațiile esențiale ale sistemului:

- utilizatori;
- senzori;
- citiri de la senzori;
- alerte;
- actuatoare;
- comenzi trimise actuatoarelor.

Baza de date utilizată este `SmartGreenhouseDB`, iar scriptul de creare se află în [Script_Creare.sql](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/BazaDate/Script_Creare.sql).

## 4.2 Tabelele bazei de date

### 4.2.1 Tabela Users

Reține informații despre utilizatorii aplicației:

- `UserId`
- `Username`
- `PasswordHash`
- `Email`
- `Role`
- `CreatedAt`
- `LastLogin`
- `IsActive`

Rolul acestei tabele este să permită autentificarea și diferențierea accesului în funcție de rol.

### 4.2.2 Tabela Sensors

Conține toți senzorii gestionați în aplicație:

- `SensorId`
- `Name`
- `Type`
- `Unit`
- `Location`
- `MinValue`
- `MaxValue`
- `WarningLow`
- `WarningHigh`
- `CriticalLow`
- `CriticalHigh`
- `IsActive`
- `InstalledDate`
- `CreatedByUserId`

Această tabelă este esențială pentru definirea parametrilor monitorizați și a pragurilor de alertare.

### 4.2.3 Tabela SensorReadings

Stochează istoricul citirilor de la senzori:

- `ReadingId`
- `SensorId`
- `Timestamp`
- `Value`
- `Quality`

Această tabelă permite analiza evoluției în timp a valorilor măsurate.

### 4.2.4 Tabela Actuators

Tabela există în schema bazei de date pentru extinderea aplicației spre control activ:

- `ActuatorId`
- `Name`
- `Type`
- `Location`
- `Status`
- `IsAutoMode`
- `InstalledDate`
- `CreatedByUserId`

### 4.2.5 Tabela ActuatorCommands

Înregistrează comenzile trimise către actuatoare:

- `CommandId`
- `ActuatorId`
- `IssuedByUserId`
- `Timestamp`
- `Command`
- `DurationSeconds`
- `Executed`

### 4.2.6 Tabela Alerts

Stochează alertele generate de sistem:

- `AlertId`
- `SensorId`
- `ActuatorId`
- `Timestamp`
- `Level`
- `Message`
- `Value`
- `IsAcknowledged`
- `AcknowledgedByUserId`
- `AcknowledgedAt`

## 4.3 Relații importante

Relațiile principale dintre tabele sunt:

- `Users -> Sensors` prin `CreatedByUserId`;
- `Sensors -> SensorReadings` prin `SensorId`;
- `Sensors -> Alerts` prin `SensorId`;
- `Users -> Alerts` prin `AcknowledgedByUserId`;
- `Users -> Actuators` prin `CreatedByUserId`;
- `Actuators -> ActuatorCommands` prin `ActuatorId`;
- `Users -> ActuatorCommands` prin `IssuedByUserId`.

## 4.4 Elemente suplimentare din baza de date

Scriptul creează și:

- index pe istoricul citirilor pentru interogări mai rapide;
- index pe alerte;
- view `vw_LatestReadings` pentru ultimele citiri.

Aceste elemente îmbunătățesc performanța și susțin extinderea aplicației.

## 4.5 Date de test

Scriptul populează baza de date cu:

- doi utilizatori de test: `admin` și `operator`;
- șase senzori de test;
- cinci actuatoare de test;
- un set de citiri inițiale pentru senzori.

Aceste date permit testarea rapidă a aplicației fără introducere manuală extinsă.

## 4.6 Observații privind implementarea

Deși baza de date include și actuatoare, comenzi și suport pentru confirmarea alertelor, aceste componente nu sunt integrate complet în interfața grafică. Din acest motiv, schema bazei de date este mai avansată decât funcționalitatea complet expusă utilizatorului.

---

# 5. Interfețe grafice

## 5.1 Prezentare generală

Interfața grafică a fost dezvoltată în WinForms și urmărește să ofere o experiență vizuală modernă, clară și ușor de utilizat. Aplicația folosește culori, efecte vizuale și elemente grafice pentru a face interacțiunea mai plăcută și pentru a evidenția rapid informațiile importante.

## 5.2 Formularul de login

Formularul `FormLogin` este primul ecran relevant al aplicației și permite autentificarea utilizatorului.

Caracteristici:

- câmp pentru username;
- câmp pentru parolă;
- validare de câmpuri obligatorii;
- buton de autentificare;
- afișarea mesajelor de stare;
- efecte grafice precum gradient și hover.

Rolul acestui formular este de a controla accesul la sistem și de a identifica utilizatorul conectat.

## 5.3 Dashboard-ul principal

Formularul `FormDashboard` reprezintă centrul aplicației.

Elemente principale din dashboard:

- mesaj de bun venit;
- afișarea utilizatorului curent;
- ceas actualizat în timp real;
- carduri pentru fiecare senzor;
- zonă pentru alerte recente;
- meniu principal;
- panou de grafice.

Dashboard-ul primește evenimente de la simulator și de la serviciul de alerte, astfel încât valorile și notificările sunt actualizate automat.

## 5.4 Formularul de gestionare a senzorilor

Formularul `FormSensorsManagement` este dedicat administrării senzorilor și folosește un `DataGridView`.

Funcții disponibile:

- afișarea tuturor senzorilor activi;
- adăugare senzor;
- editare senzor;
- ștergere logică;
- reîmprospătare listă.

Acesta este un formular important pentru administrator, deoarece permite configurarea infrastructurii de monitorizare.

## 5.5 Formularul de editare a senzorului

Formularul `FormSensorEdit` este folosit atât pentru adăugarea, cât și pentru editarea unui senzor.

Câmpuri configurabile:

- nume senzor;
- tip senzor;
- unitate de măsură;
- locație;
- valoare minimă;
- valoare maximă;
- praguri de warning;
- praguri critice.

Formularul include validări pentru a evita introducerea de date invalide.

## 5.6 Observații despre interfața grafică

Interfața existentă acoperă bine zona de monitorizare și administrare senzori. Totuși, au fost identificate și unele componente lipsă sau incomplete:

- `FormSplash` este referit în [Program.cs](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/SmartGreenhouse.UI/Program.cs);
- `FormReports` este referit în [FormDashboard.cs](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/SmartGreenhouse.UI/Forms/FormDashboard.cs);
- funcția `Setări` este doar placeholder;
- confirmarea alertelor nu are formular dedicat.

---

# 6. Funcționalități implementate

## 6.1 Autentificare

Autentificarea este implementată prin formularul `FormLogin` și `UserRepository`.

Pași realizați:

- utilizatorul introduce username și parolă;
- parola este transformată în hash SHA256;
- repository-ul verifică datele în tabela `Users`;
- dacă autentificarea reușește, se actualizează `LastLogin`;
- se deschide dashboard-ul principal.

## 6.2 Dashboard cu monitorizare în timp real

Dashboard-ul este una dintre funcțiile principale ale aplicației.

Capabilități:

- încarcă senzorii activi;
- creează dinamic carduri pentru fiecare senzor;
- afișează valorile curente și locația;
- actualizează datele automat;
- afișează alertele în timp real;
- afișează notificări toast.

## 6.3 Simulare inteligentă a valorilor

Clasa `SensorSimulator` generează automat citiri pentru fiecare senzor.

Tipuri de simulare:

- temperatură;
- umiditate aer;
- umiditate sol;
- lumină;
- CO2.

Simularea ține cont de:

- ora din zi;
- variații zilnice;
- variații sezoniere;
- zgomot aleator pentru realism.

## 6.4 Salvarea citirilor în baza de date

Fiecare valoare simulată este stocată în `SensorReadings` prin `SensorRepository.AddReading`.

Aceasta asigură:

- păstrarea istoricului;
- posibilitatea unor rapoarte viitoare;
- suport pentru analiză și predicții.

## 6.5 Generarea de alerte

Clasa `AlertService` analizează citirile și compară valorile cu pragurile definite.

Situații tratate:

- depășirea pragului critic superior;
- depășirea pragului critic inferior;
- depășirea pragului de avertizare superior;
- depășirea pragului de avertizare inferior.

În aceste cazuri:

- se creează obiecte `Alert`;
- alertele sunt salvate în baza de date;
- UI este notificat;
- utilizatorul primește feedback vizual.

## 6.6 CRUD senzori

Administrarea senzorilor este implementată prin `FormSensorsManagement`, `FormSensorEdit` și `SensorRepository`.

Operații disponibile:

- creare senzor;
- citire listă senzori;
- actualizare senzor;
- ștergere logică.

## 6.7 Funcții existente doar parțial

Pe lângă funcțiile complet implementate, proiectul conține și funcționalități pregătite doar parțial:

- rapoarte;
- trend prediction;
- controlul actuatoarelor;
- confirmarea alertelor;
- gestionarea completă a rolurilor;
- splash screen la pornire.

## 6.8 Elemente de tip WOW

Aplicația include și câteva elemente mai spectaculoase pentru prezentare:

- efecte grafice în login și dashboard;
- simulare realistă pentru senzori;
- actualizare în timp real a cardurilor;
- folosirea delegatelor și evenimentelor;
- notificări toast pentru alerte.

---

# 7. Testare

## 7.1 Obiectivul testării

Scopul testării a fost verificarea funcționării corecte a principalelor module și identificarea zonelor neimplementate sau instabile.

Au fost urmărite:

- autentificarea;
- monitorizarea valorilor;
- operațiile CRUD pentru senzori;
- simularea citirilor;
- generarea alertelor;
- persistența în baza de date;
- comportamentul în cazuri invalide.

## 7.2 Tipuri de testare aplicate

Au fost luate în considerare:

- testare funcțională;
- testare de integrare;
- testare de validare;
- testare de regresie.

## 7.3 Exemple de cazuri de test

Exemple reprezentative:

- login corect cu `admin`;
- login incorect;
- câmpuri goale la autentificare;
- încărcare dashboard după login;
- actualizare valori în timp real;
- generare alertă warning;
- generare alertă critical;
- adăugare senzor valid;
- validare `MinValue < MaxValue`;
- editare senzor;
- ștergere logică senzor.

## 7.4 Rezultate sintetice

Conform planului de testare generat:

- total cazuri definite: `20`;
- cazuri trecute: `17`;
- cazuri neimplementate sau eșuate: `3`;
- procent estimat de reușită: `85%`.

## 7.5 Probleme identificate

Principalele probleme observate:

- lipsa formularului `FormSplash`;
- lipsa formularului `FormReports`;
- gestionare incompletă a rolurilor în dashboard;
- lipsa interfeței pentru confirmarea alertelor;
- grafice demonstrative, nelegate de date reale.

## 7.6 Concluzii privind testarea

Rezultatele arată că nucleul funcțional al aplicației este stabil și poate fi demonstrat: login, dashboard, simulare, CRUD senzori și alerte. Totuși, anumite extensii planificate nu sunt încă finalizate și trebuie tratate ca dezvoltări viitoare.

Pentru detalii complete există deja și [TEST_PLAN.md](c:/UTCN/Anul3Sem2/II/SmartGreenhouse/docs/TEST_PLAN.md).

---

# 8. Concluzii

## 8.1 Rezumat general

Proiectul `Smart Greenhouse Manager` reprezintă o aplicație desktop bine structurată pentru monitorizarea unei sere inteligente. Soluția folosește o arhitectură pe straturi și integrează interfață grafică, logică de business, acces la baza de date și model de date într-un mod coerent.

## 8.2 Ce a fost realizat

Au fost implementate cu succes:

- autentificarea utilizatorilor;
- dashboard-ul principal;
- simularea automată a citirilor;
- salvarea în baza de date;
- generarea alertelor;
- CRUD-ul pentru senzori;
- o bază de date bine proiectată;
- documentația principală a proiectului.

## 8.3 Ce poate fi îmbunătățit

Pentru o versiune viitoare, proiectul poate fi extins prin:

- implementarea rapoartelor;
- integrarea completă a predicțiilor de trend;
- adăugarea controlului pentru actuatoare;
- confirmarea alertelor din interfață;
- finalizarea managementului pe roluri;
- adăugarea formularului de splash și a altor ecrane auxiliare;
- folosirea unui sistem de configurare unificat pentru connection string.

## 8.4 Valoarea proiectului

Din punct de vedere didactic, proiectul demonstrează competențe importante:

- proiectare software pe straturi;
- modelare de baze de date;
- programare orientată pe obiect;
- lucru cu evenimente și delegate;
- integrarea unei aplicații C# cu SQL Server;
- realizarea unei interfețe grafice desktop.

## 8.5 Concluzie finală

Smart Greenhouse Manager este un proiect reușit pentru domeniul Informatica Industrială, deoarece combină monitorizarea parametrilor de mediu cu o interfață accesibilă și o arhitectură software clară. Chiar dacă există componente încă nefinalizate, aplicația oferă o bază solidă pentru un sistem real de monitorizare și poate fi extinsă în mod natural către control automat și analiză avansată.

