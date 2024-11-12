
namespace Gra
{
    class Postac
    {
        public string nazwa { get; set; } //nazwa gracza
        public string Klasa_postaci { get; set; } // klasa postaci na przyklad jakis mag czy cos(trzeba wymyslic)
        public int Poziom { get; set; } // poziom doswiadczenia lvl
        public int HP { get; set; } // ilosc zdrowia jaka mamu
        public int obrazenia { get; set; } // ile zadajemy obrazen
        public int doswiadczenie { get; set; } = 0; // ile mamy doswiadczenia dana ilosc potrzebna do wbicia poziomu z kazdym poziomem bedzie sie zwieszkac wymagana ilosc


    }
}
