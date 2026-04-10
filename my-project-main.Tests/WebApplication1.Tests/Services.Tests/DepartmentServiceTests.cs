using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.Implementations;
using Xunit.Abstractions;

namespace my_project_main.Tests.WebApplication1.Tests.Services.Tests;

public class DepartmentServiceTests
{
    private readonly ITestOutputHelper _output;

    public DepartmentServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private DepartmentServiceImp CreateSut()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        return new DepartmentServiceImp(context);
    }

    [Fact(DisplayName = "[Département — Service] Ajouter un département")]
    public void CreateDepartment_PersistsAndReturnsDepartment()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "RH" });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("RH", created.Name);
        _output.WriteLine($"→ Département créé : Id={created.Id}, Nom={created.Name}");
    }

    [Fact(DisplayName = "[Département — Service] Ajouter des départements puis lister (scénario complet)")]
    public void AddDepartments_Then_ListAll_ShowsInTerminal()
    {
        var service = CreateSut();

        _output.WriteLine("=== Étape 1 : ajout de départements ===");
        var rh = service.CreateDepartment(new DepartmentCreate { Name = "RH" });
        _output.WriteLine($"  ✓ Ajouté — Id={rh.Id}, Nom={rh.Name}");
        var it = service.CreateDepartment(new DepartmentCreate { Name = "IT" });
        _output.WriteLine($"  ✓ Ajouté — Id={it.Id}, Nom={it.Name}");
        var finance = service.CreateDepartment(new DepartmentCreate { Name = "Finance" });
        _output.WriteLine($"  ✓ Ajouté — Id={finance.Id}, Nom={finance.Name}");

        _output.WriteLine("=== Étape 2 : liste de tous les départements (après ajout) ===");
        var all = service.GetAllDepartments();
        Assert.Equal(3, all.Count);
        foreach (var d in all.OrderBy(x => x.Name))
            _output.WriteLine($"  • Id={d.Id} | Nom={d.Name}");
        _output.WriteLine($"=== Total : {all.Count} département(s) en base (InMemory) ===");
    }

    [Fact(DisplayName = "[Département — Service] Récupérer un département par id (existant)")]
    public void FindDepartmentById_Existing_ReturnsDepartment()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "IT" });

        var found = service.FindDepartmentById(created.Id);

        Assert.NotNull(found);
        Assert.Equal("IT", found!.Name);
        _output.WriteLine($"→ Département lu : Id={found.Id}, Nom={found.Name}");
    }

    [Fact(DisplayName = "[Département — Service] Récupérer un département par id (inexistant → null)")]
    public void FindDepartmentById_Missing_ReturnsNull()
    {
        var service = CreateSut();
        var missingId = Guid.NewGuid();

        var found = service.FindDepartmentById(missingId);

        Assert.Null(found);
        _output.WriteLine($"→ Aucun département pour Id={missingId} (attendu).");
    }

    [Fact(DisplayName = "[Département — Service] Lister tous les départements")]
    public void GetAllDepartments_ReturnsCreatedDepartments()
    {
        var service = CreateSut();
        service.CreateDepartment(new DepartmentCreate { Name = "A" });
        service.CreateDepartment(new DepartmentCreate { Name = "B" });

        var all = service.GetAllDepartments();

        Assert.Equal(2, all.Count);
        foreach (var d in all)
            _output.WriteLine($"→ Liste : Id={d.Id}, Nom={d.Name}");
    }

    [Fact(DisplayName = "[Département — Service] Modifier un département")]
    public void UpadateDepartment_Existing_UpdatesName()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "Ancien" });

        var updated = service.UpadateDepartment(created.Id, new DepartmentUpdate { Name = "Nouveau" });

        Assert.Equal("Nouveau", updated.Name);
        _output.WriteLine($"→ Département mis à jour : Id={updated.Id}, Nom={updated.Name}");
    }

    [Fact(DisplayName = "[Département — Service] Modifier un département inexistant → exception")]
    public void UpadateDepartment_Missing_Throws()
    {
        var service = CreateSut();
        var missingId = Guid.NewGuid();

        var ex = Assert.Throws<Exception>(() =>
            service.UpadateDepartment(missingId, new DepartmentUpdate { Name = "X" }));

        Assert.Contains(missingId.ToString(), ex.Message);
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Département — Service] Supprimer un département")]
    public void DeleteDepartment_Existing_RemovesFromDatabase()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "Temp" });

        service.DeleteDepartment(created.Id);

        Assert.Null(service.FindDepartmentById(created.Id));
        _output.WriteLine($"→ Département supprimé (Id={created.Id}), plus présent en base.");
    }

    [Fact(DisplayName = "[Département — Service] Supprimer un département inexistant → exception")]
    public void DeleteDepartment_Missing_Throws()
    {
        var service = CreateSut();
        var missingId = Guid.NewGuid();

        var ex = Assert.Throws<Exception>(() => service.DeleteDepartment(missingId));

        Assert.Contains(missingId.ToString(), ex.Message);
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }
}
