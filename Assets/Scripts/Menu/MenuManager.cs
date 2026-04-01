using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject SceneSelecao;
    [SerializeField] private GameObject panelCreditos;

    public void btnVoltarMissoes()
    {
        SceneManager.LoadScene("Menu");
    }

    public void btnCreditos()
    {
        panelMenu.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void btnVoltarCreditos()
    {
        panelMenu.SetActive(true);
        panelCreditos.SetActive(false);
    }

    public void btnSair()
    {
        Application.Quit();
        Debug.Log("Jogo fechou");
    }
}
