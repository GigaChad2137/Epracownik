using Epracownik.Models;
using System;
using Xunit;


//W powy¿szym teœcie definiujemy dwie metody, które testuj¹ ustawianie
//wartoœci w³aœciwoœci DataRozpoczecia oraz DataZakonczenia.

//W pierwszej metodzie Praca_SetDataRozpoczecia_CorrectValueSet
//tworzymy nowy obiekt Praca, a nastêpnie ustawiamy wartoœæ w³aœciwoœci
//DataRozpoczecia na konkretn¹ datê (2022-03-01). Na koñcu testu sprawdzamy,
//czy w³aœciwoœæ DataRozpoczecia faktycznie ma wartoœæ ustawion¹ wczeœniej.

//W drugiej metodzie Praca_SetDataZakonczenia_CorrectValueSet postêpujemy
//analogicznie, ale ustawiamy wartoœæ w³aœciwoœci DataZakonczenia na datê 2022-04-30.

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
