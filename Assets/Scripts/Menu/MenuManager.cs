using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Configuração de Cena")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject panelFases;
    [SerializeField] private GameObject panelCreditos;

    public void btnJogar()
    {
        panelMenu.SetActive(false);
        panelFases.SetActive(true);
    }

    public void btnVoltarMissoes()
    {
        panelMenu.SetActive(true);
        panelFases.SetActive(false);
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
