namespace OrleansPOC.Config;

public class SiloConfig
{
    public int SiloPort { get; set; } = 11111;
    public int PrimarySiloPort { get; set; } = 11111;
    public int SiloGateway { get; set; } = 30000;
    public string[] Urls { get; set; } = { "http://localhost:5000" };
}