using OrleansPOC;
using OrleansPOC.Config;

SiloHelper.Startup(new SiloConfig
{
    SiloPort = 20000,
    SiloGateway = 40000,
    Urls = [ "http://localhost:5100" ]
});