using Epracownik.Models;
using System;
using Xunit;


//W powy�szym te�cie definiujemy dwie metody, kt�re testuj� ustawianie
//warto�ci w�a�ciwo�ci DataRozpoczecia oraz DataZakonczenia.

//W pierwszej metodzie Praca_SetDataRozpoczecia_CorrectValueSet
//tworzymy nowy obiekt Praca, a nast�pnie ustawiamy warto�� w�a�ciwo�ci
//DataRozpoczecia na konkretn� dat� (2022-03-01). Na ko�cu testu sprawdzamy,
//czy w�a�ciwo�� DataRozpoczecia faktycznie ma warto�� ustawion� wcze�niej.

//W drugiej metodzie Praca_SetDataZakonczenia_CorrectValueSet post�pujemy
//analogicznie, ale ustawiamy warto�� w�a�ciwo�ci DataZakonczenia na dat� 2022-04-30.

public class PracaTests
{ 

    [Fact]
    public void Praca_SetDataRozpoczecia_CorrectValueSet()
    {
        // Arrange
        var praca = new Praca();
        var dataRozpoczecia = new DateTime(2022, 03, 01);

        // Act
        praca.DataRozpoczecia = dataRozpoczecia;

        // Assert
        Assert.Equal(dataRozpoczecia, praca.DataRozpoczecia);
    }

    [Fact]
    public void Praca_SetDataZakonczenia_CorrectValueSet()
    {
        // Arrange
        var praca = new Praca();
        var dataZakonczenia = new DateTime(2022, 04, 30);

        // Act
        praca.DataZakonczenia = dataZakonczenia;

        // Assert
        Assert.Equal(dataZakonczenia, praca.DataZakonczenia);
    }
}
