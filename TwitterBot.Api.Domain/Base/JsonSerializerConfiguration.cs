using Newtonsoft.Json;

namespace TwitterBot.Api.Domain.Base
{
    public class JsonSerializerConfiguration
    {
        public static JsonSerializerSettings TwitterEntitySerializerSettings = new()
        { 
            NullValueHandling = NullValueHandling.Ignore,
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //DateFormatString = "yyyy-MM-dd",
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }
}
