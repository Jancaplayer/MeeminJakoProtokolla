using System;
using System.Net;
using System.Net.Sockets;

namespace Asiakas;

public class Asiakas
{
    public static void Main()
    {
        //käynnistä
        Socket soketti = Yhdistä();
        Kirjaudu();
        //jos kirjautunut
        Listaa();
        Kerää();
        Lataa();
        Lähetä();
        Poista();
        Vaihda();
        //
        soketti.Close();
    }

    /// <summary>
    /// Yhdistää soketin
    /// </summary>
    /// <param name="ip">osoite</param>
    /// <param name="portti">portti</param>
    /// <returns>toimivan soketin</returns>
    public static Socket Yhdistä(string ip, int portti)
    {
        return null;
    }

    /// <summary>
    /// kirjautuu palvelimelle
    /// </summary>
    /// <param name="psw">salasanatunnus</param>
    /// <returns>palautteen onnistumisesta</returns>
    public static string Kirjaudu(string psw)
    {
        return null;
    }

    /// <summary>
    /// listaa hakua vastanneiden tiedostojen indeksit
    /// </summary>
    public static void Listaa()
    {

    }

    /// <summary>
    /// kerää thumbnailit listalle indekseistä
    /// </summary>
    public static void Kerää()
    {

    }

    /// <summary>
    /// lataa itse tiedoston datan
    /// </summary>
    public static void Lataa()
    {

    }

    /// <summary>
    /// lähettää uuden tiedoston palvelimelle
    /// </summary>
    public static void Lähetä()
    {

    }

    /// <summary>
    /// poistaa indeksiä vastaavan tiedoston palvelimelta
    /// </summary>
    public static void Poista()
    {

    }

    /// <summary>
    /// vaihtaa indeksiä vastaavan tiedoston avainsanat
    /// </summary>
    public static void Vaihda()
    {

    }

    /// <summary>
    /// tilakone
    /// </summary>
    /// <param name="tila">tunnettu tila</param>
    /// <param name="servViesti">palvelimen lähettämä viesti</param>
    /// <returns>uuden tilan ja palvelimelle lähetettävän viestin muodon</returns>
    public static (string, string) Tila(string tila, string servViesti)
    {
        string vastaus = string.Empty;
        bool error = false;
        string[] servOsat = servViesti.Split("|");
        switch (tila)
        {
            case "0":
                switch (servOsat[0])
                {
                    case "GREETINGS":
                        vastaus = "user123#"; //userpassword
                        break;
                    case "WHITELIST":
                        tila = servOsat[1] == "OK" ? "1" : "0";
                        vastaus = servOsat[1] == "OK" ? "Noted" : "Quit";
                        break;

                    case "QUIT":
                        vastaus = "Quit";
                        tila = "0";
                        break;
                    default:
                        error = true;
                        break;
                }
                break;
            case "1":
                switch (servOsat[0])
                {
                    case "APPROVED":
                        //suorita komento
                        tila = "01";
                        break;

                    case "QUIT":
                        vastaus = "Quit";
                        tila = "0";
                        break;
                    default:
                        error = true;
                        break;
                }
                break;
            case "1.1":
                switch (servOsat[0])
                {
                    case "NOTED":
                        //suorita komento
                        tila = "01";
                        break;
                    case "RECEIVED":
                        //jatka datan lähetystä
                        tila = "001";
                        break;
                    case "SAVED":
                        //suorita komento
                        tila = "01";
                        break;

                    case "QUIT":
                        vastaus = "Quit";
                        tila = "0";
                        break;
                    default:
                        error = true;
                        break;
                }
                break;

            default:
                error = true;
                break;
        }
        if (error)
        {
            tila = "error";
        }
        return (tila, vastaus);
    }
}