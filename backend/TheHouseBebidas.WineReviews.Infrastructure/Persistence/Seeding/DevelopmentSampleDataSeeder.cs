using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheHouseBebidas.WineReviews.Domain.Entities;

namespace TheHouseBebidas.WineReviews.Infrastructure.Persistence.Seeding;

public sealed class DevelopmentSampleDataSeeder
{
    private sealed record WineSeed(
        string Code,
        string Name,
        string Winery,
        int Year,
        string GrapeVariety,
        string Description,
        string ImageUrl,
        string? SecondaryImageUrl,
        string? FeaturedReviewSummary,
        bool IsActive = true);

    private sealed record SiteContentSeed(
        string Key,
        string Title,
        string Content);

    private static readonly WineSeed[] WineSeeds =
    [
        new("malbec-heritage", "Malbec Heritage", "Bodega del Plata", 2021, "Malbec", "Malbec de altura con perfil frutado, taninos suaves y final persistente.", "https://images.unsplash.com/photo-1506377247377-2a5b3b417ebb?auto=format&fit=crop&w=1200&q=80", "https://images.unsplash.com/photo-1510626176961-4b57d4fbad03?auto=format&fit=crop&w=1200&q=80", "Complejo y redondo, ideal para carnes grilladas."),
        new("cabernet-reserva", "Cabernet Reserva", "Viña Estrella", 2020, "Cabernet Sauvignon", "Reserva con crianza en roble, notas a cassis y especias secas.", "https://images.unsplash.com/photo-1516594915697-87eb3b1c14ea?auto=format&fit=crop&w=1200&q=80", null, "Taninos firmes con excelente estructura."),
        new("tannat-legacy", "Tannat Legacy", "Casa Oriental", 2019, "Tannat", "Tannat de perfil intenso con boca amplia y final largo.", "https://images.unsplash.com/photo-1558001373-7b93ee48ffa0?auto=format&fit=crop&w=1200&q=80", "https://images.unsplash.com/photo-1519671482749-fd09be7ccebf?auto=format&fit=crop&w=1200&q=80", "Potente pero equilibrado, con muy buena integración."),
        new("pinot-noir-ocean", "Pinot Noir Ocean", "Costa Serena", 2022, "Pinot Noir", "Tinto ligero y elegante, con frutas rojas frescas y acidez vivaz.", "https://images.unsplash.com/photo-1474722883778-792e7990302f?auto=format&fit=crop&w=1200&q=80", null, "Muy fresco y fácil de tomar en reuniones."),
        new("syrah-fuego", "Syrah Fuego", "Finca Terracota", 2021, "Syrah", "Syrah especiado con notas de pimienta negra, moras y textura sedosa.", "https://images.unsplash.com/photo-1569919659476-f0852f6834b7?auto=format&fit=crop&w=1200&q=80", "https://images.unsplash.com/photo-1470337458703-46ad1756a187?auto=format&fit=crop&w=1200&q=80", "Aromático y expresivo, gran companion para platos especiados."),
        new("blend-signature", "Blend Signature", "Altos del Viento", 2018, "Blend", "Corte de Malbec, Cabernet Franc y Merlot con estilo clásico.", "https://images.unsplash.com/photo-1530023367847-a683933f4172?auto=format&fit=crop&w=1200&q=80", null, "Blend armonioso con capas de fruta y madera.")
    ];

    private static readonly SiteContentSeed[] SiteContentSeeds =
    [
        new("hero", "The House Bebidas", "Una experiencia moderna para explorar vinos, leer reseñas y elegir mejor cada botella."),
        new("about", "Nuestra propuesta", "Combinamos selección, contexto y recomendaciones para que descubras tu próxima botella."),
        new("contact", "Contacto", "Escribinos a hola@thehousebebidas.com para consultas, colaboraciones y eventos.")
    ];

    private readonly WineReviewsDbContext _dbContext;
    private readonly ILogger<DevelopmentSampleDataSeeder> _logger;

    public DevelopmentSampleDataSeeder(WineReviewsDbContext dbContext, ILogger<DevelopmentSampleDataSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await RemoveDefaultWineDataAsync(cancellationToken);
            await SeedSiteContentAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Development sample seed skipped because database schema is not ready.");
        }
    }

    private async Task RemoveDefaultWineDataAsync(CancellationToken cancellationToken)
    {
        var existingWines = await _dbContext.Wines
            .Select(wine => new { wine.Id, wine.Name, wine.Winery, wine.Year })
            .ToListAsync(cancellationToken);

        var defaultWineIds = existingWines
            .Where(wine => WineSeeds.Any(seed =>
                seed.Name.Equals(wine.Name, StringComparison.OrdinalIgnoreCase) &&
                seed.Winery.Equals(wine.Winery, StringComparison.OrdinalIgnoreCase) &&
                seed.Year == wine.Year))
            .Select(wine => wine.Id)
            .ToList();

        if (defaultWineIds.Count == 0)
        {
            return;
        }

        var reviewsToDelete = await _dbContext.Reviews
            .Where(review => defaultWineIds.Contains(review.WineId))
            .ToListAsync(cancellationToken);
        _dbContext.Reviews.RemoveRange(reviewsToDelete);

        var winesToDelete = await _dbContext.Wines
            .Where(wine => defaultWineIds.Contains(wine.Id))
            .ToListAsync(cancellationToken);
        _dbContext.Wines.RemoveRange(winesToDelete);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSiteContentAsync(CancellationToken cancellationToken)
    {
        var existingEntries = await _dbContext.SiteContents.ToListAsync(cancellationToken);
        var existingByKey = existingEntries.ToDictionary(content => content.Key, StringComparer.OrdinalIgnoreCase);

        var hasChanges = false;

        foreach (var seed in SiteContentSeeds)
        {
            if (existingByKey.TryGetValue(seed.Key, out var existing))
            {
                if (!string.Equals(existing.Title, seed.Title, StringComparison.Ordinal) ||
                    !string.Equals(existing.Content, seed.Content, StringComparison.Ordinal))
                {
                    existing.Update(seed.Title, seed.Content);
                    hasChanges = true;
                }

                continue;
            }

            var newContent = new SiteContent(
                id: Guid.Empty,
                key: seed.Key,
                title: seed.Title,
                content: seed.Content);

            await _dbContext.SiteContents.AddAsync(newContent, cancellationToken);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

}
