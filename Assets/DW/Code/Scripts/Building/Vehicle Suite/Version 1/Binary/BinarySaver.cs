using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unifish.Serialization;

namespace DW.Building.DepreciatedShipSuite
{
    public class BinarySaver : MonoBehaviour
    {
        public string folderName = "BinaryData";
        public string testname = "Unifish";
        public float size = 1.5f;
        public int verts = 4;
        public Vector3 position = Vector3.zero;

        private TestData data;
        private const string fileExtension = ".dat";
        private string folderPath;

        private void Awake()
        {
            folderPath = Path.Combine(Application.persistentDataPath, folderName);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K)) {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                data = new TestData(testname, size, verts, position);
                data.DebugInfo();

                string dataPath = Path.Combine(folderPath, data.name + fileExtension);
                SaveCharacter(data, dataPath);
            }

            if (Input.GetKeyDown(KeyCode.L)) {
                string[] filePaths = Directory.GetFiles(folderPath);

                foreach (var item in filePaths) {
                    TestData result = LoadCharacter(item);
                    result.DebugInfo();
                }
                    
                
            }
        }

        static void SaveCharacter(TestData data, string path)
        {
            BinaryFormatter binaryFormatter = GenerateBinaryFormatter();

            using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate)) {
                binaryFormatter.Serialize(fileStream, data);
            }
        }

        static TestData LoadCharacter(string path)
        {
            BinaryFormatter binaryFormatter = GenerateBinaryFormatter();

            using (FileStream fileStream = File.Open(path, FileMode.Open)) {
                return (TestData)binaryFormatter.Deserialize(fileStream);
            }
        }

        static BinaryFormatter GenerateBinaryFormatter()
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