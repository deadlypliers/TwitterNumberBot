using Newtonsoft.Json;
using TwitterBot.Api.Domain.Model.Meta;

namespace TwitterBot.Api.Domain.Base
{
    public abstract class TwitterEntityBase
    {
        public BaseTwitterMeta Meta { get; set; }

        public virtual string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerConfiguration.TwitterEntitySerializerSettings);
        }
    }
}
