using Newtonsoft.Json.Linq;

namespace WorkshopTools.Parser
{
    public class WorkshopRequestMessage
    {
        public WorkshopRequestMessage(string json)
        {
            Content = JObject.Parse(json);
        }

        public JObject Content { private set; get; }
    }
}
