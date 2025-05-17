using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

//Añadir esta clase a un objeto en escena
//Lee el texto de los elementos de la UI que lo tengan
public class TTSHUD : MonoBehaviour
{
    TextMeshPro text;

    [SerializeField]
    int mouseButton = 0;

    [SerializeField]
    LayerMask mask;

    [SerializeField]
    float cooldownTime = 2f;
    float elapsedTime = 2f;

    private void Awake()
    {
       mask = LayerMask.GetMask("UI");
    }

    public void ReadHUD()
    {
        TTSManager.m_Instance.StartSpeech(text.text);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            Debug.Log("mouse pressed");
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector3.forward, Mathf.Infinity, mask);
            
            //Collider2D hit = Physics2D.OverlapPoint(Input.mousePosition, mask); //--> sigue dando hit = Null
            Debug.Log(hit.collider);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<TextMeshProUGUI>() != null) {

                text.text = hit.collider.GetComponent<TextMeshProUGUI>().text;
                if(text.text != "" || text.text != null) ReadHUD();
                Debug.Log(text.text);
            }

        }
    }
}
