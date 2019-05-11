using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;



public class serializedObjectTest : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Test1")]
    void Test1()
    {
        transform.localPosition = Vector3.zero;
        Debug.Log("Local Position Original Value:" + transform.localPosition.ToString());

        SerializedObject so = new SerializedObject(transform);
        SerializedProperty sp = so.FindProperty("m_LocalPosition");
        Debug.Log("Readed Value In SerializedProperty:" + sp.vector3Value.ToString());

        transform.localPosition = Vector3.one;
        Debug.Log("Local Position Changed Value:" + transform.localPosition.ToString());

        Debug.Log("Readed Value In SerializedProperty:" + sp.vector3Value.ToString());

        so.Update();
        Debug.Log("Readed Value In SerializedProperty After Update:" + sp.vector3Value.ToString());


        sp.vector3Value = Vector3.one * 2f;
        so.ApplyModifiedProperties();
        Debug.Log("Readed Value In Transform After Modified:" + transform.localPosition.ToString());
    }
#endif
}
