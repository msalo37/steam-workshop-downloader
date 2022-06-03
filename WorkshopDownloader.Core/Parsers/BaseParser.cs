using System.Net.Http;
using System.Threading.Tasks;

namespace WorkshopDownloader.Core.Parsers
{
    public abstract class BaseParser
    {
        public BaseParser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public BaseParser()
        {
            httpClient = new HttpClient();
        }

        protected HttpClient httpClient;

        public abstract Task<string[]> RequestInfo(ulong itemId);
    }
}
