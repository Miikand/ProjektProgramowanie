using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Gra_RPG
{
    // === KLASY ===

    // Klasa przechowuj?ca post?p u?ytkownika (poziom i do?wiadczenie)
    // Klasa przechowuj?ca post?p u?ytkownika (poziom i do?wiadczenie)
    public class UserProgress
    {
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        public void LevelUp()
        {
            Level++;
            Experience = 0;  // Resetujemy do?wiadczenie po awansie na wy?szy poziom
        }
    }


    // Klasa reprezentuj?ca u?ytkownika
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserProgress Progress { get; set; }  // Poprawiona referencja do UserProgress, a nie ogólnego typu

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Progress = new UserProgress();
        }
    }


    // Klasa zarz?dzaj?ca u?ytkownikami, logowaniem i rejestracj?
    public class UserManager
    {
        private List<User> users;
        private const string FilePath = "../../../users.txt";

        public UserManager()
        {
            users = new List<User>();
        }

        public void RegisterUser()
        {
            Console.Write("\nPodaj nazw? u?ytkownika: ");
            string username = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("\n>>> Nazwa u?ytkownika nie mo?e by? pusta! <<<\n");
                return;
            }

            Console.Write("Podaj has?o: ");
            string password = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\n>>> Has?o nie mo?e by? puste! <<<\n");
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("\n>>> U?ytkownik o takiej nazwie ju? istnieje! <<<\n");
                return;
            }

            users.Add(new User(username, password));
            Console.WriteLine("\n>>> Rejestracja zako?czona sukcesem! <<<\n");
        }

        public User LoginUser()
        {
            Console.Write("\nPodaj nazw? u?ytkownika: ");
            string username = Console.ReadLine()?.Trim();
            Console.Write("Podaj has?o: ");
            string password = Console.ReadLine()?.Trim();

            User user = users.Find(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("\n>>> Nie znaleziono u?ytkownika o podanej nazwie. <<<\n");
                return null;
            }

            if (user.Password != password)
            {
                Console.WriteLine("\n>>> Nieprawid?owe has?o. <<<\n");
                return null;
            }

            return user;
        }

        public void SaveUsers()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.Username}|{user.Password}|{user.Progress.Level}|{user.Progress.Experience}");
                    }
                }
                Console.WriteLine("\n>>> Dane u?ytkowników zapisano. <<<\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n>>> B??d zapisywania danych: {ex.Message} <<<\n");
            }
        }

        public void LoadUsers()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(FilePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                string username = parts[0];
                                string password = parts[1];
                                int level = int.Parse(parts[2]);
                                int experience = int.Parse(parts[3]);

                                var user = new User(username, password)
                                {
                                    Progress = new UserProgress
                                    {
                                        Level = level,
                                        Experience = experience
                                    }
                                };

                                users.Add(user);
                            }
                        }
                    }
                    Console.WriteLine("\n>>> Dane u?ytkowników wczytano. <<<\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n>>> B??d wczytywania danych: {ex.Message} <<<\n");
                }
            }
        }
    }

    // Klasa reprezentuj?ca przedmiot
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Power { get; set; }
        public ItemType Type { get; set; }

        public Item(string name, string description, int power, ItemType type)
        {
            Name = name;
            Description = description;
            Power = power;
            Type = type;
        }
    }

    // Typy przedmiotów
    public enum ItemType
    {
        Weapon,
        HealthPotion
    }

    // Klasa bazowa dla postaci i przeciwników
    public abstract class Entity
    {
        public string Name { get; set; }
        public int Level { get; protected set; }
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; }
        public int AttackPower { get; protected set; }
        public List<Item> Inventory { get; private set; }

        public Entity(string name, int level, int maxHP, int attackPower)
        {
            Name = name;
            Level = level;
            MaxHP = maxHP;
            HP = maxHP;
            AttackPower = attackPower;
            Inventory = new List<Item>();
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
        }


        public bool IsDefeated()
        {
            return HP <= 0;
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public void DisplayInventory()
        {
            Console.WriteLine($"\n=== Ekwipunek {Name} ===");
            if (Inventory.Count == 0)
            {
                Console.WriteLine("Brak przedmiotów w ekwipunku.");
                return;
            }

            for (int i = 0; i < Inventory.Count; i++)
            {
                var item = Inventory[i];
                Console.WriteLine($"{i + 1}. {item.Name} - {item.Description} (Moc: {item.Power})");
            }
        }

        public void UseItem(int index)
        {
            if (index < 0 || index >= Inventory.Count)
            {
                Console.WriteLine("\n>>> Nieprawid?owy wybór. <<<");
                return;
            }

            Item item = Inventory[index];
            if (item.Type == ItemType.HealthPotion)
            {
                HP = Math.Min(MaxHP, HP + item.Power);
                Console.WriteLine($"\n>>> U?yto {item.Name}. Odzyskano {item.Power} HP. Aktualne HP: {HP}/{MaxHP} <<<\n");
                Inventory.RemoveAt(index);
            }
            else if (item.Type == ItemType.Weapon)
            {
                Console.WriteLine($"\n>>> U?yto {item.Name}. Atak zwi?kszony o {item.Power}. <<<");
                AttackPower += item.Power;
                Inventory.RemoveAt(index); // Usu? przedmiot po u?yciu
            }
        }
    }

    // Klasa reprezentuj?ca posta? gracza
    public class Character : Entity
    {
        public string Class { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }

        public Character(string name, string charClass)
            : base(name, 1, 100, 10)
        {
            Class = charClass;
            Experience = 0;
            Level = 1;

            switch (Class.ToLower())
            {
                case "mag":
                    MaxHP = 80;
                    AttackPower = 20;
                    break;
                case "?ucznik":
                    MaxHP = 100;
                    AttackPower = 15;
                    break;
                case "wojownik":
                    MaxHP = 120;
                    AttackPower = 10;
                    break;
                default:
                    throw new ArgumentException("Nieznana klasa postaci");
            }

            HP = MaxHP;
        }

        public void GainExperience(int experiencePoints)
        {
            Experience += experiencePoints;

            // Sprawdzamy, czy do?wiadczenie przekroczy?o 100, co powoduje awans na poziom
            while (Experience >= 100)
            {
                Experience -= 100; // Zmniejszamy do?wiadczenie o 100
                Level++;  // Zwi?kszamy poziom postaci

                // Zwi?kszenie HP i obra?e? o 1% na ka?dym poziomie
                MaxHP = (int)(MaxHP * 1.01);
                AttackPower = (int)(AttackPower * 1.01);

                // Wy?wietlamy informacj? o awansie
                Console.WriteLine($"\n>>> {Name} osi?gn?? nowy poziom! Poziom: {Level} <<<");
                Console.WriteLine($">>> Maksymalne HP: {MaxHP}, Moc ataku: {AttackPower} <<<");
            }
        }
    }





    // Klasa reprezentuj?ca przeciwnika
    public class Enemy : Entity
    {
        private static Random random = new Random();

        // S³ownik ataków dla przeciwników
        private Dictionary<string, (int minDamage, int maxDamage)> attackTypes;

        public Enemy(string name, int level, int maxHP, int attackPower)
            : base(name, level, maxHP, attackPower)
        {
            // Przypisanie specyficznych ataków do przeciwnika
            attackTypes = new Dictionary<string, (int, int)>();
        }

        // Skalowanie statystyk przeciwnika na podstawie poziomu
        public void ScaleStats()
        {
            MaxHP = (int)(MaxHP * (1 + 0.1 * Level));  // HP roœnie o 1% na poziom
            AttackPower = (int)(AttackPower * (1 + 0.1 * Level));  // Obra¿enia rosn¹ o 1% na poziom
        }

        // Przypisanie ataków do przeciwnika
        public void SetAttacks(List<(string, int, int)> attacks)
        {
            attackTypes.Clear();
            foreach (var attack in attacks)
            {
                attackTypes.Add(attack.Item1, (attack.Item2, attack.Item3));
            }
        }

        // Wybór losowego ataku przeciwnika
        public string GetRandomAttack(out int damage)
        {
            // Losowanie ataku
            var attack = attackTypes.ElementAt(random.Next(attackTypes.Count));

            // Losowanie obra¿eñ dla tego ataku
            damage = random.Next(attack.Value.minDamage, attack.Value.maxDamage + 1);
            return attack.Key;
        }
    }

    // Klasa odpowiedzialna za wy?wietlanie menu
    public class Menu
    {
        public void ShowTutorial()
        {
            Console.WriteLine("\n=== Tutorial dla nowego gracza ===");
            Console.WriteLine("Witaj w grze RPG!");
            Console.WriteLine("Twoim celem jest walka z potworami, zdobywanie doœwiadczenia i przedmiotów.");
            Console.WriteLine("Oto kilka wa¿nych informacji:");
            Console.WriteLine("1. Walczysz, aby zdobywaæ doœwiadczenie i podnosiæ poziom postaci.");
            Console.WriteLine("2. W ekwipunku znajdziesz mikstury zdrowia i broñ, które pomog¹ Ci w walce.");
            Console.WriteLine("3. Pamiêtaj, aby zarz¹dzaæ ekwipunkiem i u¿ywaæ przedmiotów w odpowiednich momentach.");
            Console.WriteLine("4. Po wygranej walce mo¿esz zdobyæ nagrody, takie jak nowe przedmioty.");
            Console.WriteLine("\nPowodzenia w grze! Rozpocznij swoj¹ przygodê!");
            Console.WriteLine("\nNaciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
        }
        public void ShowAuthMenu()
        {
            Console.WriteLine("\n=== Menu Logowania ===");
            Console.WriteLine("1. Zarejestruj u?ytkownika");
            Console.WriteLine("2. Zaloguj si?");
            Console.WriteLine("3. Wyjd?");
            Console.Write("Wybierz opcj?: ");
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("\n=== G?ówne Menu ===");
            Console.WriteLine("1. Rozpocznij bitw?");
            Console.WriteLine("2. Zarz?dzaj ekwipunkiem");
            Console.WriteLine("3. Wyloguj si?");
            Console.WriteLine("4. Wyjd? z gry");
            Console.Write("Wybierz opcj?: ");
        }

        public void ShowInventoryMenu(Character player)
        {
            Console.WriteLine("\n=== Ekwipunek ===");
            player.DisplayInventory();
            Console.WriteLine("\nWybierz przedmiot do u?ycia lub naci?nij '0' aby wróci?:");
            string choice = Console.ReadLine()?.Trim();

            if (int.TryParse(choice, out int itemIndex) && itemIndex > 0 && itemIndex <= player.Inventory.Count)
            {
                player.UseItem(itemIndex - 1); // Indeksowanie zaczyna si? od 1, wi?c odejmujemy 1
            }
            else if (choice == "0")
            {
                return; // Wraca do g?ównego menu
            }
            else
            {
                Console.WriteLine("\n>>> Nieprawid?owy wybór. <<<");
            }
        }

        public Character CreateCharacter()
        {
            Console.WriteLine("\n=== Tworzenie Postaci ===");
            Console.Write("Podaj nazw? postaci: ");
            string name = Console.ReadLine()?.Trim();

            Console.WriteLine("Wybierz klas? postaci:");
            Console.WriteLine("1. Mag");
            Console.WriteLine("2. ?ucznik");
            Console.WriteLine("3. Wojownik");
            Console.Write("Wybierz opcj?: ");

            string classChoice = Console.ReadLine()?.Trim();
            string charClass = classChoice switch
            {
                "1" => "mag",
                "2" => "?ucznik",
                "3" => "wojownik",
                _ => throw new ArgumentException("Nieprawid?owy wybór klasy postaci")
            };

            Console.WriteLine($"\n>>> Utworzono posta?: {name}, Klasa: {charClass} <<<\n");
            return new Character(name, charClass);
        }
    }

    // Klasa odpowiedzialna za przeprowadzanie walki
    public class Game
    {
        private Random random = new Random();

        public void StartBattle(Character player, Enemy enemy)
        {
            Console.WriteLine("\n=== Rozpoczêcie walki ===");

            // Skalowanie statystyk przeciwnika przed ka¿d¹ walk¹
            enemy.ScaleStats();

            // Walka
            while (!player.IsDefeated() && !enemy.IsDefeated())
            {
                Console.WriteLine($"\n{player.Name}: {player.HP}/{player.MaxHP} HP");
                Console.WriteLine($"{enemy.Name}: {enemy.HP}/{enemy.MaxHP} HP");
                Console.WriteLine("\n1. Atakuj");
                Console.WriteLine("2. U¿yj przedmiotu");
                Console.Write("Wybierz akcjê: ");

                string action = Console.ReadLine()?.Trim();
                int damage;

                switch (action)
                {
                    case "1":
                        int playerDamage = random.Next(player.AttackPower - 5, player.AttackPower + 5);
                        enemy.TakeDamage(playerDamage);
                        Console.WriteLine($"\n>>> Zada³eœ {playerDamage} obra¿eñ przeciwnikowi! <<<");
                        break;
                    case "2":
                        player.DisplayInventory();
                        Console.Write("Wybierz przedmiot do u¿ycia (numer): ");
                        if (int.TryParse(Console.ReadLine(), out int itemIndex))
                        {
                            player.UseItem(itemIndex - 1); // Indeksowanie zaczyna siê od 1, wiêc odejmujemy 1
                        }
                        else
                        {
                            Console.WriteLine("\n>>> Nieprawid³owy wybór. <<<");
                        }
                        break;
                    default:
                        Console.WriteLine("\n>>> Nieprawid³owy wybór. <<<");
                        break;
                }

                if (!enemy.IsDefeated())
                {
                    // Losowanie ataku przeciwnika i obra¿eñ
                    string enemyAttack = enemy.GetRandomAttack(out damage);
                    player.TakeDamage(damage);
                    Console.WriteLine($"\n>>> {enemy.Name} u¿y³ ataku: {enemyAttack} i zada³ {damage} obra¿eñ! <<<");
                }
            }

            if (player.IsDefeated())
            {
                Console.WriteLine("\n>>> Zosta³eœ pokonany. <<<");
            }
            else
            {
                Console.WriteLine("\n>>> Pokona³eœ przeciwnika! <<<");
                player.GainExperience(50);  // Dodawanie doœwiadczenia po wygranej walce
            }
        }

            public void RewardPlayer(Character player)
        {
            Console.WriteLine("\n=== ?upy po walce ===");

            // 75% szans na mikstur?
            if (random.Next(100) < 75)
            {
                Item potion = new Item("Mikstura Zdrowia", "Przywraca 50 HP.", 50, ItemType.HealthPotion);
                player.AddItem(potion);
                Console.WriteLine($">>> Otrzymano: {potion.Name} - {potion.Description}");
            }

            // 20% szans na now? bro?
            if (random.Next(100) < 20)
            {
                Item weapon = new Item("Miecz", "Zwi?ksza moc ataku.", 10, ItemType.Weapon);
                player.AddItem(weapon);
                Console.WriteLine($">>> Otrzymano: {weapon.Name} - {weapon.Description}");
            }
        }
    }

    // Klasa g?ówna gry
    class Program
    {
        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            userManager.LoadUsers();
            Menu menu = new Menu();
            User currentUser = null;
            Character player = null;
            bool loggedIn = false;
            bool exit = false;

            while (!exit)
            {
                if (!loggedIn)
                {
                    menu.ShowAuthMenu();
                    string choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            userManager.RegisterUser();
                            break;
                        case "2":
                            currentUser = userManager.LoginUser();
                            if (currentUser != null)
                            {
                                loggedIn = true;
                                Console.WriteLine($"\n>>> Zalogowano jako: {currentUser.Username} <<<\n");
                                player = menu.CreateCharacter();
                                menu.ShowTutorial();  // Wywo³anie tutorialu po utworzeniu postaci
                            }
                            break;
                        case "3":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\n>>> Nieprawid³owy wybór. <<<");
                            break;
                    }
                }
                else
                {
                    menu.ShowMainMenu();
                    string choice = Console.ReadLine()?.Trim();

                    switch (choice)
                    {
                        case "1":
                            // Tworzenie przeciwników z unikalnymi atakami
                            var enemy1 = new Enemy("Goblin", 1, 50, 10);
                            var goblinAttacks = new List<(string, int, int)>
                        {
                            ("Sztylet", 5, 12),
                            ("Skradzione ¿ycie", 2, 6)
                        };
                            enemy1.SetAttacks(goblinAttacks);

                            var enemy2 = new Enemy("Ork", 2, 70, 15);
                            var orkAttacks = new List<(string, int, int)>
                        {
                            ("M³ot", 8, 15),
                            ("Rage", 10, 20)
                        };
                            enemy2.SetAttacks(orkAttacks);

                            var enemy3 = new Enemy("Smok", 3, 150, 30);
                            var smokAttacks = new List<(string, int, int)>
                        {
                            ("Ognisty podmuch", 20, 40),
                            ("Skrzyd³a", 10, 30)
                        };
                            enemy3.SetAttacks(smokAttacks);

                            var enemy4 = new Enemy("Wampir", 4, 80, 20);
                            var wampirAttacks = new List<(string, int, int)>
                        {
                            ("Uk¹szenie", 15, 25),
                            ("Czarny Mrok", 10, 20)
                        };
                            enemy4.SetAttacks(wampirAttacks);

                            var enemy5 = new Enemy("Szkielet", 5, 90, 25);
                            var szkieletAttacks = new List<(string, int, int)>
                        {
                                ("Strza³ z ³uku", 17, 27),
                                ("Cios mieczem", 12, 22)
                        };
                            enemy5.SetAttacks(szkieletAttacks);

                            var enemy6 = new Enemy("Zombiak", 6, 95, 27);
                            var zombieAttacks = new List<(string, int, int)>
                        {
                                ("Uderzenie", 19, 29),
                                ("Zara¿enie", 15, 25)
                        };
                            enemy6.SetAttacks(szkieletAttacks);

                            var enemy7 = new Enemy("Czarodziej", 7, 100, 30);
                            var czarodziejAttacks = new List<(string, int, int)>
                        {
                                 ("Kula Ognia", 21, 31),
                                   ("Zamro¿enie", 17, 27)
                        };
                            enemy7.SetAttacks(czarodziejAttacks);

                            var enemy8 = new Enemy("minotaur", 8, 250, 45);
                            var minotaurAttacks = new List<(string, int, int)>
                        {
                                ("Rogaty atak", 23, 33),
                                ("Szalony bieg", 19, 30)
                        };
                            enemy8.SetAttacks(minotaurAttacks);

                            var enemy9 = new Enemy("demon", 9, 500, 50);
                            var demonAttacks = new List<(string, int, int)>
                        {
                                  ("Ognisty wybuch", 30, 120),
                                ("Cios piekielny", 40, 110)
                        };
                            enemy9.SetAttacks(demonAttacks);

                            



                            // Wybór przeciwnika do walki
                            Console.WriteLine("Wybierz przeciwnika do walki:");
                            Console.WriteLine("1. Goblin");
                            Console.WriteLine("2. Ork");
                            Console.WriteLine("3. Smok");
                            Console.WriteLine("4. Wampir");
                            Console.WriteLine("5. Szkielet");
                            Console.WriteLine("6. Zombiak");
                            Console.WriteLine("7. Czarodziej");
                            Console.WriteLine("8. Minotaur");
                            Console.WriteLine("9. Demon");
                            Console.Write("Wybierz opcjê: ");
                            string enemyChoice = Console.ReadLine()?.Trim();
                            Enemy selectedEnemy = null;

                            switch (enemyChoice)
                            {
                                case "1":
                                    selectedEnemy = enemy1;
                                    break;
                                case "2":
                                    selectedEnemy = enemy2;
                                    break;
                                case "3":
                                    selectedEnemy = enemy3;
                                    break;
                                case "4":
                                    selectedEnemy = enemy4;
                                    break;
                                case "5":
                                    selectedEnemy = enemy5;
                                    break;
                                case "6":
                                    selectedEnemy = enemy6;
                                    break;
                                case "7":
                                    selectedEnemy = enemy7;
                                    break;
                                case "8":
                                    selectedEnemy = enemy8;
                                    break;
                                case "9":
                                    selectedEnemy = enemy9;
                                    break;
                                default:
                                    Console.WriteLine("\n>>> Nieprawid³owy wybór. <<<");
                                    continue; // Powrót do g³ównego menu
                            }

                            var game = new Game();
                            game.StartBattle(player, selectedEnemy);
                            game.RewardPlayer(player);
                            break;
                        case "2":
                            menu.ShowInventoryMenu(player);
                            break;
                        case "3":
                            loggedIn = false;
                            Console.WriteLine("\n>>> Wylogowano. <<<");
                            break;
                        case "4":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\n>>> Nieprawid³owy wybór. <<<");
                            break;
                    }
                }
            }

            userManager.SaveUsers();
        }
    }

}