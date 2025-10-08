using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

public class MinigameMemoria : MonoBehaviour
{
    public Transform gridParent;
    public GameObject cartaPrefab;
    public VideoPlayer videoPlayer;

    private List<CartaMemoria> cartas = new List<CartaMemoria>();
    private CartaMemoria primeiraCarta;
    private CartaMemoria segundaCarta;
    private int paresRestantes;
    private bool bloqueioClique = false;

    void Start()
    {
        Fase fase = GameManager.instancia.faseAtual;
        if (fase == null)
        {
            Debug.LogError("Nenhuma fase carregada!");
            return;
        }

        Debug.Log($"Iniciando fase de memória: {fase.palavraDebug}");

        string[] pares = fase.pares.Split(';');
        int idParAtual = 0;

        foreach (string p in pares)
        {
            if (string.IsNullOrWhiteSpace(p)) continue;

            string[] dados = p.Split(':');
            if (dados.Length < 3)
            {
                Debug.LogWarning($"Formato incorreto do par: {p}");
                continue;
            }

            string sinal = dados[0].Trim();
            string imagem = dados[1].Trim();
            string video = dados[2].Trim();

            CriarCarta(sinal, video, idParAtual);
            CriarCarta(imagem, video, idParAtual);

            idParAtual++;
        }

        paresRestantes = idParAtual;

        // Embaralhar cartas
        for (int i = 0; i < gridParent.childCount; i++)
        {
            int rnd = Random.Range(0, gridParent.childCount);
            gridParent.GetChild(i).SetSiblingIndex(rnd);
        }
    }

    void CriarCarta(string imagemNome, string videoNome, int idPar)
    {
        GameObject obj = Instantiate(cartaPrefab, gridParent);
        CartaMemoria carta = obj.GetComponent<CartaMemoria>();
        if (carta != null)
            carta.Configurar(imagemNome, videoNome, idPar, this);
        else
            Debug.LogError("❌ Prefab da carta não possui CartaMemoria!");
        cartas.Add(carta);
    }

    public void SelecionarCarta(CartaMemoria carta)
    {
        if (bloqueioClique || carta.revelada) return;

        carta.VirarParaCima();

        if (primeiraCarta == null)
        {
            primeiraCarta = carta;
        }
        else if (segundaCarta == null)
        {
            segundaCarta = carta;
            bloqueioClique = true;
            StartCoroutine(VerificarPar());
        }
    }

    private System.Collections.IEnumerator VerificarPar()
    {
        yield return new WaitForSeconds(0.5f);

        if (primeiraCarta.idPar == segundaCarta.idPar)
        {
            // Par correto
            VideoClip clip = Resources.Load<VideoClip>(primeiraCarta.videoNome);
            if (clip != null && videoPlayer != null)
            {
                videoPlayer.clip = clip;
                videoPlayer.Play();
            }

            primeiraCarta.Desativar();
            segundaCarta.Desativar();
            paresRestantes--;

            if (paresRestantes <= 0)
                Debug.Log("✅ Todos os pares encontrados! Fase concluída!");
        }
        else
        {
            primeiraCarta.VirarParaBaixo();
            segundaCarta.VirarParaBaixo();
        }

        primeiraCarta = null;
        segundaCarta = null;
        bloqueioClique = false;
    }
}
