using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParalelDownloader.src.Core
{
    public sealed class HttpClientSingleton
    {
        // Jedna statická instance pro celou aplikaci (vytvořena při startu programu)
        private static readonly HttpClient _instance = new HttpClient();

        // Veřejná vlastnost pro přístup
        public static HttpClient Instance => _instance;

        // Soukromý konstruktor zabrání vytvoření instancí
        private HttpClientSingleton() { }
    }
}
