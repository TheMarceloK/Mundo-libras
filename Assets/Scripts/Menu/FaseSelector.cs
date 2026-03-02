using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FaseSelector : MonoBehaviour
{
    [Header("Lista de Fases")]
    [Tooltip("Adicione os nomes das cenas aqui (ex: Jogo_QuebraCabeca, Jogo_Memoria)")]
    [SerializeField] private List<string> fases = new List<string>();

    // Função para ser chamada no botão da UI
    public void CarregarFasePorIndice(int index)
    {
        if (index >= 0 && index < fases.Count)
        {
            SceneManager.LoadScene(fases[index]);
        }
        else
        {
            Debug.LogError($"Erro: Tentou carregar o índice {index}, mas a lista só vai de 0 a {fases.Count - 1}.");
        }
    }
}