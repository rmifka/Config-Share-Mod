using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Config_Share.MonoBehaviours
{
    internal class ColorPreviewCubes : MonoBehaviour
    {
        [FormerlySerializedAs("_parent")] public GameObject parent;
        private GameObject _noteTemplate;
        private GameObject _cubeA;
        private GameObject _cubeB;

        private bool _isReady;
        private Color? _pendingColorA;
        private Color? _pendingColorB;

        public static ColorPreviewCubes Instance
        {
            get
            {
                if (field == null)
                {
                    var go = new GameObject("ColorPreviewCubes");
                    DontDestroyOnLoad(go);
                    field = go.AddComponent<ColorPreviewCubes>();
                }
                return field;
            }
        }

        private void Awake()
        {
            parent = new GameObject("Config_Share_PreviewCubesParent");
            parent.transform.position = new Vector3(-2.5f, 1.2f, 3f);
            parent.transform.rotation = Quaternion.Euler(0, 320f, 0);
            DontDestroyOnLoad(parent);

            const string gameCoreScene = "GameCore";
            const string standardGameplayScene = "StandardGameplay";

            SceneManager.LoadSceneAsync(gameCoreScene, LoadSceneMode.Additive)?.completed += _ =>
            {
                SceneManager.LoadSceneAsync(standardGameplayScene, LoadSceneMode.Additive)?.completed += __ =>
                {
                    var installer = Resources.FindObjectsOfTypeAll<BeatmapObjectsInstaller>().FirstOrDefault();
                    if (installer == null)
                    {
                        Plugin.Logger.Warn("Could not find BeatmapObjectsInstaller for note template.");
                        return;
                    }

                    _noteTemplate = Instantiate(installer._normalBasicNotePrefab.transform.GetChild(0).gameObject);
                    _noteTemplate.SetActive(false);
                    DontDestroyOnLoad(_noteTemplate);

                    CreatePreviewCubes();

                    SceneManager.UnloadSceneAsync(standardGameplayScene);
                    SceneManager.UnloadSceneAsync(gameCoreScene);
                };
            };
        }

        private void CreatePreviewCubes()
        {
            _cubeA = Instantiate(_noteTemplate, parent.transform, false);
            _cubeB = Instantiate(_noteTemplate, parent.transform, false);

            _cubeA.name = "Config_Share_PreviewCubeA";
            _cubeB.name = "Config_Share_PreviewCubeB";

            _cubeA.transform.localPosition = new Vector3(-0.4f, 0f, 0f);
            _cubeB.transform.localPosition = new Vector3(0.4f, 0f, 0f);

            _cubeA.transform.localRotation = Quaternion.Euler(0f, 0, 0f);
            _cubeB.transform.localRotation = Quaternion.Euler(0f, 0, 0f);

            _cubeA.SetActive(true);
            _cubeB.SetActive(true);

            _isReady = true;

            if (_pendingColorA.HasValue && _pendingColorB.HasValue)
            {
                SetColors(_pendingColorA.Value, _pendingColorB.Value);
                _pendingColorA = null;
                _pendingColorB = null;
            }
        }

        public void SetColors(Color colorA, Color colorB)
        {
            if (!_isReady)
            {
                _pendingColorA = colorA;
                _pendingColorB = colorB;
                return;
            }

            PatchNote(_cubeA, colorA);
            PatchNote(_cubeB, colorB);
        }

        private static void PatchNote(GameObject note, Color color)
        {
            if (note == null) return;

            foreach (var controller in note.GetComponents<MaterialPropertyBlockController>())
            {
                controller.materialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), color);
                controller.ApplyChanges();
            }
        }

        public void SetActive(bool active)
        {
            _cubeA?.SetActive(active);
            _cubeB?.SetActive(active);
        }
    }
}