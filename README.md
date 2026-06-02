# Dokumentáció

## Játék leirása
A játék elején a felhasználó 1 db C-3PO pakoló robottal, 1 db Voyager töltő robottal, egy 10 férőhelyes raktárral és 20 000 dolláral kezdi meg a játékot. A játék során az a cél, hogy minél nagyobb raktárat és bevételt birtokoljon a játékos. Minden játéknap elején a raktár szabad férőhelyének 60%-ának megfelelő csomag érkezik. Csak addig érkezik csomag, amíg van szabad férőhely a raktárban. A játék során kell fizetni karbantartási költséget is, ami minden nap legelején kerül levonásra. A robotoknál már a vásárlásnál meg van adva, hogy mennyi lesz az adott robot fenntartási költsége. A raktár után is kell fizetni karbantartási díjat, ami a raktár aktuális méretének 30% * 200 $. A játék akkor ér véget, ha a játékos egyenlege mínuszba megy. Ekkor már nem tud semmit csinálni abban a játékban, és újat kell kezdenie.

A csomagoknál két típus van, de azon belül többféle csomag létezik. A csomagtípus és a csomag megnevezése random kerül kiválasztásra a játék során. A csomagok lehetséges elnevezéseit a packages.json tartalmazza.

Az alábbi csomagtípusok léteznek:
- Romlandó csomag
- Nem romlandó csomag

Pakolás során a romlandó csomagok elsőbbséget élveznek, emiatt ezek kerülnek elpakolásra először. A csomagoknál az ár és a pakolási idő is random kerül kiválasztásra. A tárolási idő 1–8 nap között lehet, valamint a csomagok ára 1700$ és 3900$ között mozog. Minél tovább kell tárolni egy csomagot, annál több pénzt kap érte a felhasználó. Az alábbiak szerint módosul a csomag ára:
- 1-3 napig 1 * csomag ára
- 4-7 napig 1,5 * csomag ára
- 8 napig 2 * csomag ára

A játék során a töltő- és pakolórobotokból több fajtát is lehet majd vásárolni.

A pakolórobotokból összesen 6 db van, és azok az alábbiak:
- C-3PO
- Jhonny 5
- Data
- HAL 9000
- Ultron
- T-800

A töltő robotokból összesen 4 db van, és azok az alábbiak:
- Voyager
- ASIMO
- BB-8
- Sonny

A játék során 3 db fajta raktár bővítés közül lehet választani, amik az alábbiak:
- Kis bővítés
- Közepes bővítés
- Nagy bővítés 

A raktár bővítések és a robotok a shop.json-ben vannak eltárolva, hogy a későbbiekben könyen modositható és bövithető legyen.

## Változás az 1. mérföldkőhöz képest:
A játéknál minden meg lett valósítva, ami az első mérföldkő leírásában szerepelt, csak a játékhoz hozzá lett adva egy extra rendszer is, amivel játékbeli trófeákat lehet gyűjteni. Összesen 43 játékbeli trófeát tud szerezni a felhasználó. A játékos a már elindított játékon belül tudja megszerezni ezeket a trófeákat, ami annyit jelent, hogy minden játéknak külön trófearendszere van.

## 1. Megvalósíthatósági elemzés (részletek):
- Humán erőforrások: egy tervező/fejlesztő (45 óra), egy tesztelő (5 óra)
- Hardver erőforrások: egy fejlesztői, egy tesztelői számítógép (közepes hardverigény)
- Szoftver erőforrások: fejlesztőkörnyezet (Visual Studio), verziókövető (Git), adatbáziskezelő (MSSQL)
- Üzemeltetés: a telepítést/átadást követően további üzemeltetést nem kell biztosítani
- Karbantartás: az esetleges hibajavításon felül nem kell biztosítani
- Megvalósítás időtartama 50 emberóra

## 2. Funkcionális követelmények:
- Játékot megelőzően: új játék indítása, korábbi játékállás betöltése vagy kilépés az alkalmazásból
- Játék közben:
  - Aktuális raktár állapot megjelenítése
  - Vásárlás esetén egy vásárlási menü megjelenítésé, ahol robotokat és raktár területet lehet venni.
  - Raktár készlet lekérdezés esetén egy menü megjelenítésé, ahol az áruk adatait lehet megtekinteni.
  - Egy gomb, amivel a következő napra lehet lépni vagy pedig automatikus léptetés bekapcsolása
- Automatikus mentések minden új napnál és manuális mentési lehetőség

## 3. Termék követelmények:
- Hatékonyság:
  - jelentéktelen terhelés a processzor, memória és háttértár részére, hálózatot nem igényel
  - gyors (1 másodperc alatti) válaszidő minden bevitelre egy alsó kategóriás számítógépen
- Megbízhatóság:
  - szabványos használat esetén nem fordul elő hibajelenség, nem jelenik meg hibaüzenet
  - hibás emberi bevitel esetén hibaüzenet és a bevitel megismétlése
- Biztonság: nem releváns
- Hordozhatóság: a legtöbb személyi számítógépen biztosított a használat
- Felhasználhatóság:
  - intuitív felhasználói felület, külön segédlet, vagy leírás nem szükséges a használathoz

## 4. Menedzselési követelmények:
- Környezeti:
  - nem működik együtt semmilyen külső szoftverrel vagy szolgáltatással, az adatokat lokális adatbázisban tárolja
- Fejlesztési:
  - C# nyelv, Visual Studio környezet, objektumorientált paradigma

## 5. Felhasználói történet
**Mint játékos szeretnék új robotot vásárolni.**
1. Amennyiben a robot vásárlás menüpontot választottuk, ha kiválasztjuk a robotot amelyiket szeretnénk és rendelkezésre áll elegendő pénz, akkor a program hozzá adja a robotot a raktárhoz és visszatér a vásárlási menübe.
2. Amennyiben a robot vásárlás menüpontot választottuk, ha kiválasztjuk a robotot amelyiket szeretnénk és nem áll rendelkezésre elegendő pénz akkor a program egy hiba üzenetet jelenít meg majd visszatér a vásárlási menübe.

**Mint játékos szeretnék a következő napra lépni.**
1. Amennyiben a következő nap gombra rányomtunk, ha rendelkezésre áll elegendő pénz a karbantartási költségek fizetésére, akkor a program a következő napra lép és levonja a karbantartási költségeket.
2. Amennyiben a következő nap gomra rányomtunk, ha nem áll rendelkezésre elegendő pénz a karbantartási költségek fizetésére, akkor a program befejezi a játékot és megjelenít egy összesítő felületet, ahol meg tekinthe a játékos a játék során elért eredményeit.

## 6 Használati esetek diagram
![alt text](img/image.png)

## 7 Osztály diagram
```mermaid
classDiagram
%% Relationships
    Game "1" -- "1" WareHouse : Kezeli 
    Game "1" -- "*" UnlockedAchievement : Kezeli 
    WareHouse "1" -- "*" Packages : Kezeli 
    WareHouse "1" -- "*" Robot : Kezeli 
    WareHouse "1" -- "*" WarehouseUpgrade : Kezeli
    Robot <|-- PackingRobot : Örököl
    Robot <|-- ChargingRobot : Örököl
    
    %% Dependencies (Függőségek és Interakciók)
    PackingRobot ..> Packages : Használja
    ChargingRobot ..> PackingRobot : Kezeli 
    %% Classes
    class Game {
        +int ID
        +string GameName
        +int Balance
        +bool IsGameOver
        +int WarehouseId
        +int CurrentDay
        +int CurrentHour
        +ProcessDeliveries()
        +NextDay()
        +CanAfford(price: int) bool
        +DeductBalance(price: int)
    }

    class UnlockedAchievement {
        +int ID
        +int GameId
        +string AchievementId
    }

    class WareHouse {
        +int ID
        +int StorgarSize
        +int MaitananceFee
        +int UsedSpace
        +int FreeSpace
        +int RobotMaintenanceFee
        +int WarehouseMaintenanceFee
        +int TotalMaintenanceFee
        +GetOwnedRobotCounts() Dictionary~string, int~
        +GetUpgradeCount(upgradeName: string) int
        +AddUpgrade(upgradeName: string, sizeIncrease: int)
        +AddRobotFromShop(robotType: string, robotName: string, shopData: JsonElement) Robot
        +ProcessDailyPacking(currentDay: int, hoursToProcess: int)
        +ProcessHourlyPacking(currentDay: int)
    }

    class WarehouseUpgrade {
        +int ID
        +int WareHouseId
        +string UpgradeName
        +int Quantity
    }

    class Packages {
        +int ID
        +string Type
        +string Name
        +int Price
        +int StorageTime
        +bool Status
        +bool IsUnderPacking
        +bool IsDelivered
        +float BatteryCost
        +int WareHouseId
        +int CreatedOnDay
        +int? PackedOnDay
        +IsReadyForDelivery(currentDay: int) bool
        +GenerateForWarehouse(wareHouse: WareHouse, packageJsonPath: string, currentDay: int)$ List~Packages~
    }

    class Robot {
        +int ID
        +string Name
        +bool Status
        +int MaintenanceFee
        +int Price
        +string Img
        +int? WareHouseId
    }

    class PackingRobot {
        +float BatteryLevel
        +int PackingSpeed
        +int BatterySize
        +bool IsCharging
        +PackPackages(packagesToPack: List~Packages~, currentDay: int)
    }

    class ChargingRobot {
        +float ChargingSpeed
        +int MaxChargingCapacity
        +ChargeRobots(robotsToCharge: List~PackingRobot~)
    }
```

## 8 Adatbázis diagram
```mermaid
erDiagram
    %% Táblák kapcsolatai
    GAMES ||--|| WAREHOUSES : ""
    GAMES ||--o{ UNLOCKED_ACHIEVEMENTS : ""
    WAREHOUSES ||--o{ PACKAGES : ""
    WAREHOUSES ||--o{ ROBOTS : ""
    WAREHOUSES ||--o{ WAREHOUSE_UPGRADES : ""

    %% Táblák és oszlopok
    GAMES {
        int ID PK
        string GameName
        int Balance
        bool IsGameOver
        int CurrentDay
        int CurrentHour
        int WarehouseId FK
    }

    WAREHOUSES {
        int ID PK
        int StorgarSize
        int MaitananceFee
    }

    UNLOCKED_ACHIEVEMENTS {
        int ID PK
        int GameId FK
        string AchievementId
    }

    WAREHOUSE_UPGRADES {
        int ID PK
        int WareHouseId FK
        string UpgradeName
        int Quantity
    }

    PACKAGES {
        int ID PK
        string Type
        string Name
        int Price
        int StorageTime
        bool Status
        bool IsUnderPacking
        bool IsDelivered
        float BatteryCost
        int WareHouseId FK
        int CreatedOnDay
        int PackedOnDay
    }

    ROBOTS {
        int ID PK
        string Discriminator "EF Core TPH (Melyik típus?)"
        string Name
        bool Status
        int MaintenanceFee
        int WareHouseId FK
        float BatteryLevel "Csak PackingRobot"
        int PackingSpeed "Csak PackingRobot"
        int BatterySize "Csak PackingRobot"
        bool IsCharging "Csak PackingRobot"
        float ChargingSpeed "Csak ChargingRobot"
        int MaxChargingCapacity "Csak ChargingRobot"
    }
```
## 9 Komponens diagram

```mermaid
flowchart TB
    %% Client
    Client["Client Web Browser\nHTML, CSS, JS"]

    %% Presentation Layer
    subgraph Presentation["Presentation Layer (ASP.NET Core MVC)"]
        Controllers["Controllers\nGamesController, HomeController"]
        Views["Views\nRazor Views"]
    end

    %% Business Logic Layer
    subgraph BusinessLogic["Business Logic Layer (Services)"]
        AchievementService["AchievementService"]
        ShopService["ShopService"]
    end

    %% Data Access Layer
    subgraph DataAccess["Data Access Layer"]
        Models["Models Business Entities\nGame, WareHouse, Robot, etc."]
        DbContext["RobotSimulationContext\nEntity Framework Core"]
    end

    %% Database and Files
    Database[("Relational Database\nSQL Server")]
    JsonFiles["Static Data Files\npackages.json, shop.json"]

    %% Relationships
    Client <-->|HTTP GET and POST| Controllers
    Controllers -->|Pass Models and Render| Views
    Views -->|HTML Response| Client
    
    Controllers <-->|Delegate Business Logic| AchievementService
    Controllers <-->|Fetch Data| ShopService
    
    Controllers <-->|CRUD Operations| DbContext
    AchievementService <-->|Data Fetch| DbContext
    
    ShopService -->|Read Data| JsonFiles
    
    DbContext <-->|Instantiation and Modification| Models
    Controllers -->|Uses| Models
    
    DbContext <-->|SQL Queries| Database
```
## 10 Állapotdiagramok (State Diagrams)

### 1. Aggregált állapotdiagram

```mermaid
stateDiagram-v2
    [*] --> JatekIndul
    
    state "Aktív Játék Ciklus" as JatekCiklus {
        [*] --> Varakozas
        
        %% Óránkénti tick
        Varakozas --> CsomagokGeneralasa : Új óra kezdődik
        CsomagokGeneralasa --> CsomagolasiFolyamat : Csomagok várólistán
        CsomagolasiFolyamat --> ToltesiFolyamat : Robotok merülnek
        ToltesiFolyamat --> Varakozas : Óra véget ért
        
        %% Napi tick
        Varakozas --> NapVegeZaras : Eltelt 24 óra
        NapVegeZaras --> KiszallitasFeldolgozasa : Kész csomagok eladása
        KiszallitasFeldolgozasa --> KoltsegekLevonasa : Fenntartási díjak kifizetése
        KoltsegekLevonasa --> EgyenlegEllenorzes
    }
    
    JatekIndul --> JatekCiklus
    EgyenlegEllenorzes --> JatekCiklus : Egyenleg >= 0
    EgyenlegEllenorzes --> GameOver : Egyenleg < 0 (Csőd)
    GameOver --> [*]
```

<hr/>

## 2. Részekre bontott Állapotdiagramok

### 2.1 Csomag állapotai
Egy csomag életútja a generálástól a kiszállításig.

```mermaid
stateDiagram-v2
    [*] --> Varakozik : Létrejött (Status = false)
    
    Varakozik --> CsomagolasAlatt : Robot megkezdi a csomagolást
    CsomagolasAlatt --> Becsomagolva : Robot sikeresen befejezi (Status = true)
    CsomagolasAlatt --> Varakozik : Robot akkumulátora lemerült (megszakítva)
    
    Becsomagolva --> Kiszallitva : Tárolási idő (StorageTime) letelt
    Kiszallitva --> [*] : Pénz jóváírva
```

### 2.2 Csomagoló Robot állapotai
A csomagoló robot munka és töltés közbeni állapotai.

```mermaid
stateDiagram-v2
    [*] --> Üresjarat : Akkumulátor feltöltve
    
    Üresjarat --> Dolgozik : Van csomagolnivaló 
    Dolgozik --> Üresjarat : Csomagolás kész / Nincs több csomag
    Dolgozik --> Lemerult : Akkumulátor eléri a 0-t
    
    Lemerult --> Toltodik : Töltő robot elkezdi tölteni (IsCharging = true)
    Toltodik --> Üresjarat : Akkumulátor ismét maximumon (IsCharging = false)
```

### 2.3 Játék állapotai
A játékmenet egyszerűsített állapota.

```mermaid
stateDiagram-v2
    [*] --> Aktiv : Játék létrehozva
    Aktiv --> CsodbeMent : Egyenleg negatívba fordult (IsGameOver = true)
    CsodbeMent --> [*] : Játék vége
```
## 11 Szekvencia Diagramok

## 1. A Játék Ciklusa 
Ez a diagram azt mutatja be, hogy mi történik a háttérben, amikor a játékos elindítja a játékot, és az idő elkezd telni.

```mermaid
sequenceDiagram
    actor Player as Játékos (Böngésző)
    participant JS as Kliens JavaScript
    participant Ctrl as GamesController
    participant Game as Game (Modell)
    participant WH as WareHouse (Modell)
    participant DB as Adatbázis (EF Core)

    Player->>JS: Megnyomja a "Játék Indítása" gombot
    loop Kb. 2 másodpercenként (Óra tick)
        JS->>Ctrl: POST /Games/NextHour
        Ctrl->>DB: Játék és Raktár betöltése
        DB-->>Ctrl: Entitások visszaadása
        
        Ctrl->>WH: ProcessHourlyPacking(CurrentDay)
        WH->>WH: Robotok dolgoztatása (Csomagolás/Töltés)
        
        Ctrl->>Game: CurrentHour++
        
        alt CurrentHour >= 24 (Nap vége)
            Ctrl->>Game: NextDay()
            Game->>Game: Napi költségek levonása (Csőd ellenőrzés)
            Ctrl->>Game: ProcessDeliveries() (Kiszállítás & Pénz jóváírás)
        end
        
        Ctrl->>DB: Mentés (SaveChanges)
        DB-->>Ctrl: Mentés sikeres
        
        Ctrl-->>JS: Oldal frissítése (HTML válasz)
        JS->>Player: UI frissül (Új egyenleg, idő)
    end
```

<hr/>

## 2. Robot vásárlása a boltban
Ez a diagram a vásárlás folyamatát és ellenőrzését mutatja be.

```mermaid
sequenceDiagram
    actor Player as Játékos
    participant Ctrl as GamesController
    participant Shop as ShopService
    participant Game as Game (Modell)
    participant WH as WareHouse (Modell)
    participant DB as Adatbázis (EF Core)

    Player->>Ctrl: POST /Games/BuyRobot (Robot típusa)
    Ctrl->>DB: Aktuális játék betöltése
    DB-->>Ctrl: Game & WareHouse entitások
    
    Ctrl->>Shop: GetShopData("robots.json")
    Shop-->>Ctrl: Árak és statisztikák
    
    alt Van elég egyenleg és raktárhely?
        Ctrl->>Game: DeductBalance(Ár)
        Ctrl->>WH: AddRobotFromShop(Adatok)
        WH->>WH: Új Robot objektum létrehozása
        
        Ctrl->>DB: Mentés (SaveChanges)
        DB-->>Ctrl: Mentés sikeres
        Ctrl-->>Player: Sikeres vásárlás (Visszairányítás a boltba)
    else Nincs elég pénz / Nincs hely
        Ctrl-->>Player: Hibaüzenet megjelenítése (TempData)
    end
```

<hr/>

## 3. Trófeák ellenőrzése
Ez a folyamat a háttérben fut aszinkron módon, hogy megvizsgálja, elért-e valamilyen trófeát a játékos.

```mermaid
sequenceDiagram
    actor Player as Játékos
    participant JS as Kliens JavaScript
    participant Ctrl as GamesController
    participant AchService as AchievementService
    participant DB as Adatbázis (EF Core)

    JS->>Ctrl: GET /Games/CheckAchievements
    Ctrl->>DB: Játék adatok lekérése
    DB-->>Ctrl: Game (Pénz, Robotok, stb.)
    
    Ctrl->>AchService: CheckAchievements(Game)
    AchService->>AchService: Szabályok betöltése (achievements.json)
    
    loop Minden fel nem oldott trófeára
        AchService->>AchService: Szabály tesztelése (pl. Balance >= 10000)
        alt Feltétel teljesül
            AchService->>DB: Új UnlockedAchievement hozzáadása
        end
    end
    
    AchService->>DB: Mentés (SaveChanges)
    DB-->>AchService: Sikeres mentés
    AchService-->>Ctrl: Újonnan feloldott trófeák listája
    
    Ctrl-->>JS: JSON válasz (Új trófeák adatai)
    
    alt Van új trófea
        JS->>Player: Felugró értesítés megjelenítése a képernyőn (Popup)
    end
```