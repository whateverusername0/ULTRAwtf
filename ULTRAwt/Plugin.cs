using BepInEx; using HarmonyLib; using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ULTRAwt
{
    public static class Extensions
    {
        public static bool HasComponent<T>(this GameObject GM) where T : Component
        { return GM.GetComponent<T>() != null; }
    }

    [BepInPlugin(GUID, GUID, Version)] public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "ULTRAwt", Version = "6.1";

        public void Awake()
        {
            new Harmony(GUID).PatchAll();
            SceneManager.activeSceneChanged += (x, y) => SceneChanged(y);
        }
        public void SceneChanged(Scene S)
        {
            foreach (Canvas C in Object.FindObjectsOfType<Canvas>())
                if (!C.gameObject.HasComponent<TransformJigger>())
                    RecursiveConvertCanvas();

            foreach (BoxCollider C in Object.FindObjectsOfType<BoxCollider>())
                if (!C.gameObject.HasComponent<TransformJigger>() && C.gameObject.name != "OutOfBounds")
                    C.gameObject.AddComponent<TransformJigger>();
        }


        private static void RecursiveConvertCanvas(GameObject GO = null)
        {
            if (GO != null)
            {
                try { ConvertCanvas(GO.GetComponent<Canvas>()); } catch { }

                if (GO.transform.childCount > 0)
                    for (int i = 0; i < GO.transform.childCount; i++)
                        RecursiveConvertCanvas(GO.transform.GetChild(i).gameObject);
            }
            else
            {
                foreach (Canvas C in Object.FindObjectsOfType<Canvas>())
                    if (!C.gameObject.HasComponent<TransformJigger>())
                        try { ConvertCanvas(C); } catch {}
            }
        }
        private static void ConvertCanvas(Canvas C)
        {
            C.gameObject.AddComponent<TransformJigger>();
            foreach (Transform Child in C.transform) ConvertElement(Child);
        }
        private static void ConvertElement(Transform Element)
        {
            Element.gameObject.AddComponent<TransformJigger>();
            foreach (Transform Child in Element) ConvertElement(Child);
        }
    }

    public class TransformJigger : MonoBehaviour
    {
        static float T = 0;
        public void Update()
        {
            transform.localRotation *= Quaternion.Euler(Mathf.Lerp(Random.Range(-2.5f, 1), Random.Range(-2.5f, 1), T),
                                                        Mathf.Lerp(Random.Range(-2.5f, 1), Random.Range(-2.5f, 1), T),
                                                        Mathf.Lerp(Random.Range(-2.5f, 1), Random.Range(-2.5f, 1), T));
            //transform.localScale += new Vector3(Mathf.Lerp(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), T),
            //                                    Mathf.Lerp(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), T),
            //                                    Mathf.Lerp(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), T));
            T += 0.1f * Time.deltaTime;
        }
    }
}
