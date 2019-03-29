using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Fishy.Serialization
{
	public static class Serialzation {

        public static BinaryFormatter GenerateBinaryFormatter()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            // 1. Construct a SurrogateSelector object
            SurrogateSelector surrogateSelector = new SurrogateSelector();

            Vector3SerializationSurrogate vector3SerializationSurrogate = new Vector3SerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3SerializationSurrogate);

            // 2. Have the formatter use our surrogate selector
            binaryFormatter.SurrogateSelector = surrogateSelector;

            return binaryFormatter;
        }
    }
}
