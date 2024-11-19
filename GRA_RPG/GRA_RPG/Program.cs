namespace GRA_RPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
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

    }
}

