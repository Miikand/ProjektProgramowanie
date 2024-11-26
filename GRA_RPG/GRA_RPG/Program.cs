using System;

namespace GRA_RPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            Menu menu = new Menu(); bool exit = false; while (!exit)
            {
                menu.ShowMainMenu(); string choice = Console.ReadLine(); switch (choice)
                {
                    case "1": menu.ShowClasses(); break;
                    case "2": break;
                    case "3": break;
                    case "4": exit = true; break;
                    default: Console.WriteLine("Nieprawidłowa opcja. Spróbuj ponownie."); break;
                }
                List<Enemy> enemies = new List<Enemy>();
                //dodanie kilku przeciwnikow na start
                enemies.Add(new Enemy("Goblin", 1, 50, 10));
                enemies.Add(new Enemy("Wilk", 2, 70, 15));
                enemies.Add(new Enemy("Ogr", 3, 120, 20));
            }
        }

        class Character
        {
            public string Name { get; set; }
            public string Class { get; set; }
            public int Level { get; private set; }
            public int Experience { get; private set; }
            public int HP { get; private set; }
            public int MaxHP { get; private set; }
            public int AttackPower { get; private set; }

            public Character(string name, string charClass)
            {
                Name = name;
                Class = charClass;
                Level = 1;
                Experience = 0;

                // początkowe wartosci postaci w zależności od klasy
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

        class Enemy
        {
            public string Name { get; set; }
            public int Level { get; private set; }
            public int HP { get; private set; }
            public int MaxHP { get; private set; }
            public int AttackPower { get; private set; }

            public Enemy(string name, int level, int maxHP, int attackPower)
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
    }
}