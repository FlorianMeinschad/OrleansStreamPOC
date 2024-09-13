// See https://aka.ms/new-console-template for more information

using OrleansPOC;
using OrleansPOC.Config;

SiloHelper.Startup(new SiloConfig
{
    SiloPort = 11111,
    SiloGateway = 30000,
    Urls = [ "http://localhost:5000" ]
});