using UnityEngine;
using UnityEngine.SceneManagement;

public class FaseSelector : MonoBehaviour
{
    [SerializeField] private string fase1 = "Jogo_QuebraCabeca";
    [SerializeField] private string fase2 = "Jogo_Memoria";

    public void Fase1()
    {
        SceneManager.LoadScene(fase1);
    }

    public void Fase2()
    {
        SceneManager.LoadScene(fase2);
    }
}
