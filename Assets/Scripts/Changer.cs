using UnityEngine;
using UnityEngine.SceneManagement;

public class Changer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneLoader();
    }


    public void SceneLoader()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
    /*public void Audiochanger()
    {
        AudioListener.volume = 0.2f;
    }
    public void Timer()
    {
        Debug.Log("la cuenta atras a iniciado...");
        Invoke("FinalText",3);
    }
    public void Appear(GameObject obj)
    {
        obj.SetActive(true);
        Debug.Log("El objeto misterioso aparecio");
    }
    public void Dissapear(GameObject obj)
    {
        obj.SetActive(false);
        Debug.Log("El Armario desaparecio");
    }*/
}
