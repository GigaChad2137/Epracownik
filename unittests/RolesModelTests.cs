using Epracownik.Models;
using System.Collections.Generic;
using Xunit;
//Czy konstruktor klasy Role tworzy kolekcjê UserRoles o oczekiwanej wartoœci.
//Czy w³aœciwoœci Id i Role1 klasy Role s¹ ustawiane i zwracane poprawnie.
//Czy obiekty klasy UserRole s¹ dodawane do kolekcji UserRoles klasy Role.
namespace Epracownik.Tests.Models
{
    public class RoleTests
    {
        [Fact]
        public void RoleConstructor_SetsUserRolesToEmptyCollection()
        {
            // Arrange
            var role = new Role();

            // Act

            // Assert
            Assert.NotNull(role.UserRoles);
            Assert.IsType<HashSet<UserRole>>(role.UserRoles);
            Assert.Empty(role.UserRoles);
        }

        [Fact]
        public void RoleProperties_SetAndGetCorrectly()
        {
            // Arrange
            var role = new Role();

            // Act
            role.Id = 1;
            role.Role1 = "Admin";

            // Assert
            Assert.Equal(1, role.Id);
            Assert.Equal("Admin", role.Role1);
        }

        [Fact]
        public void UserRoles_AddedToCollection()
        {
            // Arrange
            var role = new Role();
            var userRole = new UserRole();

            // Act
            role.UserRoles.Add(userRole);

            // Assert
            Assert.Single(role.UserRoles);
            Assert.Contains(userRole, role.UserRoles);
        }
    }
}
