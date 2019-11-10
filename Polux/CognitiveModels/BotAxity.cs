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
            ComprarProductos,
            QnAMakerSigma,
            TicketReview,
            Pregunta_1,
            Pregunta_2,
            Pregunta_3,
            Pregunta_4,
            Pregunta_5,
            Pregunta_6,
            Pregunta_7,
            None
        }
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Entities Declarations
            public string[][] Modulo;
            public string[] ANIO_PRESUPUESTAL;
            public string[] Id_Ticket;
            public string[][] DESC_CUENTA;
            public string[][] DESC_ESCUELAS;
            public string[][] DESC_FUNCION;
            public string[][] DESC_INSTITUCION;
            public string[][] DESC_PRODUCTO;
            public string[][] DESC_PROYECTO;
            public string[][] DESC_REGION;
            public string[][] DESC_UBICACION;
            public string[][] TIPO_RETORNO;

            // Modulo Instance
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

            // ANIO_PRESUPUESTAL INSTANCE
            public class _InstanceLuisANIO_PRESUPUESTAL
            {
                public InstanceData ANIO_PRESUPUESTAL;
            }

            public class LuisANIO_PRESUPUESTAL
            {
                public string[] ANIO_PRESUPUESTAL;
                [JsonProperty("$instance")]
                public _InstanceLuisANIO_PRESUPUESTAL _Instance;
            }

            // DESC_CUENTA INSTANCE
            public class _InstanceLuisDESC_CUENTA
            {
                public InstanceData DESC_CUENTA;
            }

            public class LuisDESC_CUENTA
            {
                public string[] DESC_CUENTA;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_CUENTA _Instance;
            }

            // DESC_ESCUELAS INSTANCE
            public class _InstanceLuisDESC_ESCUELAS
            {
                public InstanceData DESC_ESCUELAS;
            }

            public class LuisDESC_ESCUELAS
            {
                public string[] DESC_ESCUELAS;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_ESCUELAS _Instance;
            }

            // DESC_FUNCION INSTANCE
            public class _InstanceLuisDESC_FUNCION
            {
                public InstanceData DESC_FUNCION;
            }

            public class LuisDESC_FUNCION
            {
                public string[] DESC_FUNCION;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_FUNCION _Instance;
            }

            // DESC_INSTITUCION INSTANCE
            public class _InstanceLuisDESC_INSTITUCION
            {
                public InstanceData DESC_INSTITUCION;
            }

            public class LuisDESC_INSTITUCION
            {
                public string[] DESC_INSTITUCION;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_INSTITUCION _Instance;
            }

            // DESC_PRODUCTO INSTANCE
            public class _InstanceLuisDESC_PRODUCTO
            {
                public InstanceData DESC_PRODUCTO;
            }

            public class LuisDESC_PRODUCTO
            {
                public string[] DESC_PRODUCTO;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_PRODUCTO _Instance;
            }

            // DESC_PROYECTO INSTANCE
            public class _InstanceLuisDESC_PROYECTO
            {
                public InstanceData DESC_PROYECTO;
            }

            public class LuisDESC_PROYECTO
            {
                public string[] DESC_PROYECTO;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_PROYECTO _Instance;
            }

            // DESC_REGION INSTANCE
            public class _InstanceLuisDESC_REGION
            {
                public InstanceData DESC_REGION;
            }

            public class LuisDESC_REGION
            {
                public string[] DESC_REGION;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_REGION _Instance;
            }

            // DESC_REGION INSTANCE
            public class _InstanceLuisDESC_UBICACION
            {
                public InstanceData DESC_UBICACION;
            }

            public class LuisDESC_UBICACION
            {
                public string[] DESC_UBICACION;
                [JsonProperty("$instance")]
                public _InstanceLuisDESC_UBICACION _Instance;
            }

            // TIPO_RETORNO INSTANCE
            public class _InstanceLuisTIPO_RETORNO
            {
                public InstanceData TIPO_RETORNO;
            }

            public class LuisTIPO_RETORNO
            {
                public string[] TIPO_RETORNO;
                [JsonProperty("$instance")]
                public _InstanceLuisTIPO_RETORNO _Instance;
            }


            // IDTICKET INSTANCE
            public class _InstanceLuisIdTicket
            {
                public InstanceData Id_Ticket;
            }
            public class LuisIdTicket
            {
                public string Id_Ticket;
                [JsonProperty("$instance")]
                public _InstanceLuisIdTicket _Instance;
            }

            public class _Instance
            {
                public InstanceData[] Modulo;
                public InstanceData[] Id_Ticket;
                public InstanceData[] DESC_CUENTA;
                public InstanceData[] DESC_ESCUELAS;
                public InstanceData[] DESC_FUNCION;
                public InstanceData[] DESC_INSTITUCION;
                public InstanceData[] DESC_PRODUCTO;
                public InstanceData[] DESC_PROYECTO;
                public InstanceData[] DESC_REGION;
                public InstanceData[] DESC_UBICACION;
                public InstanceData[] TIPO_RETORNO;
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
