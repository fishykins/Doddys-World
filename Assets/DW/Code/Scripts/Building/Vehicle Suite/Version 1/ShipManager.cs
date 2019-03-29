using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Fishy.Serialization;

namespace DW.Building.DepreciatedShipSuite
{
    [RequireComponent(typeof(BuildManager)), RequireComponent(typeof(InputManager))]
	public class ShipManager : MonoBehaviour {
        #region Variables
        //Public & Serialized
        [Header("Saving/Loading")]
        [SerializeField]
        private string path = "Ship Manager";
        public string fileName = "default";
        [Header("Building")]
        public EditMode editMode = EditMode.build;
        
        //Private
        private InputManager inputManager;
        private BuildManager buildManager;

        private string folderPath;
        private const string fileExtension = ".dat";
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void Awake () {
            inputManager = GetComponent<InputManager>();
            buildManager = GetComponent<BuildManager>();
            folderPath = Path.Combine(Application.persistentDataPath, path);
        }
        #endregion;

        #region Custom Methods
        public void CycleEditMode()
        {
            editMode++;
            if ((int)editMode > 2) {
                editMode = 0;
            }
        }

        public void SaveBuild(string fileName)
        {
            ValidateSaveFolder();
            string dataPath = Path.Combine(folderPath, fileName + fileExtension);
            BinaryFormatter binaryFormatter = GenerateBinaryFormatter();

            using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate)) {
                binaryFormatter.Serialize(fileStream, buildManager.Grid);
            }
        }

        public void LoadBuild(string fileName)
        {

        }

        private void ValidateSaveFolder()
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        private BinaryFormatter GenerateBinaryFormatter()
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
        #endregion
	}
}
