using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Asiakas;

public class Asiakas
{

    /// <summary>
    /// cli ympäristö protokollan testaamiselle.
    /// </summary>
    public static void Main()
    {
        //alustus
        bool kirjautunut = false;
        Socket soketti = null;
        string server, port, userid;
        (server, port, userid) = haeConfig();
        Console.WriteLine($"{server} {port} {userid}");


        string command = string.Empty;
        while (command.ToLower() != "quit")
        {
            if (soketti == null) { Console.WriteLine("\"yhdistä\" avataksesi yhteyden palvelimeen"); }
            if (!kirjautunut) { Console.WriteLine("\"kirjaudu\" kirjautuaksesi palvelimeen"); }
            Console.WriteLine("-------------------------------\n");
            Console.Write(": ");
            command = Console.ReadLine();
            command = command == null ? string.Empty : command;
            switch (command.ToLower())
            {
                case "yhdistä":
                    soketti = Yhdistä(server, int.Parse(port));
                    break;
                case "kirjaudu":
                    string kirjautuminen = Kirjaudu(userid);
                    kirjautunut = kirjautuminen == "success" ? true : false;
                    if (!kirjautunut)
                    {
                        Console.WriteLine(kirjautuminen);
                        throw new Exception();
                    }
                    break;
                default:
                    if (kirjautunut) { komentoKytkin(command); }
                    else { Console.WriteLine("komennot eivät käytössä ennen kuin on kirjauduttu"); }
                    break;
            }
            
        }


        if (soketti != null) { soketti.Close(); }
    }

    public static void komentoKytkin(string command)
    {
        switch (command.ToLower())
            {
                case "listaa":
                    break;
                case "kerää":
                    break;
                case "lataa":
                    break;
                case "lähetä":
                    break;
                case "poista":
                    break;
                case "vaihda":
                    break;
                case "quit":
                    Console.WriteLine("Quitting..");
                    break;
                case "help":
                    Console.WriteLine(
                    "Listaa: listaa hakutulokset avainsanojen mukaan\n" +
                    "Kerää : kerää data thumbnaileista, hyödytön komentorivillä\n" +
                    "Lataa : lataa indeksin mukaisen tiedoston (placeholder: tämänhetkiseen kansioon)\n"+
                    "Lähetä: lähettää uuden kuvatiedoston sen avainsanojen ja kategorian kanssa palvelimelle\n"+
                    "Poista: poistaa indeksin mukaisen kuvan palvelimelta\n"+
                    "Vaihda: vaihtaa indeksin avainsanat uusiin\n\n"+
                    "Help&Quit");
                    break;
                default:
                    Console.WriteLine("Komentoa ei ymmärretty");
                    break;
            }
    }

    /// <summary>
    /// Yhdistää soketin
    /// </summary>
    /// <param name="ip">osoite</param>
    /// <param name="portti">portti</param>
    /// <returns>toimivan soketin</returns>
    public static Socket Yhdistä(string ip, int portti)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        s.Connect(ip, portti);
        return s;
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

    /// <summary>
    /// hakee serverin osoitteen, porttinumeron, sekä kirjautumiseen käytettävän käyttäjänimen palvelimelta
    /// </summary>
    /// <returns>osoite, porttinumero, käyttäjänimi</returns>
    /// <exception cref="ArgumentException"></exception>
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