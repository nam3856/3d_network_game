using UnityEngine;
// 스크립트
public class cam : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Deleteeeeeeeeeeeeeeeee"))
        {
            Destroy(gameObject);
        }
    }
}
