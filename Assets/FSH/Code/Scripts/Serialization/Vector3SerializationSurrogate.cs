using System.Runtime.Serialization;
using UnityEngine;

namespace Fishy.Serialization {
    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {

        // Method called to serialize a Vector3 object
        public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
        {

            Vector3 vector3 = (Vector3)obj;
            info.AddValue("x", vector3.x);
            info.AddValue("y", vector3.y);
            info.AddValue("z", vector3.z);
        }

        // Method called to deserialize a Vector3 object
        public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 vector3 = (Vector3)obj;
            vector3.x = (float)info.GetValue("x", typeof(float));
            vector3.y = (float)info.GetValue("y", typeof(float));
            vector3.z = (float)info.GetValue("z", typeof(float));
            obj = vector3;
            return obj;   // Formatters ignore this return value //Seems to have been fixed!
        }
    }
}
