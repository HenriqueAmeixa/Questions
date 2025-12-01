using Questions.Domain;
using Questions.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<QuestoesRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.MapGet("/", () => Results.Ok(new
{
    mensagem = "API de Questões está no ar.",
    versao = "1.0"
}));

// GET /questoes  (lista todas ou filtra por query)
app.MapGet("/questoes", (
    QuestoesRepository repo,
    string? area,
    string? assunto,
    string? curso,
    string? tag) =>
{
    // Se não tiver filtro, devolve tudo
    if (string.IsNullOrWhiteSpace(area) &&
        string.IsNullOrWhiteSpace(assunto) &&
        string.IsNullOrWhiteSpace(curso) &&
        string.IsNullOrWhiteSpace(tag))
    {
        var todas = repo.GetAll();
        return Results.Ok(todas);
    }

    // Se tiver algum filtro, usa o método Filter
    var filtradas = repo.Filter(area, assunto, curso, tag);
    return Results.Ok(filtradas);
});

// GET /questoes/{id}
app.MapGet("/questoes/{id:guid}", (QuestoesRepository repo, Guid id) =>
{
    var questao = repo.GetById(id);
    return questao is null ? Results.NotFound() : Results.Ok(questao);
});

// POST /questoes
app.MapPost("/questoes", (QuestoesRepository repo, Questao nova) =>
{
    // Aqui você pode colocar validações simples, se quiser
    if (string.IsNullOrWhiteSpace(nova.Enunciado))
        return Results.BadRequest("Enunciado é obrigatório.");

    var criada = repo.Add(nova);
    return Results.Created($"/questoes/{criada.Id}", criada);
});

// PUT /questoes/{id}
app.MapPut("/questoes/{id:guid}", (QuestoesRepository repo, Guid id, Questao atualizada) =>
{
    var ok = repo.Update(id, atualizada);
    return ok ? Results.NoContent() : Results.NotFound();
});

// DELETE /questoes/{id}
app.MapDelete("/questoes/{id:guid}", (QuestoesRepository repo, Guid id) =>
{
    var ok = repo.Delete(id);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/questoes/random", (
    QuestoesRepository repo,
    string? area,
    string? assunto,
    string? curso,
    string? tag) =>
{
    var lista = repo.Filter(area, assunto, curso, tag).ToList();

    if (lista.Count == 0)
        return Results.NotFound("Nenhuma questão encontrada para os filtros informados.");

    var random = new Random();
    var index = random.Next(lista.Count);
    var sorteada = lista[index];

    return Results.Ok(sorteada);
});

// GET /questoes/random/area/{area}
app.MapGet("/questoes/random/area/{area}", (
    QuestoesRepository repo,
    string area) =>
{
    if (string.IsNullOrWhiteSpace(area))
        return Results.BadRequest("A área é obrigatória.");

    // Filtra apenas pela área
    var lista = repo.Filter(area: area, assunto: null, curso: null, tag: null).ToList();

    if (lista.Count == 0)
        return Results.NotFound($"Nenhuma questão encontrada na área '{area}'.");

    var random = new Random();
    var index = random.Next(lista.Count);
    var sorteada = lista[index];

    return Results.Ok(sorteada);
});

// GET /questoes/area/{area} - retorna todas as questões de uma área específica
app.MapGet("/questoes/area/{area}", (QuestoesRepository repo, string area) =>
{
    if (string.IsNullOrWhiteSpace(area))
        return Results.BadRequest("A área é obrigatória.");

    var questoes = repo
        .Filter(area: area, assunto: null, curso: null, tag: null)
        .ToList();

    if (questoes.Count == 0)
        return Results.NotFound($"Nenhuma questão encontrada na área '{area}'.");

    return Results.Ok(questoes);
});


// GET /areas - retorna todas as áreas cadastradas (sem duplicados)
app.MapGet("/areas", (QuestoesRepository repo) =>
{
    var areas = repo
        .GetAll()
        .Select(q => q.Area)
        .Where(a => !string.IsNullOrWhiteSpace(a))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(a => a)
        .ToList();

    return Results.Ok(areas);
});


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
