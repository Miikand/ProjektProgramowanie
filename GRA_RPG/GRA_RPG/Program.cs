using System;
using System.Collections.Generic;

namespace GRA_RPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
                        menu.ShowClasses();
                        break;
                    case "2":
                        Console.WriteLine("Pokaż wszystkie postacie - funkcjonalność do zaimplementowania.");
                        break;
                    case "3":
                        // Przykładowe rozpoczęcie bitwy
                        Character player = new Character("Gracz", "mag");
                        Enemy enemy = new Enemy("Goblin", 1, 50, 10);
                        Game game = new Game();
                        game.StartBattle(player, enemy);
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("Dziękujemy za grę!");
                        break;
                    case "5":
                        Game g = new Game();
                        g.Run();
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                        break;
                }
            }
        }
    }

    // Bazowa klasa Entity
    abstract class Entity
    {
        public string Name { get; set; }
        public int Level { get; protected set; }
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; }
        public int AttackPower { get; protected set; }

        public Entity(string name, int level, int maxHP, int attackPower)
        {
            Name = name;
            Level = level;
            MaxHP = maxHP;
            HP = maxHP;
            AttackPower = attackPower;
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            if (HP < 0) HP = 0;
            Console.WriteLine($"{Name} otrzymał {damage} obrażeń. Pozostało HP: {HP}/{MaxHP}.");
        }

        public bool IsDefeated()
        {
            return HP <= 0;
        }
    }

    // Klasa Character dziedziczy z Entity
    class Character : Entity
    {
        public string Class { get; set; }
        public int Experience { get; private set; }
        public List<Item> Inventory { get; private set; }

        public Character(string name, string charClass) 
            : base(name, 1, 0, 0) // Przekazujemy początkowe wartości do Entity
        {
            Class = charClass;
            Experience = 0;
            Inventory = new List<Item>();

            switch (Class.ToLower())
            {
                case "mag":
                    MaxHP = 80;
                    AttackPower = 20;
                    break;
                case "łucznik":
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

        public void AddItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"{item.Name} dodano do ekwipunku.");
        }

        public void DisplayInventory()
        {
            Console.WriteLine("=== Ekwipunek ===");
            foreach (var item in Inventory)
            {
                Console.WriteLine($"{item.Name} - {item.Description} (Moc: {item.Power})");
            }
        }
    }

    // Klasa Enemy dziedziczy z Entity
    class Enemy : Entity
    {
        public Enemy(string name, int level, int maxHP, int attackPower) 
            : base(name, level, maxHP, attackPower) { }
    }

    class Menu
    {
        public void ShowMainMenu()
        {
            Console.WriteLine("=== Główne Menu ===");
            Console.WriteLine("1. Stwórz nową postać");
            Console.WriteLine("2. Pokaż wszystkie postacie");
            Console.WriteLine("3. Rozpocznij bitwę");
            Console.WriteLine("4. Wyjdź z gry");
            Console.Write("Wybierz opcję: ");
        }

        public void ShowClasses()
        {
            Console.WriteLine("=== Klasy Postaci ===");
            Console.WriteLine("1. Mag - wysoki atak, niskie HP");
            Console.WriteLine("2. Łucznik - średni atak, średnie HP");
            Console.WriteLine("3. Wojownik - niski atak, wysokie HP");
            Console.Write("Wybierz klasę postaci: ");
        }
    }

    class Game
    {
        public void Run()
        {
            Character mainCharacter = new Character("John", "mag");
            mainCharacter.AddItem(new Item("Magic Wand", "A powerful wand", 50));
            mainCharacter.AddItem(new Item("Health Potion", "Restores 50 HP", 50));
            mainCharacter.DisplayInventory();
        }

        public void StartBattle(Character player, Enemy enemy)
        {
            Console.WriteLine($"\n=== Rozpoczyna się bitwa! ===");
            Console.WriteLine($"Przeciwnik: {enemy.Name} (HP: {enemy.HP}/{enemy.MaxHP}, Atak: {enemy.AttackPower})");

            while (player.HP > 0 && !enemy.IsDefeated())
            {
                Console.WriteLine($"\nTwój HP: {player.HP}/{player.MaxHP}");
                Console.WriteLine("1. Atakuj");
                Console.WriteLine("2. Ucieczka");
                Console.Write("Wybierz opcję: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        int playerDamage = player.AttackPower;
                        Console.WriteLine($"Zadajesz {playerDamage} obrażeń przeciwnikowi!");
                        enemy.TakeDamage(playerDamage);

                        if (!enemy.IsDefeated())
                        {
                            int enemyDamage = enemy.AttackPower;
                            Console.WriteLine($"Przeciwnik kontratakuje i zadaje {enemyDamage} obrażeń!");
                            player.TakeDamage(enemyDamage);
                        }
                        break;

                    case "2":
                        Console.WriteLine("Uciekasz z walki!");
                        return;

                    default:
                        Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie.");
                        break;
                }
            }

            if (player.HP <= 0)
                Console.WriteLine("Zostałeś pokonany!");
            else if (enemy.IsDefeated())
                Console.WriteLine($"Pokonałeś przeciwnika: {enemy.Name}!");
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
