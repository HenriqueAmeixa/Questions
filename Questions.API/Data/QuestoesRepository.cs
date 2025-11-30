using System.Text.Json;
using Questions.Domain;

namespace Questions.Api.Data;

public class QuestoesRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions;

    public QuestoesRepository(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDir);

        _filePath = Path.Combine(dataDir, "questoes.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        // Se o arquivo não existir, cria com lista vazia
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    private List<Questao> LoadAll()
    {
        var json = File.ReadAllText(_filePath);
        var lista = JsonSerializer.Deserialize<List<Questao>>(json, _jsonOptions);
        return lista ?? new List<Questao>();
    }

    private void SaveAll(List<Questao> questoes)
    {
        var json = JsonSerializer.Serialize(questoes, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<Questao> GetAll()
    {
        return LoadAll();
    }

    public Questao? GetById(Guid id)
    {
        return LoadAll().FirstOrDefault(q => q.Id == id);
    }

    public Questao Add(Questao questao)
    {
        var lista = LoadAll();

        // Garante um Id válido
        if (questao.Id == Guid.Empty)
        {
            // precisa de set; no Domain pra funcionar
            questao = new Questao
            {
                Id = Guid.NewGuid(),
                Enunciado = questao.Enunciado,
                Alternativas = questao.Alternativas,
                ComentarioResolucao = questao.ComentarioResolucao,
                Fonte = questao.Fonte,
                Area = questao.Area,
                Assuntos = questao.Assuntos,
                CursosRelacionados = questao.CursosRelacionados,
                Tags = questao.Tags,
                Dificuldade = questao.Dificuldade,
                UrlFonteOficial = questao.UrlFonteOficial
            };
        }

        lista.Add(questao);
        SaveAll(lista);

        return questao;
    }

    public bool Update(Guid id, Questao atualizada)
    {
        var lista = LoadAll();
        var existente = lista.FirstOrDefault(q => q.Id == id);

        if (existente is null)
            return false;

        // Atualiza campos (mantendo o Id)
        existente.Enunciado = atualizada.Enunciado;
        existente.Alternativas = atualizada.Alternativas;
        existente.ComentarioResolucao = atualizada.ComentarioResolucao;
        existente.Fonte = atualizada.Fonte;
        existente.Area = atualizada.Area;
        existente.Assuntos = atualizada.Assuntos;
        existente.CursosRelacionados = atualizada.CursosRelacionados;
        existente.Tags = atualizada.Tags;
        existente.Dificuldade = atualizada.Dificuldade;
        existente.UrlFonteOficial = atualizada.UrlFonteOficial;

        SaveAll(lista);
        return true;
    }

    public bool Delete(Guid id)
    {
        var lista = LoadAll();
        var removed = lista.RemoveAll(q => q.Id == id);

        if (removed == 0)
            return false;

        SaveAll(lista);
        return true;
    }

    public IEnumerable<Questao> Filter(
        string? area = null,
        string? assunto = null,
        string? curso = null,
        string? tag = null)
    {
        var lista = LoadAll().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(area))
        {
            lista = lista.Where(q =>
                q.Area.Contains(area, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(assunto))
        {
            lista = lista.Where(q =>
                q.Assuntos.Any(a =>
                    a.Contains(assunto, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(curso))
        {
            lista = lista.Where(q =>
                q.CursosRelacionados.Any(c =>
                    c.Contains(curso, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            lista = lista.Where(q =>
                q.Tags.Any(t =>
                    t.Contains(tag, StringComparison.OrdinalIgnoreCase)));
        }

        return lista;
    }
}
