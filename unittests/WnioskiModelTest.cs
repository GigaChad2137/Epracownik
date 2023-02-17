using Epracownik.Models;
using System.Collections.Generic;
using Xunit;

//Pierwszy test sprawdza, czy kolekcja UserWnioskis jest 
//    inicjalizowana jako pusta kolekcja typu HashSet<UserWnioski>.

//Drugi test sprawdza, czy wartoœæ przypisana do w³aœciwoœci 
//TypWniosku jest poprawnie ustawiona i zwrócona.
namespace Epracownik.Tests.Models
{
    public class WnioskiTests
    {
        [Fact]
        public void Wnioski_InitializeWithEmptyUserWnioskis_ShouldCreateEmptyCollection()
        {
            // Arrange
            var wnioski = new Wnioski();

            // Act
            var userWnioskis = wnioski.UserWnioskis;

            // Assert
            Assert.NotNull(userWnioskis);
            Assert.IsType<HashSet<UserWnioski>>(userWnioskis);
            Assert.Empty(userWnioskis);
        }

        [Fact]
        public void Wnioski_InitializeWithTypWniosku_ShouldSetTypWnioskuProperty()
        {
            // Arrange
            var typWniosku = "Urlop";
            var wnioski = new Wnioski
            {
                TypWniosku = typWniosku
            };

            // Act
            var result = wnioski.TypWniosku;

            // Assert
            Assert.Equal(typWniosku, result);
        }
    }
}
