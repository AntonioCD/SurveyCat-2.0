using SurveyCat.Shared.Entities;

namespace SurveyCat.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckDepartamentosAsync();
    }

    private async Task CheckDepartamentosAsync()
    {
        if (!_context.Departamentos.Any())
        {
            _context.Departamentos.Add(new Departamento { CodDepto = "01", Nombre = "CHINANDEGA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "02", Nombre = "LEON" });
            _context.Departamentos.Add(new Departamento { CodDepto = "03", Nombre = "ESTELI" });
            _context.Departamentos.Add(new Departamento { CodDepto = "04", Nombre = "MADRIZ" });
            _context.Departamentos.Add(new Departamento { CodDepto = "05", Nombre = "NUEVA SEGOVIA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "06", Nombre = "JINOTEGA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "07", Nombre = "R. ATLANTICO NORTE" });
            _context.Departamentos.Add(new Departamento { CodDepto = "08", Nombre = "MATAGALPA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "09", Nombre = "BOACO" });
            _context.Departamentos.Add(new Departamento { CodDepto = "10", Nombre = "MANAGUA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "11", Nombre = "MASAYA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "12", Nombre = "CARAZO" });
            _context.Departamentos.Add(new Departamento { CodDepto = "13", Nombre = "GRANADA" });
            _context.Departamentos.Add(new Departamento { CodDepto = "14", Nombre = "RIVAS" });
            _context.Departamentos.Add(new Departamento { CodDepto = "15", Nombre = "RIO SAN JUAN" });
            _context.Departamentos.Add(new Departamento { CodDepto = "16", Nombre = "CHONTALES" });
            _context.Departamentos.Add(new Departamento { CodDepto = "17", Nombre = "R. ATLANTICO SUR" });
            _context.Departamentos.Add(new Departamento { CodDepto = "18", Nombre = "SIN INFORMACION" });
        }

        await _context.SaveChangesAsync();
    }
}