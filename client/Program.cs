using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Asiakas;

public class Asiakas
{
    public static void Main()
    {
        //käynnistä
        //Socket soketti = Yhdistä();
        //Kirjaudu();
        //jos kirjautunut
        Listaa();
        Kerää();
        Lataa();
        Lähetä();
        Poista();
        Vaihda();
        //
        //soketti.Close();
        string x, y, z;
        (x, y, z) = haeConfig();
        Console.WriteLine($"{x} {y} {z}");
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
    /// hakee thumbnailin indeksille
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

    public static (string, string, string) haeConfig()
    {
        string server = string.Empty;
        string port = string.Empty;
        string userid = string.Empty;

        string dir = Directory.GetCurrentDirectory();
        string fileName = dir + @"\config.json";
        Console.WriteLine(fileName);
        byte[] data = File.ReadAllBytes(fileName);
        System.Text.Json.Utf8JsonReader reader = new System.Text.Json.Utf8JsonReader(data);

        string property = string.Empty;
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case System.Text.Json.JsonTokenType.StartObject:
                    break;
                case System.Text.Json.JsonTokenType.EndObject:
                    break;
                case System.Text.Json.JsonTokenType.EndArray:
                    break;
                case System.Text.Json.JsonTokenType.StartArray:
                    break;
                case System.Text.Json.JsonTokenType.PropertyName:
                    property = reader.GetString();
                    break;
                case System.Text.Json.JsonTokenType.String:
                    switch (property)
                    {
                        case "server":
                            server = reader.GetString();
                            break;
                        case "port":
                            port = reader.GetString();
                            break;
                        case "userid":
                            userid = reader.GetString();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    throw new ArgumentException();

            }
        }

        server = server == null ? string.Empty : server;
        port = port == null ? string.Empty : port;
        userid = userid == null ? string.Empty : userid;
        return (server, port, userid);
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