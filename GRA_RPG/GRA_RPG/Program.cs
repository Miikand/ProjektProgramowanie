using System;
using System.Collections.Generic;
using System.IO;

namespace GRA_RPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            Menu menu = new Menu();
            bool exit = false;
            bool loggedIn = false;
            User currentUser = null;

            userManager.LoadUsers();

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
                            }
                            break;
                        case "3":
                            exit = true;
                            Console.WriteLine("\n>>> Dziêkujemy za skorzystanie z programu! <<<\n");
                            break;
                        default:
                            Console.WriteLine("\n>>> Nieprawid³owa opcja. Spróbuj ponownie. <<<\n");
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
                            Character player = menu.CreateCharacter();
                            player.AddItem(new Item("Magic Wand", "A powerful wand", 50));
                            player.AddItem(new Item("Health Potion", "Restores 50 HP", 50));

                            Enemy enemy = new Enemy("Goblin", 1, 50, 10);
                            enemy.AddItem(new Item("Rusty Dagger", "A weak dagger", 10));
                            enemy.AddItem(new Item("Shield Fragment", "Part of a broken shield", 5));

                            Game game = new Game();
                            game.StartBattle(player, enemy);

                            currentUser.Progress.Level = player.Level;
                            currentUser.Progress.Experience = player.Experience;
                            break;
                        case "2":
                            loggedIn = false;
                            currentUser = null;
                            Console.WriteLine("\n>>> Wylogowano. <<<\n");
                            break;
                        case "3":
                            exit = true;
                            Console.WriteLine("\n>>> Dziêkujemy za grê! <<<\n");
                            break;
                        default:
                            Console.WriteLine("\n>>> Nieprawid³owa opcja. Spróbuj ponownie. <<<\n");
                            break;
                    }
                }
            }

            userManager.SaveUsers();
        }
    }

    class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public UserProgress Progress { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Progress = new UserProgress();
        }

        public bool ValidatePassword(string password)
        {
            return Password == password;
        }
    }

    class UserProgress
    {
        public int Level { get; set; }
        public int Experience { get; set; }

        public UserProgress()
        {
            Level = 1;
            Experience = 0;
        }
    }

    class UserManager
    {
        private List<User> users;
        private const string FilePath = "../../../users.txt";

        public UserManager()
        {
            users = new List<User>();
        }

        public void RegisterUser()
        {
            Console.Write("\nPodaj nazwê u¿ytkownika: ");
            string username = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("\n>>> Nazwa u¿ytkownika nie mo¿e byæ pusta! <<<\n");
                return;
            }

            Console.Write("Podaj has³o: ");
            string password = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\n>>> Has³o nie mo¿e byæ puste! <<<\n");
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("\n>>> U¿ytkownik o takiej nazwie ju¿ istnieje! <<<\n");
                return;
            }

            users.Add(new User(username, password));
            Console.WriteLine("\n>>> Rejestracja zakoñczona sukcesem! <<<\n");
        }

        public User LoginUser()
        {
            Console.Write("\nPodaj nazwê u¿ytkownika: ");
            string username = Console.ReadLine()?.Trim();
            Console.Write("Podaj has³o: ");
            string password = Console.ReadLine()?.Trim();

            User user = users.Find(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("\n>>> Nie znaleziono u¿ytkownika o podanej nazwie. <<<\n");
                return null;
            }

            if (!user.ValidatePassword(password))
            {
                Console.WriteLine("\n>>> Nieprawid³owe has³o. <<<\n");
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
                Console.WriteLine("\n>>> Dane u¿ytkowników zapisano. <<<\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n>>> B³¹d zapisywania danych: {ex.Message} <<<\n");
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
                    Console.WriteLine("\n>>> Dane u¿ytkowników wczytano. <<<\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n>>> B³¹d wczytywania danych: {ex.Message} <<<\n");
                }
            }
        }
    }

    abstract class Entity
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
            foreach (var item in Inventory)
            {
                Console.WriteLine($"{item.Name} - {item.Description} (Moc: {item.Power})");
            }
        }
    }

    class Character : Entity
    {
        public string Class { get; set; }
        public int Experience { get; private set; }

        public Character(string name, string charClass)
            : base(name, 1, 0, 0)
        {
            Class = charClass;
            Experience = 0;

            switch (Class.ToLower())
            {
                case "mag":
                    MaxHP = 80;
                    AttackPower = 20;
                    break;
                case "³ucznik":
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
    }

    class Enemy : Entity
    {
        private static Random random = new Random();
        private List<string> attackTypes = new List<string> { "Ciêcie", "Ugryzienie", "Taran", "Kula Ognia" };

        public Enemy(string name, int level, int maxHP, int attackPower)
            : base(name, level, maxHP, attackPower) { }

        public string GetRandomAttack()
        {
            int index = random.Next(attackTypes.Count);
            return attackTypes[index];
        }
    }

    class Menu
    {
        public void ShowAuthMenu()
        {
            Console.WriteLine("\n=== Menu Logowania ===");
            Console.WriteLine("1. Zarejestruj u¿ytkownika");
            Console.WriteLine("2. Zaloguj siê");
            Console.WriteLine("3. WyjdŸ");
            Console.Write("Wybierz opcjê: ");
        }

        public void ShowMainMenu()
        {
            Console.WriteLine("\n=== G³ówne Menu ===");
            Console.WriteLine("1. Rozpocznij bitwê");
            Console.WriteLine("2. Wyloguj siê");
            Console.WriteLine("3. WyjdŸ z gry");
            Console.Write("Wybierz opcjê: ");
        }

        public Character CreateCharacter()
        {
            Console.WriteLine("\n=== Tworzenie Postaci ===");
            Console.Write("Podaj nazwê postaci: ");
            string name = Console.ReadLine()?.Trim();

            Console.WriteLine("Wybierz klasê postaci:");
            Console.WriteLine("1. Mag");
            Console.WriteLine("2. £ucznik");
            Console.WriteLine("3. Wojownik");
            Console.Write("Wybierz opcjê: ");

            string classChoice = Console.ReadLine()?.Trim();
            string charClass = classChoice switch
            {
                "1" => "mag",
                "2" => "³ucznik",
                "3" => "wojownik",
                _ => throw new ArgumentException("Nieprawid³owy wybór klasy postaci")
            };

            Console.WriteLine($"\n>>> Utworzono postaæ: {name}, Klasa: {charClass} <<<\n");
            return new Character(name, charClass);
        }
    }

    class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Power { get; set; }

        public Item(string name, string description, int power)
        {
            Name = name;
            Description = description;
            Power = power;
        }
    }

    class Game
    {
        public void Run()
        {
            Character mainCharacter = new Character("John", "mag");
            mainCharacter.AddItem(new Item("Magic Wand", "A powerful wand", 50));
            mainCharacter.AddItem(new Item("Health Potion", "Restores 50 HP", 50));

            Console.WriteLine("Ekwipunek gracza:");
            mainCharacter.DisplayInventory();
        }

        public void StartBattle(Character player, Enemy enemy)
        {
            Console.WriteLine($"\n=== Rozpoczyna siê bitwa! ===");
            Console.WriteLine($"Przeciwnik: {enemy.Name} (HP: {enemy.HP}/{enemy.MaxHP}, Atak: {enemy.AttackPower})");
            Console.WriteLine("Ekwipunek przeciwnika:");
            enemy.DisplayInventory();

            while (player.HP > 0 && !enemy.IsDefeated())
            {
                Console.WriteLine($"\nTwój HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine("1. Atakuj");
                Console.WriteLine("2. Ucieczka");
                Console.Write("Wybierz opcjê: ");
                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        int playerDamage = player.AttackPower;
                        Console.WriteLine($"\n>>> Zadajesz {playerDamage} obra¿eñ przeciwnikowi! <<<");
                        enemy.TakeDamage(playerDamage);

                        if (!enemy.IsDefeated())
                        {
                            int enemyDamage = enemy.AttackPower;
                            Console.WriteLine($">>> Przeciwnik kontratakuje i zadaje {enemyDamage} obra¿eñ! <<<\n");
                            player.TakeDamage(enemyDamage);
                        }
                        break;

                    case "2":
                        Console.WriteLine("\n>>> Uciekasz z walki! <<<\n");
                        return;

                    default:
                        Console.WriteLine("\n>>> Nieprawid³owa opcja. Spróbuj ponownie. <<<\n");
                        break;
                }
            }

            if (player.HP <= 0)
                Console.WriteLine("\n>>> Zosta³eœ pokonany! <<<\n");
            else if (enemy.IsDefeated())
                Console.WriteLine($"\n>>> Pokona³eœ przeciwnika: {enemy.Name}! <<<\n");
        }
    }
}
