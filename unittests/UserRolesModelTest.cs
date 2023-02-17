using Epracownik.Models;
using Xunit;

//powy¿szym teœcie tworzymy nowy obiekt UserRole i testujemy, czy w³aœciwoœci IdUser,
//    IdRole, IdRoleNavigation i IdUserNavigation s¹ ustawiane i pobierane poprawnie.
namespace Epracownik.Tests
{
    public class UserRoleTests
    {
        [Fact]
        public void UserRole_ShouldCreateUserRole()
        {
            // Arrange
            var userRole = new UserRole();

            // Act

            // Assert
            Assert.NotNull(userRole);
            Assert.IsType<UserRole>(userRole);
        }

        [Fact]
        public void UserRole_ShouldSetAndGetIdUser()
        {
            // Arrange
            var userRole = new UserRole();
            var expectedIdUser = 1;

            // Act
            userRole.IdUser = expectedIdUser;
            var actualIdUser = userRole.IdUser;

            // Assert
            Assert.Equal(expectedIdUser, actualIdUser);
        }

        [Fact]
        public void UserRole_ShouldSetAndGetIdRole()
        {
            // Arrange
            var userRole = new UserRole();
            var expectedIdRole = 2;

            // Act
            userRole.IdRole = expectedIdRole;
            var actualIdRole = userRole.IdRole;

            // Assert
            Assert.Equal(expectedIdRole, actualIdRole);
        }

        [Fact]
        public void UserRole_ShouldSetAndGetIdRoleNavigation()
        {
            // Arrange
            var userRole = new UserRole();
            var expectedRole = new Role { Id = 3 };

            // Act
            userRole.IdRoleNavigation = expectedRole;
            var actualRole = userRole.IdRoleNavigation;

            // Assert
            Assert.Equal(expectedRole, actualRole);
        }

        [Fact]
        public void UserRole_ShouldSetAndGetIdUserNavigation()
        {
            // Arrange
            var userRole = new UserRole();
            var expectedUser = new User { Id = 4 };

            // Act
            userRole.IdUserNavigation = expectedUser;
            var actualUser = userRole.IdUserNavigation;

            // Assert
            Assert.Equal(expectedUser, actualUser);
        }
    }
}