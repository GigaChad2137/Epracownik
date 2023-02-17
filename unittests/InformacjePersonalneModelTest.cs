using Epracownik.Models;
using Xunit;

namespace Epracownik.Tests.Models
{
    public class InformacjePersonalneTests
    {
        [Fact]
        public void CanCreateInformacjePersonalne()
        {
            // Arrange
            var ip = new InformacjePersonalne
            {
                IdPracownika = 1,
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Zarobki = 3000,
                DniUrlopowe = 20,
                DataZatrudnienia = new System.DateTime(2021, 1, 1)
            };

            // Act

            // Assert
            Assert.Equal(1, ip.IdPracownika);
            Assert.Equal("Jan", ip.Imie);
            Assert.Equal("Kowalski", ip.Nazwisko);
            Assert.Equal(3000, ip.Zarobki);
            Assert.Equal(20, ip.DniUrlopowe);
            Assert.Equal(new System.DateTime(2021, 1, 1), ip.DataZatrudnienia);
        }
    }
}
