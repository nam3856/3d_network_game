using UnityEngine;
// 스크립트
public class Billboard : MonoBehaviour
{
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
