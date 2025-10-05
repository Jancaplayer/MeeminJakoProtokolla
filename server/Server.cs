using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Palvelin;

public class Palvelin
{
    public async static Task Main()
    {
        bool on = true;
        // alustus
        Asetukset asetukset = haeConfig();
        asetukset.Rinnakkaiset = 1;
        Console.WriteLine(asetukset.ToString() + "\n" + asetukset.WLToString());

        Socket soketti = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint iep = new IPEndPoint(IPAddress.Any, asetukset.Port);
        soketti.Bind(iep);
        soketti.Listen(asetukset.Rinnakkaiset + 1);

        //huonosti skaalautuva, jos isommalle porukalle haluaa palvella
        Socket[] asiakkaat = new Socket[asetukset.Rinnakkaiset + 1];
        while (on)
        {
            int vapaaIndex = -1;
            int vapaita = 0;
            for (int i = 0; i < asiakkaat.Length; i++)
            {
                Console.WriteLine(asiakkaat[i]);
                if (asiakkaat[i] == null || !asiakkaat[i].Connected) { vapaaIndex = i; vapaita++; }
            }
            if (vapaaIndex == -1)
            {
                vapaita = 0;
                while (vapaita == 0)
                {
                    //odota sokettien vapautumista
                    await Task.Delay(500);
                    for (int i = 0; i < asiakkaat.Length; i++)
                    {
                        if (!asiakkaat[i].Connected) { vapaita++; }
                    }
                }
            }
            else
            {
                //odotetaan uusi asiakas palveltavaksi
                Console.WriteLine($"accepting..");
                asiakkaat[vapaaIndex] = soketti.Accept();
                vapaita--;
                Console.WriteLine($"asiakas yhdisti, vapaita paikkoja {vapaita}");
                //aloitetaan asiakkaan palvelu
                if (vapaita > 1) { palvellaan(asiakkaat, vapaaIndex); }
                else { ilmoitaLiiastaAsiakasMäärästä(asiakkaat, vapaaIndex); }
            }
        }
    }

    /// <summary>
    /// palvellaan yhtä asiakasinstanssia
    /// </summary>
    /// <param name="soketit">lista soketeista</param>
    /// <param name="vapaaIndex">vapaan soketin indeksi</param>
    public static async void palvellaan(Socket[] soketit, int vapaaIndex)
    {
        Socket asiakas = soketit[vapaaIndex];
        await asiakas.SendAsync(Encoding.UTF8.GetBytes("GREETINGS"));

        bool palvellaan = true;
        string msg = string.Empty;
        while (palvellaan)
        {
            //asiakkaan palvelu
            //jotakin tähän tyyliin
            byte[] buffer = new byte[1024];
            int r = await asiakas.ReceiveAsync(buffer);
            msg = Encoding.UTF8.GetString(buffer, 0, r);
            Console.WriteLine($"vastaanotettiin:\n------------------------------------\n{msg}\n------------------------------------");
            string snd = "QUIT|Kaikki asiakaspaikat jo käytössä";
            await asiakas.SendAsync(Encoding.UTF8.GetBytes(snd));
        }
        await asiakas.DisconnectAsync(true);
    }

    /// <summary>
    /// kerrotaan asiakkaalle että ei enempää asiakkaita
    /// </summary>
    /// <param name="soketit">lista soketeista</param>
    /// <param name="vapaaIndex">vapaan soketin indeksi</param>
    public static async void ilmoitaLiiastaAsiakasMäärästä(Socket[] soketit, int vapaaIndex)
    {
        //jotakin tähän tyyliin
        Socket asiakas = soketit[vapaaIndex];
        byte[] buffer = new byte[1024];
        await asiakas.ReceiveAsync(buffer);
        string snd = "QUIT|Kaikki asiakaspaikat jo käytössä";
        byte[] sndBytes = Encoding.UTF8.GetBytes(snd);
        await asiakas.SendAsync(sndBytes);
        await asiakas.DisconnectAsync(true);
    }

    /// <summary>
    /// hakee porttinumeron ja pääsyluettelon .json tiedostosta
    /// </summary>
    /// <returns>porttinumero string muodossa sekä lista sallituista käyttäjänimistä</returns>
    /// <exception cref="ArgumentException"></exception>
    public static Asetukset haeConfig()
    {
        string port = string.Empty;
        string whitelist = string.Empty;

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
                        case "port":
                            port = reader.GetString();
                            break;
                        case "whitelist":
                            whitelist += reader.GetString() + " ";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        port = port == null || port == string.Empty ? "25000" : port;
        return new Asetukset(int.Parse(port), whitelist.Trim().Split(" "));
    }
}
public class Asetukset
{
    public int Port { get; set; }
    public int Rinnakkaiset { get; set; }
    public string[] Whitelist { get; }

    public Asetukset(int port, string[] whitelist, int rinnakkaiset = 1)
    {
        this.Port = port;
        this.Whitelist = whitelist;
        this.Rinnakkaiset = rinnakkaiset;
    }

    public override string ToString()
    {
        return $"port: {Port}, max parallel connections: {Rinnakkaiset}, whitelist entries: {Whitelist.Length}";
    }

    public string WLToString()
    {
        return string.Join(' ', Whitelist);
    }
}