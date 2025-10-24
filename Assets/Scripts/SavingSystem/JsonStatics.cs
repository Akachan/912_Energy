using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Utilities;

namespace SavingSystem
{
    
    public static class JsonStatics
    {

        public static JToken ToToken(this BigNumber value)
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["base"] = value.Base;
            stateDict["exponent"] = value.Exponent;
            return state;
        }

        public static BigNumber ToBigNumber(this JToken state)
        {
            BigNumber bigNumber = new BigNumber();
            if (state is JObject jObject)
            {
                IDictionary<string, JToken> stateDict = jObject;
                
                if (stateDict.TryGetValue("base", out JToken baseValue))
                {
                    bigNumber.Base = baseValue.ToObject<double>();
                }

                if (stateDict.TryGetValue("exponent", out JToken exponentValue))
                {
                    bigNumber.Exponent = exponentValue.ToObject<int>();
                }
            }
            return bigNumber;
        }
        
        public static JToken ToToken(this Vector3 vector) //Hace un minidiccionario para guardar xyz
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["x"] = vector.x;
            stateDict["y"] = vector.y;
            stateDict["z"] = vector.z;
            return state;
        }

        public static Vector3 ToVector3(this JToken state) //Retrotrae los valores  del dicc al vector
        {
            Vector3 vector = new Vector3();
            if (state is JObject jObject)
            {
                IDictionary<string, JToken> stateDict = jObject;

                if (stateDict.TryGetValue("x", out JToken x))
                {
                    vector.x = x.ToObject<float>();
                }

                if (stateDict.TryGetValue("y", out JToken y))
                {
                    vector.y = y.ToObject<float>();
                }

                if (stateDict.TryGetValue("z", out JToken z))
                {
                    vector.z = z.ToObject<float>();
                }
            }

            return vector;
        }

    }
}