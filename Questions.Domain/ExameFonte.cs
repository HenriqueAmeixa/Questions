namespace Questions.Domain;

public class ExameFonte
{
    public TipoExame Tipo { get; set; } = TipoExame.Desconhecido;
    public string Nome { get; set; } = string.Empty;
    public int? Ano { get; set; }
    public string? CodigoProva { get; set; }
    public string? AreaOficial { get; set; }
}
