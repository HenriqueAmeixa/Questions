namespace Questions.Domain;

public class Questao
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Enunciado { get; set; } = string.Empty;
    public List<Alternativa> Alternativas { get; set; } = new();
    public string? ComentarioResolucao { get; set; }
    public ExameFonte Fonte { get; set; } = new();
    public string Area { get; set; } = string.Empty;
    public List<string> Assuntos { get; set; } = new();
    public List<string> CursosRelacionados { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public NivelDificuldade Dificuldade { get; set; } = NivelDificuldade.Medio;
    public string? UrlFonteOficial { get; set; }
}
