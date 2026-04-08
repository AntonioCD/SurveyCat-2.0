using Microsoft.EntityFrameworkCore;
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
        //await CheckDepartamentosAsync();
        await CheckDepartamentosFullAsync();
    }

    private async Task CheckDepartamentosFullAsync()
    {
        if (!_context.Departamentos.Any())
        {
            var countriesSQLScript = File.ReadAllText("Data\\Dpt-Mun-Bar-Com-Cas-Sec.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
        }
    }

    private async Task CheckDepartamentosAsync()
    {
        if (!_context.Departamentos.Any())
        {
            if (!_context.Departamentos.Any())
            {
                // Departamento 01 - CHINANDEGA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "01",
                    Nombre = "CHINANDEGA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0101", Nombre = "EL VIEJO" },
                        new Municipio { CodMuni = "0102", Nombre = "PUERTO MORAZAN" },
                        new Municipio { CodMuni = "0103", Nombre = "SOMOTILLO" },
                        new Municipio { CodMuni = "0104", Nombre = "SANTO TOMAS DEL NORTE" },
                        new Municipio { CodMuni = "0105", Nombre = "CINCO PINOS" },
                        new Municipio { CodMuni = "0106", Nombre = "SAN PEDRO DEL NORTE" },
                        new Municipio { CodMuni = "0107", Nombre = "SAN FRANCISCO DEL NORTE" },
                        new Municipio { CodMuni = "0108", Nombre = "VILLANUEVA" },
                        new Municipio { CodMuni = "0109", Nombre = "CHINANDEGA" },
                        new Municipio { CodMuni = "0110", Nombre = "POSOLTEGA" },
                        new Municipio { CodMuni = "0111", Nombre = "CHICHIGALPA" },
                        new Municipio { CodMuni = "0112", Nombre = "EL REALEJO" },
                        new Municipio { CodMuni = "0113", Nombre = "CORINTO" }
                    }
                });

                // Departamento 02 - LEON
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "02",
                    Nombre = "LEON",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0201", Nombre = "ACHUAPA" },
                        new Municipio { CodMuni = "0202", Nombre = "EL SAUCE" },
                        new Municipio { CodMuni = "0204", Nombre = "SANTA ROSA DEL PEÑON" },
                        new Municipio { CodMuni = "0205", Nombre = "EL JICARAL" },
                        new Municipio { CodMuni = "0206", Nombre = "LARREYNAGA" },
                        new Municipio { CodMuni = "0207", Nombre = "TELICA" },
                        new Municipio { CodMuni = "0208", Nombre = "QUEZALGUAQUE" },
                        new Municipio { CodMuni = "0209", Nombre = "LEON" },
                        new Municipio { CodMuni = "0210", Nombre = "LA PAZ CENTRO" },
                        new Municipio { CodMuni = "0211", Nombre = "NAGAROTE" },
                        new Municipio { CodMuni = "0212", Nombre = "EL JICARAL" },
                        new Municipio { CodMuni = "0213", Nombre = "MALPAISILLO" }
                    }
                });

                // Departamento 03 - ESTELI
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "03",
                    Nombre = "ESTELI",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0301", Nombre = "PUEBLO NUEVO" },
                        new Municipio { CodMuni = "0302", Nombre = "CONDEGA" },
                        new Municipio { CodMuni = "0303", Nombre = "ESTELI" },
                        new Municipio { CodMuni = "0304", Nombre = "SAN JUAN DE LIMAY" },
                        new Municipio { CodMuni = "0305", Nombre = "LA TRINIDAD" },
                        new Municipio { CodMuni = "0306", Nombre = "SAN NICOLAS" }
                    }
                });

                // Departamento 04 - MADRIZ
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "04",
                    Nombre = "MADRIZ",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0401", Nombre = "SAN JOSE DE CUSMAPA" },
                        new Municipio { CodMuni = "0402", Nombre = "LAS SABANAS" },
                        new Municipio { CodMuni = "0403", Nombre = "SAN LUCAS" },
                        new Municipio { CodMuni = "0404", Nombre = "SOMOTO" },
                        new Municipio { CodMuni = "0405", Nombre = "TOTOGALPA" },
                        new Municipio { CodMuni = "0406", Nombre = "YALAGUINA" },
                        new Municipio { CodMuni = "0407", Nombre = "PALACAGUINA" },
                        new Municipio { CodMuni = "0408", Nombre = "TELPANECA" },
                        new Municipio { CodMuni = "0409", Nombre = "SAN JUAN DEL RIO COCO" }
                    }
                });

                // Departamento 05 - NUEVA SEGOVIA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "05",
                    Nombre = "NUEVA SEGOVIA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0501", Nombre = "SANTA MARIA" },
                        new Municipio { CodMuni = "0502", Nombre = "MACUELIZO" },
                        new Municipio { CodMuni = "0503", Nombre = "DIPILTO" },
                        new Municipio { CodMuni = "0504", Nombre = "OCOTAL" },
                        new Municipio { CodMuni = "0505", Nombre = "MOSONTE" },
                        new Municipio { CodMuni = "0506", Nombre = "SAN FERNANDO" },
                        new Municipio { CodMuni = "0507", Nombre = "JALAPA" },
                        new Municipio { CodMuni = "0508", Nombre = "MURRA" },
                        new Municipio { CodMuni = "0509", Nombre = "EL JICARO" },
                        new Municipio { CodMuni = "0510", Nombre = "CIUDAD ANTIGUA" },
                        new Municipio { CodMuni = "0511", Nombre = "QUILALI" },
                        new Municipio { CodMuni = "0512", Nombre = "WIWILI DE NUEVA SEGOVIA" }
                    }
                });

                // Departamento 06 - JINOTEGA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "06",
                    Nombre = "JINOTEGA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0601", Nombre = "LA CONCORDIA" },
                        new Municipio { CodMuni = "0602", Nombre = "SAN SEBASTIAN DE YALI" },
                        new Municipio { CodMuni = "0603", Nombre = "SAN RAFAEL DEL NORTE" },
                        new Municipio { CodMuni = "0604", Nombre = "JINOTEGA" },
                        new Municipio { CodMuni = "0605", Nombre = "SANTA MARIA DE PANTASMA" },
                        new Municipio { CodMuni = "0606", Nombre = "EL CUA" },
                        new Municipio { CodMuni = "0607", Nombre = "WIWILI DE JINOTEGA" },
                        new Municipio { CodMuni = "0608", Nombre = "SAN JOSE DE BOCAY" }
                    }
                });

                // Departamento 07 - R. ATLANTICO NORTE
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "07",
                    Nombre = "R. ATLANTICO NORTE",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0701", Nombre = "WASPAM" },
                        new Municipio { CodMuni = "0702", Nombre = "PUERTO CABEZAS" },
                        new Municipio { CodMuni = "0703", Nombre = "PRINZAPOLKA" },
                        new Municipio { CodMuni = "0704", Nombre = "BONANZA" },
                        new Municipio { CodMuni = "0705", Nombre = "SIUNA" },
                        new Municipio { CodMuni = "0706", Nombre = "ROSITA" },
                        new Municipio { CodMuni = "0707", Nombre = "WASLALA" },
                        new Municipio { CodMuni = "0708", Nombre = "MULUKUKU" }
                    }
                });

                // Departamento 08 - MATAGALPA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "08",
                    Nombre = "MATAGALPA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0801", Nombre = "MATAGALPA" },
                        new Municipio { CodMuni = "0802", Nombre = "SEBACO" },
                        new Municipio { CodMuni = "0803", Nombre = "SAN ISIDRO" },
                        new Municipio { CodMuni = "0804", Nombre = "CIUDAD DARIO" },
                        new Municipio { CodMuni = "0805", Nombre = "TERRABONA" },
                        new Municipio { CodMuni = "0806", Nombre = "SAN DIONICIO" },
                        new Municipio { CodMuni = "0807", Nombre = "ESQUIPULA" },
                        new Municipio { CodMuni = "0808", Nombre = "MUY MUY" },
                        new Municipio { CodMuni = "0809", Nombre = "SAN RAMON" },
                        new Municipio { CodMuni = "0810", Nombre = "MATIGUAS" },
                        new Municipio { CodMuni = "0811", Nombre = "RIO BLANCO" },
                        new Municipio { CodMuni = "0812", Nombre = "RANCHO GRANDE" },
                        new Municipio { CodMuni = "0813", Nombre = "EL TUMA LA DALIA" },
                        new Municipio { CodMuni = "0814", Nombre = "SAN RAMON" },
                        new Municipio { CodMuni = "0815", Nombre = "DARIO" },
                        new Municipio { CodMuni = "0816", Nombre = "LA DALIA" }
                    }
                });

                // Departamento 09 - BOACO
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "09",
                    Nombre = "BOACO",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "0901", Nombre = "TEUSTEPE" },
                        new Municipio { CodMuni = "0902", Nombre = "SAN JOSE DE LOS REMATES" },
                        new Municipio { CodMuni = "0903", Nombre = "SANTA LUCIA" },
                        new Municipio { CodMuni = "0904", Nombre = "BOACO" },
                        new Municipio { CodMuni = "0905", Nombre = "CAMOAPA" },
                        new Municipio { CodMuni = "0906", Nombre = "SAN LORENZO" }
                    }
                });

                // Departamento 10 - MANAGUA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "10",
                    Nombre = "MANAGUA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1001", Nombre = "SAN FRANCISCO LIBRE" },
                        new Municipio { CodMuni = "1002", Nombre = "TIPITAPA" },
                        new Municipio { CodMuni = "1003", Nombre = "MANAGUA" },
                        new Municipio { CodMuni = "1004", Nombre = "SAN RAFAEL DEL SUR" },
                        new Municipio { CodMuni = "1005", Nombre = "MATEARE" },
                        new Municipio { CodMuni = "1006", Nombre = "TICUANTEPE" },
                        new Municipio { CodMuni = "1007", Nombre = "VILLA CARLOS FONSECA" },
                        new Municipio { CodMuni = "1008", Nombre = "EL CRUCERO" },
                        new Municipio { CodMuni = "1009", Nombre = "CIUDAD SANDINO" },
                        new Municipio { CodMuni = "1010", Nombre = "VILLA EL CARMEN" },
                        new Municipio { CodMuni = "1011", Nombre = "SIN INFORMACION" }
                    }
                });

                // Departamento 11 - MASAYA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "11",
                    Nombre = "MASAYA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1101", Nombre = "TISMA" },
                        new Municipio { CodMuni = "1102", Nombre = "MASAYA" },
                        new Municipio { CodMuni = "1103", Nombre = "NINDIRI" },
                        new Municipio { CodMuni = "1104", Nombre = "LA CONCEPCION" },
                        new Municipio { CodMuni = "1105", Nombre = "MASATEPE" },
                        new Municipio { CodMuni = "1106", Nombre = "NANDASMO" },
                        new Municipio { CodMuni = "1107", Nombre = "NIQUINOMO" },
                        new Municipio { CodMuni = "1108", Nombre = "CATARINA" },
                        new Municipio { CodMuni = "1109", Nombre = "SAN JUAN DE ORIENTE" }
                    }
                });

                // Departamento 12 - CARAZO
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "12",
                    Nombre = "CARAZO",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1201", Nombre = "SAN MARCOS" },
                        new Municipio { CodMuni = "1202", Nombre = "DIRIAMBA" },
                        new Municipio { CodMuni = "1203", Nombre = "DOLORES" },
                        new Municipio { CodMuni = "1204", Nombre = "JINOTEPE" },
                        new Municipio { CodMuni = "1205", Nombre = "EL ROSARIO" },
                        new Municipio { CodMuni = "1206", Nombre = "LA PAZ CARAZO" },
                        new Municipio { CodMuni = "1207", Nombre = "SANTA TERESA" },
                        new Municipio { CodMuni = "1208", Nombre = "LA CONQUISTA" }
                    }
                });

                // Departamento 13 - GRANADA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "13",
                    Nombre = "GRANADA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1301", Nombre = "GRANADA" },
                        new Municipio { CodMuni = "1302", Nombre = "DIRIOMO" },
                        new Municipio { CodMuni = "1303", Nombre = "DIRIA" },
                        new Municipio { CodMuni = "1304", Nombre = "NANDAIME" }
                    }
                });

                // Departamento 14 - RIVAS
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "14",
                    Nombre = "RIVAS",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1401", Nombre = "TOLA" },
                        new Municipio { CodMuni = "1402", Nombre = "BELEN" },
                        new Municipio { CodMuni = "1403", Nombre = "POTOSI" },
                        new Municipio { CodMuni = "1404", Nombre = "BUENOS AIRES" },
                        new Municipio { CodMuni = "1405", Nombre = "SAN JORGE" },
                        new Municipio { CodMuni = "1406", Nombre = "RIVAS" },
                        new Municipio { CodMuni = "1407", Nombre = "SAN JUAN DEL SUR" },
                        new Municipio { CodMuni = "1408", Nombre = "CARDENAS" },
                        new Municipio { CodMuni = "1409", Nombre = "MOYOGALPA" },
                        new Municipio { CodMuni = "1410", Nombre = "ALTAGRACIA" }
                    }
                });

                // Departamento 15 - RIO SAN JUAN
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "15",
                    Nombre = "RIO SAN JUAN",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1501", Nombre = "MORRITO" },
                        new Municipio { CodMuni = "1502", Nombre = "EL ALMENDRO" },
                        new Municipio { CodMuni = "1503", Nombre = "SAN MIGUELITO" },
                        new Municipio { CodMuni = "1504", Nombre = "SAN CARLOS" },
                        new Municipio { CodMuni = "1505", Nombre = "EL CASTILLO" },
                        new Municipio { CodMuni = "1506", Nombre = "SAN JUAN DEL NORTE" }
                    }
                });

                // Departamento 16 - CHONTALES
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "16",
                    Nombre = "CHONTALES",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1601", Nombre = "COMALAPA" },
                        new Municipio { CodMuni = "1602", Nombre = "JUIGALPA" },
                        new Municipio { CodMuni = "1603", Nombre = "LA LIBERTAD" },
                        new Municipio { CodMuni = "1604", Nombre = "SANTO DOMINGO" },
                        new Municipio { CodMuni = "1605", Nombre = "SAN PEDRO DEL LOVAGO" },
                        new Municipio { CodMuni = "1606", Nombre = "SANTO TOMAS" },
                        new Municipio { CodMuni = "1607", Nombre = "VILLA SANDINO" },
                        new Municipio { CodMuni = "1608", Nombre = "ACOYAPA" },
                        new Municipio { CodMuni = "1609", Nombre = "SAN FRANCISCO DE CUAPA" },
                        new Municipio { CodMuni = "1610", Nombre = "EL CORAL" }
                    }
                });

                // Departamento 17 - R. ATLANTICO SUR
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "17",
                    Nombre = "R. ATLANTICO SUR",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1701", Nombre = "PAIWAS" },
                        new Municipio { CodMuni = "1702", Nombre = "LA CRUZ DE RIO GRANDE" },
                        new Municipio { CodMuni = "1703", Nombre = "EL RAMA" },
                        new Municipio { CodMuni = "1704", Nombre = "KUKRA HILL" },
                        new Municipio { CodMuni = "1705", Nombre = "LAGUNAS DE PERLAS" },
                        new Municipio { CodMuni = "1706", Nombre = "MUELLE DE LOS BUELLES" },
                        new Municipio { CodMuni = "1707", Nombre = "NUEVA GUINEA" },
                        new Municipio { CodMuni = "1708", Nombre = "BLUEFIELDS" },
                        new Municipio { CodMuni = "1709", Nombre = "CORN ISLAND" },
                        new Municipio { CodMuni = "1710", Nombre = "EL ALMENDRO" },
                        new Municipio { CodMuni = "1711", Nombre = "EL TORTUGUERO" },
                        new Municipio { CodMuni = "1712", Nombre = "EL AYOTE" }
                    }
                });

                // Departamento 18 - SIN INFORMACION
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "18",
                    Nombre = "SIN INFORMACION",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMuni = "1801", Nombre = "SIN INFORMACION" }
                    }
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}