using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Newtonsoft.Json;

namespace CoreBot.CognitiveModels
{
    public partial class BotAxity : IRecognizerConvert
    {
        public string Text;
        public string AlteredText;

        public enum Intent
        {
            PasswordReset,
            None
        }
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            public string[][] Modulo;

            public class _InstanceLuisModulo
            {
                public InstanceData Modulo;
            }

            public class LuisModulo
            {
                public string[] Modulo;
                [JsonProperty("$instance")]
                public _InstanceLuisModulo _Instance;
            }

            public class _Instance
            {
                public InstanceData[] Modulo;
            }
            [JsonProperty("$instance")]
            public _Instance _instance;
        }
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData =true)]
        public Dictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<BotAxity>(JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties; 
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = 0.0;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}
