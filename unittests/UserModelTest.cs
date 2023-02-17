using Epracownik.Models;
using Xunit;

//W tym te�cie tworzony jest nowy obiekt klasy User, a nast�pnie ustawiane s�
//    jego w�a�ciwo�ci. W asercjach sprawdzane s� warto�ci tych w�a�ciwo�ci
//    , czy s� zgodne z oczekiwaniami. W tym przypadku test sprawdza poprawno��
//    tworzenia obiektu User, ale w rzeczywistych aplikacjach testy 
//    jednostkowe powinny pokrywa� szerszy zakres funkcjonalno�ci danej klasy.
public class UserTests
{
    [Fact]
    public void User_Instantiation_Succeeds()
    {
        // Arrange
        var user = new User();

        // Act
        user.Id = 1;
        user.Username = "john_doe";
        user.Password = "password";
        var infoPersonalne = new InformacjePersonalne { Imie = "John", Nazwisko = "Doe" };
        user.InformacjePersonalne = infoPersonalne;
        var userRole = new UserRole {IdUser = user.Id ,IdRole = 1 };
        user.UserRole = userRole;

        // Assert
        Assert.Equal(1, user.Id);
        Assert.Equal("john_doe", user.Username);
        Assert.Equal("password", user.Password);
        Assert.Equal(infoPersonalne, user.InformacjePersonalne);
        Assert.Equal(userRole, user.UserRole);
    }
}