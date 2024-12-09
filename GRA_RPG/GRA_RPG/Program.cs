using GRA_RPG;
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
            bool loggedIn = false;
            User currentUser = null;

            while (!exit)
            {
                if (!loggedIn)
                {
                    menu.ShowAuthMenu();
                    string choice = Console.ReadLine();

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
                                Console.WriteLine($"Zalogowano jako {currentUser.Username}.");
                            }
                            break;
                        case "3":
                            exit = true;
                            Console.WriteLine("Dziêkujemy za skorzystanie z programu!");
                            break;
                        default:
                            Console.WriteLine("Nieprawid³owa opcja. Spróbuj ponownie.");
                            break;
                    }
                }
                else
                {
                    menu.ShowMainMenu();
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Character player = new Character("Gracz", "mag");
                            Enemy enemy = new Enemy("Goblin", 1, 50, 10);
                            Game game = new Game();
                            game.StartBattle(player, enemy);
                            break;
                        case "2":
                            loggedIn = false;
                            currentUser = null;
                            Console.WriteLine("Wylogowano.");
                            break;
                        case "3":
                            exit = true;
                            Console.WriteLine("Dziêkujemy za grê!");
                            break;
                        default:
                            Console.WriteLine("Nieprawid³owa opcja. Spróbuj ponownie.");
                            break;
                    }
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

        public bool ValidatePassword(string password)
        {
            return Password == password;
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
            Console.Write("Podaj nazwê u¿ytkownika: ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Nazwa u¿ytkownika nie mo¿e byæ pusta!");
                return;
            }

            Console.Write("Podaj has³o: ");
            string password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Has³o nie mo¿e byæ puste!");
                return;
            }

            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("U¿ytkownik o takiej nazwie ju¿ istnieje!");
                return;
            }

            users.Add(new User(username, password));
            Console.WriteLine("Rejestracja zakoñczona sukcesem!");
        }

        public User LoginUser()
        {
            Console.Write("Podaj nazwê u¿ytkownika: ");
            string username = Console.ReadLine();
            Console.Write("Podaj has³o: ");
            string password = Console.ReadLine();

            User user = users.Find(u => u.Username == username);
            if (user == null)
            {
                Console.WriteLine("Nie znaleziono u¿ytkownika o podanej nazwie.");
                return null;
            }

            if (!user.ValidatePassword(password))
            {
                Console.WriteLine("Nieprawid³owe has³o.");
                return null;
            }

            return user;
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
class Game
{
    public void StartBattle(Character player, Enemy enemy)
    {
        Console.WriteLine($"\n=== Rozpoczêcie bitwy: {player.Name} vs {enemy.Name} ===");
        Console.WriteLine($"Statystyki {player.Name}: HP: {player.HP}/{player.MaxHP}, Atak: {player.AttackPower}");
        Console.WriteLine($"Statystyki {enemy.Name}: HP: {enemy.HP}/{enemy.MaxHP}, Atak: {enemy.AttackPower}");

        while (!player.IsDefeated() && !enemy.IsDefeated())
        {
            Console.WriteLine("\n=== Tura Gracza ===");
            Console.WriteLine("1. Atakuj");
            Console.WriteLine("2. Wyœwietl ekwipunek");
            Console.WriteLine("Wybierz opcjê: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Attack(player, enemy);
                    break;
                case "2":
                    player.DisplayInventory();
                    break;
                default:
                    Console.WriteLine("Nieprawid³owa opcja! Tracisz kolejkê.");
                    break;
            }

            if (enemy.IsDefeated())
            {
                Console.WriteLine($"\nGratulacje! Pokona³eœ {enemy.Name}!");
                break;
            }

            Console.WriteLine("\n=== Tura Wroga ===");
            EnemyAttack(player, enemy);

            if (player.IsDefeated())
            {
                Console.WriteLine("\nZosta³eœ pokonany! Gra zakoñczona.");
                break;
            }
        }
    }

    private void Attack(Entity attacker, Entity defender)
    {
        Console.WriteLine($"\n{attacker.Name} atakuje {defender.Name} i zadaje {attacker.AttackPower} obra¿eñ!");
        defender.TakeDamage(attacker.AttackPower);
        Console.WriteLine($"{defender.Name} ma teraz {defender.HP}/{defender.MaxHP} HP.");
    }

    private void EnemyAttack(Entity player, Enemy enemy)
    {
        string attackType = enemy.GetRandomAttack();
        Console.WriteLine($"\n{enemy.Name} u¿ywa ataku: {attackType} i zadaje {enemy.AttackPower} obra¿eñ!");
        player.TakeDamage(enemy.AttackPower);
        Console.WriteLine($"{player.Name} ma teraz {player.HP}/{player.MaxHP} HP.");
    }
}
