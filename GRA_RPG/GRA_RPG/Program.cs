using System;
using System.Collections.Generic;

namespace GRA_RPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            UserManager userManager = new UserManager();
            Menu menu = new Menu();
            List<Enemy> enemies = new List<Enemy>();
            bool exit = false;

            while (!exit)
            {
                menu.ShowMainMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        userManager.RegisterUser();
                        break;
                    case "2":
                        userManager.ShowAllUsers();
                        break;
                    case "3":
                        Character player = new Character("Gracz", "mag");
                        Enemy enemy = new Enemy("Goblin", 1, 50, 10);
                        Game game = new Game();
                        game.StartBattle(player, enemy);
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("Dzi�kujemy za gr�!");
                        break;
                    default:
                        Console.WriteLine("Nieprawid�owa opcja. Spr�buj ponownie.");
                        break;
                }
            }
        }
    }

    class User
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    class UserManager
    {
        private List<User> users;

        public UserManager()
        {
            users = new List<User>();
        }

        public void RegisterUser()
        {
            Console.Write("Podaj nazw� u�ytkownika: ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Nazwa u�ytkownika nie mo�e by� pusta!");
                return;
            }

            Console.Write("Podaj has�o: ");
            string password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Has�o nie mo�e by� puste!");
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("U�ytkownik o takiej nazwie ju� istnieje!");
                return;
            }

            users.Add(new User(username, password));
            Console.WriteLine("Rejestracja zako�czona sukcesem!");
        }

        public void ShowAllUsers()
        {
            Console.WriteLine("\n=== Zarejestrowani U�ytkownicy ===");
            if (users.Count == 0)
            {
                Console.WriteLine("Brak zarejestrowanych u�ytkownik�w.");
                return;
            }

            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Username}");
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
                case "�ucznik":
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
        private List<string> attackTypes = new List<string> { "Ci�cie", "Ugryzienie", "Taran", "Kula Ognia" };

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
        public void ShowMainMenu()
        {
            Console.WriteLine("\n=== G��wne Menu ===");
            Console.WriteLine("1. Zarejestruj u�ytkownika");
            Console.WriteLine("2. Wy�wietl u�ytkownik�w");
            Console.WriteLine("3. Rozpocznij bitw�");
            Console.WriteLine("4. Wyjd� z gry");
            Console.Write("Wybierz opcj�: ");
        }
    }

    class Game
    {
        public void Run()
        {
            Character mainCharacter = new Character("John", "mag");
            mainCharacter.AddItem(new Item("Magic Wand", "Pot�na r�d�ka", 50));
            mainCharacter.AddItem(new Item("Mikstura Zdrowia", "Przywraca 50 HP", 50));
            Console.WriteLine("Ekwipunek gracza:");
            mainCharacter.DisplayInventory();
        }

        public void StartBattle(Character player, Enemy enemy)
        {
            enemy.AddItem(new Item("Zardzewia�y Sztylet", "S�aby sztylet", 10));
            enemy.AddItem(new Item("Fragment Tarczy", "Cz�� z�amanej tarczy", 5));

            Console.WriteLine($"\n=== Rozpoczyna si� bitwa! ===");
            Console.WriteLine($"Przeciwnik: {enemy.Name} (HP: {enemy.HP}/{enemy.MaxHP}, Atak: {enemy.AttackPower})");
            Console.WriteLine("Ekwipunek przeciwnika:");
            enemy.DisplayInventory();

            while (player.HP > 0 && !enemy.IsDefeated())
            {
                Console.WriteLine($"\nTw�j HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine("1. Atakuj");
                Console.WriteLine("2. Ucieczka");
                Console.Write("Wybierz opcj�: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        int playerDamage = player.AttackPower;
                        Console.WriteLine($"Zadajesz {playerDamage} obra�e� przeciwnikowi!");
                        enemy.TakeDamage(playerDamage);

                        if (!enemy.IsDefeated())
                        {
                            string enemyAttack = enemy.GetRandomAttack();
                            int enemyDamage = enemy.AttackPower;
                            Console.WriteLine($"Przeciwnik u�ywa {enemyAttack} i zadaje {enemyDamage} obra�e�!");
                            player.TakeDamage(enemyDamage);
                        }
                        break;

                    case "2":
                        Console.WriteLine("Uciekasz z walki!");
                        return;

                    default:
                        Console.WriteLine("Nieprawid�owa opcja. Spr�buj ponownie.");
                        break;
                }
            }

            if (player.HP <= 0)
                Console.WriteLine("Zosta�e� pokonany!");
            else if (enemy.IsDefeated())
                Console.WriteLine($"Pokona�e� przeciwnika: {enemy.Name}!");
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
}
