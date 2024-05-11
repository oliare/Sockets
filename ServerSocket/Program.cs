using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Xml.Serialization;

internal class Program
{
    public static string file = "streets.xml";
    public static List<Street> streets;
    static void Main(string[] args)
    {
        streets = new List<Street>();
        ConnectServer();
    }
    static void ConnectServer()
    {
        string address = "127.0.0.1";
        int port = 8080;

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
        EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        Socket listener = null;

        try
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listener.Bind(ipPoint);
            Console.WriteLine("Connected");

            List<Street> streets = GetStreetsFromXml(file);

            while (true)
            {
                int bytes = 0;
                byte[] buffer = new byte[1024];
                bytes = listener.ReceiveFrom(buffer, ref remote);

                string msg = Encoding.Unicode.GetString(buffer);

                if (!string.IsNullOrEmpty(msg))
                {

                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} : {msg} from {remote}");

                    int postalCode;
                    if (int.TryParse(msg, out postalCode))
                    {
                        string response = GetStreetByCode(streets, postalCode);
                        if (response != null)
                        {
                            buffer = Encoding.Unicode.GetBytes(response);
                            listener.SendTo(buffer, remote);
                        }
                        else Console.WriteLine("Postal code not found");
                    }
                    else Console.WriteLine("Invalid postal code format");
                }
                else Console.WriteLine("Received message is null or empty");

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
        }
    }

    static List<Street> GetStreetsFromXml(string file)
    {
        try
        {
            XmlSerializer xsL = new XmlSerializer(typeof(List<Street>));

            using (TextReader tr = new StreamReader(file))
            {
                return xsL.Deserialize(tr) as List<Street>;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading streets from XML: {ex.Message}");
        }
        return new List<Street>();
    }


    static string? GetStreetByCode(List<Street> streets, int code)
    {
        foreach (var s in streets)
            if (s.PostIndex == code) return s.Name;
        return null;
    }
}
public class Street
{
    public string? Name { get; set; }
    public int PostIndex { get; set; }
}
