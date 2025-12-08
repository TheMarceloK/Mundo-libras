using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

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
            CarregarFasesCSV();
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
            Debug.LogError("❌ Arquivo 'fases.csv' não encontrado na pasta Resources!");
            return;
        }

        string texto = csvFile.text.Replace("\r", "");
        string[] linhas = texto.Split('\n');

        if (linhas.Length <= 1)
        {
            Debug.LogError("❌ O arquivo CSV parece estar vazio!");
            return;
        }

        // Detectar separador observando o cabeçalho (mais confiável)
        string headerLine = linhas[0];
        char separador = headerLine.Contains(';') ? ';' : (headerLine.Contains(',') ? ',' : ';');

        // Cabeçalho -> cria mapa de colunas (normalizado lowercase)
        string[] cabecalho = headerLine.Split(separador);
        var indices = new Dictionary<string, int>();
        for (int i = 0; i < cabecalho.Length; i++)
        {
            string col = cabecalho[i].Trim().ToLower();
            if (!indices.ContainsKey(col))
                indices[col] = i;
        }

        fases.Clear();
        int contador = 0;

        for (int i = 1; i < linhas.Length; i++)
        {
            string linha = linhas[i];
            if (string.IsNullOrWhiteSpace(linha)) continue;

            // IMPORTANTE: limitar o split para 6 partes (id,tipo,palavraDebug,imagem,video,pares)
            string[] colunas = linha.Split(new char[] { separador }, 6);

            // Se a linha tiver menos colunas do que o header, preenche com strings vazias
            if (colunas.Length < cabecalho.Length)
            {
                Array.Resize(ref colunas, cabecalho.Length);
                for (int k = 0; k < colunas.Length; k++)
                    colunas[k] = colunas[k] ?? "";
            }

            try
            {
                Fase fase = new Fase();
                fase.id = LerInt(colunas, indices, "id");
                fase.tipo = LerTexto(colunas, indices, "tipo");
                fase.palavraDebug = LerTexto(colunas, indices, "palavradebug"); // note lowercase
                fase.imagem = LerTexto(colunas, indices, "imagem");
                fase.video = LerTexto(colunas, indices, "video");
                fase.pares = LerTexto(colunas, indices, "pares"); // aqui pode conter ';' internamente

                fases.Add(fase);

                // log para depuração
                string previewPares = fase.pares != null && fase.pares.Length > 60 ? fase.pares.Substring(0, 60) + "..." : fase.pares;
                Debug.Log($"Linha {i + 1} lida -> id:{fase.id} tipo:{fase.tipo} palavraDebug:{fase.palavraDebug} paresPreview:{previewPares}");

                contador++;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Erro ao ler linha {i + 1}: {e.Message}");
            }
        }

        Debug.Log($"✅ {contador} fases carregadas com sucesso do CSV!");
    }

    private string LerTexto(string[] colunas, Dictionary<string, int> indices, string nome)
    {
        nome = nome.ToLower();
        if (!indices.ContainsKey(nome)) return null;

        int idx = indices[nome];
        if (idx < 0 || idx >= colunas.Length) return null;

        string valor = colunas[idx]?.Trim().Trim('"');
        return string.IsNullOrEmpty(valor) ? null : valor;
    }

    private int LerInt(string[] colunas, Dictionary<string, int> indices, string nome)
    {
        string valor = LerTexto(colunas, indices, nome);
        if (int.TryParse(valor, out int resultado))
            return resultado;
        return 0;
    }

    public void SelecionarFase(int id)
    {
        faseAtual = fases.Find(f => f.id == id);

        if (faseAtual == null)
        {
            Debug.LogError($"❌ Fase com ID {id} não encontrada!");
            return;
        }

        if (string.IsNullOrEmpty(faseAtual.tipo))
        {
            Debug.LogError($"⚠️ Fase {id} tem tipo nulo ou vazio!");
            return;
        }

        Debug.Log($"▶️ Selecionando fase {faseAtual.id}: tipo {faseAtual.tipo}, palavraDebug = {faseAtual.palavraDebug}");

        switch (faseAtual.tipo.ToLower())
        {
            case "memoria":
            case "memória": // só por segurança com acento
                MudarCena("Jogo_Memoria");
                break;

            case "forca":
            case "força":
                MudarCena("JogoDaForca");
                break;

            case "quebra cabeca":
            case "quebra-cabeca":
                MudarCena("JogoQuebraCabeca");
                break;

            default:
                Debug.LogError($"⚠️ Tipo de fase desconhecido: {faseAtual.tipo}");
                break;
        }
    }

    public void MudarCena(string CenaString)
    {
        SceneManager.LoadScene(CenaString);
    }
}
