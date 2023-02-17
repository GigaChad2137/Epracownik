using Epracownik.Models;
using Xunit;

//W tym teœcie tworzony jest nowy obiekt klasy User, a nastêpnie ustawiane s¹
//    jego w³aœciwoœci. W asercjach sprawdzane s¹ wartoœci tych w³aœciwoœci
//    , czy s¹ zgodne z oczekiwaniami. W tym przypadku test sprawdza poprawnoœæ
//    tworzenia obiektu User, ale w rzeczywistych aplikacjach testy 
//    jednostkowe powinny pokrywaæ szerszy zakres funkcjonalnoœci danej klasy.
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