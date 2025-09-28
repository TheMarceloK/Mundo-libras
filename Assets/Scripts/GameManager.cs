using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    public List<Fase> fases = new List<Fase>();
    public Fase faseAtual;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            CarregarFasesCSV(); // só carrega 1x
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CarregarFasesCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("fases");
        if (csvFile == null)
        {
            Debug.LogError("Arquivo fases.csv não encontrado em Resources!");
            return;
        }

        string[] linhas = csvFile.text.Split('\n');

        for (int i = 1; i < linhas.Length; i++) // começa da linha 1 (cabeçalho é linha 0)
        {
            if (string.IsNullOrWhiteSpace(linhas[i])) continue;

            string[] colunas = linhas[i].Split(',');

            Fase fase = new Fase();
            fase.id = int.Parse(colunas[0].Trim());
            fase.tipo = colunas[1].Trim();
            fase.palavraDebug = colunas[2].Trim();
            fase.imagem = colunas[3].Trim();
            fase.video = colunas[4].Trim();

            fases.Add(fase);
        }

        Debug.Log($"Foram carregadas {fases.Count} fases do CSV.");
    }

    public void SelecionarFase(int id)
    {
        faseAtual = fases.Find(f => f.id == id);

        if (faseAtual == null)
        {
            Debug.LogError($"Fase com ID {id} não encontrada!");
            return;
        }

        Debug.Log($"Selecionando fase {faseAtual.id}: {faseAtual.tipo}");

        switch (faseAtual.tipo)
        {
            case "Memoria":
                SceneManager.LoadScene("JogoDaMemoria");
                break;
            case "Forca":
                SceneManager.LoadScene("JogoDaForca");
                break;
            default:
                Debug.LogError($"Tipo de fase desconhecido: {faseAtual.tipo}");
                break;
        }
    }
}
