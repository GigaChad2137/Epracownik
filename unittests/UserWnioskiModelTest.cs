using Epracownik.Models;
using Xunit;
//Ten test sprawdza, czy mo�na utworzy� nowy obiekt UserWnioski i
//czy w�a�ciwo�ci tego obiektu s� ustawione prawid�owo.
//W tym celu tworzy obiekty User, Wnioski i UserWnioski z przyk�adowymi danymi,
//a nast�pnie por�wnuje warto�ci poszczeg�lnych w�a�ciwo�ci obiektu UserWnioski
//z oczekiwanymi warto�ciami.
namespace Epracownik.Tests
{
    public class UserWnioskiTests
    {
        [Fact]
        public void CanCreateUserWnioski()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "janek",
                Password = "password123",
                InformacjePersonalne = new InformacjePersonalne
                {
                    IdPracownika = 1,
                    Imie = "Jan",
                    Nazwisko = "Kowalski",
                    Zarobki = 5000,
                    DniUrlopowe = 20,
                    DataZatrudnienia = new System.DateTime(2020, 1, 1)
                },
                UserRole = new UserRole
                {
                    IdUser = 1,
                    IdRole = 2
                }
            };
            var wniosek = new Wnioski
            {
                Id = 1,
                TypWniosku = "Urlop"
            };
            var userWniosek = new UserWnioski
            {
                Id = 1,
                IdPracownika = 1,
                IdWniosku = 1,
                DataRozpoczecia = new System.DateTime(2023, 2, 1),
                DataZakonczenia = new System.DateTime(2023, 2, 4),
                Notka = "Wniosek o urlop",
                Kwota = null,
                StatusWniosku = null,
                NotiC = 0,
                IdPracownikaNavigation = user,
                IdWnioskuNavigation = wniosek
            };

            // Assert
            Assert.Equal(1, userWniosek.Id);
            Assert.Equal(1, userWniosek.IdPracownika);
            Assert.Equal(1, userWniosek.IdWniosku);
            Assert.Equal(new System.DateTime(2023, 2, 1), userWniosek.DataRozpoczecia);
            Assert.Equal(new System.DateTime(2023, 2, 4), userWniosek.DataZakonczenia);
            Assert.Equal("Wniosek o urlop", userWniosek.Notka);
            Assert.Null(userWniosek.Kwota);
            Assert.Null(userWniosek.StatusWniosku);
            Assert.Equal(0, userWniosek.NotiC);
            Assert.Equal(user, userWniosek.IdPracownikaNavigation);
            Assert.Equal(wniosek, userWniosek.IdWnioskuNavigation);
        }
    }
}
