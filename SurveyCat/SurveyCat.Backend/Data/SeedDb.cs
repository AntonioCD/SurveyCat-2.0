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
                        new Municipio { CodMun = "0101", Nombre = "EL VIEJO" },
                        new Municipio { CodMun = "0102", Nombre = "PUERTO MORAZAN" },
                        new Municipio { CodMun = "0103", Nombre = "SOMOTILLO" },
                        new Municipio { CodMun = "0104", Nombre = "SANTO TOMAS DEL NORTE" },
                        new Municipio { CodMun = "0105", Nombre = "CINCO PINOS" },
                        new Municipio { CodMun = "0106", Nombre = "SAN PEDRO DEL NORTE" },
                        new Municipio { CodMun = "0107", Nombre = "SAN FRANCISCO DEL NORTE" },
                        new Municipio { CodMun = "0108", Nombre = "VILLANUEVA" },
                        new Municipio { CodMun = "0109", Nombre = "CHINANDEGA" },
                        new Municipio { CodMun = "0110", Nombre = "POSOLTEGA" },
                        new Municipio { CodMun = "0111", Nombre = "CHICHIGALPA" },
                        new Municipio { CodMun = "0112", Nombre = "EL REALEJO" },
                        new Municipio { CodMun = "0113", Nombre = "CORINTO" }
                    }
                });

                // Departamento 02 - LEON
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "02",
                    Nombre = "LEON",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0201", Nombre = "ACHUAPA" },
                        new Municipio { CodMun = "0202", Nombre = "EL SAUCE" },
                        new Municipio { CodMun = "0204", Nombre = "SANTA ROSA DEL PEÑON" },
                        new Municipio { CodMun = "0205", Nombre = "EL JICARAL" },
                        new Municipio { CodMun = "0206", Nombre = "LARREYNAGA" },
                        new Municipio { CodMun = "0207", Nombre = "TELICA" },
                        new Municipio { CodMun = "0208", Nombre = "QUEZALGUAQUE" },
                        new Municipio { CodMun = "0209", Nombre = "LEON" },
                        new Municipio { CodMun = "0210", Nombre = "LA PAZ CENTRO" },
                        new Municipio { CodMun = "0211", Nombre = "NAGAROTE" },
                        new Municipio { CodMun = "0212", Nombre = "EL JICARAL" },
                        new Municipio { CodMun = "0213", Nombre = "MALPAISILLO" }
                    }
                });

                // Departamento 03 - ESTELI
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "03",
                    Nombre = "ESTELI",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0301", Nombre = "PUEBLO NUEVO" },
                        new Municipio { CodMun = "0302", Nombre = "CONDEGA" },
                        new Municipio { CodMun = "0303", Nombre = "ESTELI" },
                        new Municipio { CodMun = "0304", Nombre = "SAN JUAN DE LIMAY" },
                        new Municipio { CodMun = "0305", Nombre = "LA TRINIDAD" },
                        new Municipio { CodMun = "0306", Nombre = "SAN NICOLAS" }
                    }
                });

                // Departamento 04 - MADRIZ
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "04",
                    Nombre = "MADRIZ",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0401", Nombre = "SAN JOSE DE CUSMAPA" },
                        new Municipio { CodMun = "0402", Nombre = "LAS SABANAS" },
                        new Municipio { CodMun = "0403", Nombre = "SAN LUCAS" },
                        new Municipio { CodMun = "0404", Nombre = "SOMOTO" },
                        new Municipio { CodMun = "0405", Nombre = "TOTOGALPA" },
                        new Municipio { CodMun = "0406", Nombre = "YALAGUINA" },
                        new Municipio { CodMun = "0407", Nombre = "PALACAGUINA" },
                        new Municipio { CodMun = "0408", Nombre = "TELPANECA" },
                        new Municipio { CodMun = "0409", Nombre = "SAN JUAN DEL RIO COCO" }
                    }
                });

                // Departamento 05 - NUEVA SEGOVIA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "05",
                    Nombre = "NUEVA SEGOVIA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0501", Nombre = "SANTA MARIA" },
                        new Municipio { CodMun = "0502", Nombre = "MACUELIZO" },
                        new Municipio { CodMun = "0503", Nombre = "DIPILTO" },
                        new Municipio { CodMun = "0504", Nombre = "OCOTAL" },
                        new Municipio { CodMun = "0505", Nombre = "MOSONTE" },
                        new Municipio { CodMun = "0506", Nombre = "SAN FERNANDO" },
                        new Municipio { CodMun = "0507", Nombre = "JALAPA" },
                        new Municipio { CodMun = "0508", Nombre = "MURRA" },
                        new Municipio { CodMun = "0509", Nombre = "EL JICARO" },
                        new Municipio { CodMun = "0510", Nombre = "CIUDAD ANTIGUA" },
                        new Municipio { CodMun = "0511", Nombre = "QUILALI" },
                        new Municipio { CodMun = "0512", Nombre = "WIWILI DE NUEVA SEGOVIA" }
                    }
                });

                // Departamento 06 - JINOTEGA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "06",
                    Nombre = "JINOTEGA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0601", Nombre = "LA CONCORDIA" },
                        new Municipio { CodMun = "0602", Nombre = "SAN SEBASTIAN DE YALI" },
                        new Municipio { CodMun = "0603", Nombre = "SAN RAFAEL DEL NORTE" },
                        new Municipio { CodMun = "0604", Nombre = "JINOTEGA" },
                        new Municipio { CodMun = "0605", Nombre = "SANTA MARIA DE PANTASMA" },
                        new Municipio { CodMun = "0606", Nombre = "EL CUA" },
                        new Municipio { CodMun = "0607", Nombre = "WIWILI DE JINOTEGA" },
                        new Municipio { CodMun = "0608", Nombre = "SAN JOSE DE BOCAY" }
                    }
                });

                // Departamento 07 - R. ATLANTICO NORTE
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "07",
                    Nombre = "R. ATLANTICO NORTE",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0701", Nombre = "WASPAM" },
                        new Municipio { CodMun = "0702", Nombre = "PUERTO CABEZAS" },
                        new Municipio { CodMun = "0703", Nombre = "PRINZAPOLKA" },
                        new Municipio { CodMun = "0704", Nombre = "BONANZA" },
                        new Municipio { CodMun = "0705", Nombre = "SIUNA" },
                        new Municipio { CodMun = "0706", Nombre = "ROSITA" },
                        new Municipio { CodMun = "0707", Nombre = "WASLALA" },
                        new Municipio { CodMun = "0708", Nombre = "MULUKUKU" }
                    }
                });

                // Departamento 08 - MATAGALPA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "08",
                    Nombre = "MATAGALPA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0801", Nombre = "MATAGALPA" },
                        new Municipio { CodMun = "0802", Nombre = "SEBACO" },
                        new Municipio { CodMun = "0803", Nombre = "SAN ISIDRO" },
                        new Municipio { CodMun = "0804", Nombre = "CIUDAD DARIO" },
                        new Municipio { CodMun = "0805", Nombre = "TERRABONA" },
                        new Municipio { CodMun = "0806", Nombre = "SAN DIONICIO" },
                        new Municipio { CodMun = "0807", Nombre = "ESQUIPULA" },
                        new Municipio { CodMun = "0808", Nombre = "MUY MUY" },
                        new Municipio { CodMun = "0809", Nombre = "SAN RAMON" },
                        new Municipio { CodMun = "0810", Nombre = "MATIGUAS" },
                        new Municipio { CodMun = "0811", Nombre = "RIO BLANCO" },
                        new Municipio { CodMun = "0812", Nombre = "RANCHO GRANDE" },
                        new Municipio { CodMun = "0813", Nombre = "EL TUMA LA DALIA" },
                        new Municipio { CodMun = "0814", Nombre = "SAN RAMON" },
                        new Municipio { CodMun = "0815", Nombre = "DARIO" },
                        new Municipio { CodMun = "0816", Nombre = "LA DALIA" }
                    }
                });

                // Departamento 09 - BOACO
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "09",
                    Nombre = "BOACO",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "0901", Nombre = "TEUSTEPE" },
                        new Municipio { CodMun = "0902", Nombre = "SAN JOSE DE LOS REMATES" },
                        new Municipio { CodMun = "0903", Nombre = "SANTA LUCIA" },
                        new Municipio { CodMun = "0904", Nombre = "BOACO" },
                        new Municipio { CodMun = "0905", Nombre = "CAMOAPA" },
                        new Municipio { CodMun = "0906", Nombre = "SAN LORENZO" }
                    }
                });

                // Departamento 10 - MANAGUA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "10",
                    Nombre = "MANAGUA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1001", Nombre = "SAN FRANCISCO LIBRE" },
                        new Municipio { CodMun = "1002", Nombre = "TIPITAPA" },
                        new Municipio { CodMun = "1003", Nombre = "MANAGUA" },
                        new Municipio { CodMun = "1004", Nombre = "SAN RAFAEL DEL SUR" },
                        new Municipio { CodMun = "1005", Nombre = "MATEARE" },
                        new Municipio { CodMun = "1006", Nombre = "TICUANTEPE" },
                        new Municipio { CodMun = "1007", Nombre = "VILLA CARLOS FONSECA" },
                        new Municipio { CodMun = "1008", Nombre = "EL CRUCERO" },
                        new Municipio { CodMun = "1009", Nombre = "CIUDAD SANDINO" },
                        new Municipio { CodMun = "1010", Nombre = "VILLA EL CARMEN" },
                        new Municipio { CodMun = "1011", Nombre = "SIN INFORMACION" }
                    }
                });

                // Departamento 11 - MASAYA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "11",
                    Nombre = "MASAYA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1101", Nombre = "TISMA" },
                        new Municipio { CodMun = "1102", Nombre = "MASAYA" },
                        new Municipio { CodMun = "1103", Nombre = "NINDIRI" },
                        new Municipio { CodMun = "1104", Nombre = "LA CONCEPCION" },
                        new Municipio { CodMun = "1105", Nombre = "MASATEPE" },
                        new Municipio { CodMun = "1106", Nombre = "NANDASMO" },
                        new Municipio { CodMun = "1107", Nombre = "NIQUINOMO" },
                        new Municipio { CodMun = "1108", Nombre = "CATARINA" },
                        new Municipio { CodMun = "1109", Nombre = "SAN JUAN DE ORIENTE" }
                    }
                });

                // Departamento 12 - CARAZO
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "12",
                    Nombre = "CARAZO",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1201", Nombre = "SAN MARCOS" },
                        new Municipio { CodMun = "1202", Nombre = "DIRIAMBA" },
                        new Municipio { CodMun = "1203", Nombre = "DOLORES" },
                        new Municipio { CodMun = "1204", Nombre = "JINOTEPE" },
                        new Municipio { CodMun = "1205", Nombre = "EL ROSARIO" },
                        new Municipio { CodMun = "1206", Nombre = "LA PAZ CARAZO" },
                        new Municipio { CodMun = "1207", Nombre = "SANTA TERESA" },
                        new Municipio { CodMun = "1208", Nombre = "LA CONQUISTA" }
                    }
                });

                // Departamento 13 - GRANADA
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "13",
                    Nombre = "GRANADA",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1301", Nombre = "GRANADA" },
                        new Municipio { CodMun = "1302", Nombre = "DIRIOMO" },
                        new Municipio { CodMun = "1303", Nombre = "DIRIA" },
                        new Municipio { CodMun = "1304", Nombre = "NANDAIME" }
                    }
                });

                // Departamento 14 - RIVAS
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "14",
                    Nombre = "RIVAS",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1401", Nombre = "TOLA" },
                        new Municipio { CodMun = "1402", Nombre = "BELEN" },
                        new Municipio { CodMun = "1403", Nombre = "POTOSI" },
                        new Municipio { CodMun = "1404", Nombre = "BUENOS AIRES" },
                        new Municipio { CodMun = "1405", Nombre = "SAN JORGE" },
                        new Municipio { CodMun = "1406", Nombre = "RIVAS" },
                        new Municipio { CodMun = "1407", Nombre = "SAN JUAN DEL SUR" },
                        new Municipio { CodMun = "1408", Nombre = "CARDENAS" },
                        new Municipio { CodMun = "1409", Nombre = "MOYOGALPA" },
                        new Municipio { CodMun = "1410", Nombre = "ALTAGRACIA" }
                    }
                });

                // Departamento 15 - RIO SAN JUAN
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "15",
                    Nombre = "RIO SAN JUAN",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1501", Nombre = "MORRITO" },
                        new Municipio { CodMun = "1502", Nombre = "EL ALMENDRO" },
                        new Municipio { CodMun = "1503", Nombre = "SAN MIGUELITO" },
                        new Municipio { CodMun = "1504", Nombre = "SAN CARLOS" },
                        new Municipio { CodMun = "1505", Nombre = "EL CASTILLO" },
                        new Municipio { CodMun = "1506", Nombre = "SAN JUAN DEL NORTE" }
                    }
                });

                // Departamento 16 - CHONTALES
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "16",
                    Nombre = "CHONTALES",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1601", Nombre = "COMALAPA" },
                        new Municipio { CodMun = "1602", Nombre = "JUIGALPA" },
                        new Municipio { CodMun = "1603", Nombre = "LA LIBERTAD" },
                        new Municipio { CodMun = "1604", Nombre = "SANTO DOMINGO" },
                        new Municipio { CodMun = "1605", Nombre = "SAN PEDRO DEL LOVAGO" },
                        new Municipio { CodMun = "1606", Nombre = "SANTO TOMAS" },
                        new Municipio { CodMun = "1607", Nombre = "VILLA SANDINO" },
                        new Municipio { CodMun = "1608", Nombre = "ACOYAPA" },
                        new Municipio { CodMun = "1609", Nombre = "SAN FRANCISCO DE CUAPA" },
                        new Municipio { CodMun = "1610", Nombre = "EL CORAL" }
                    }
                });

                // Departamento 17 - R. ATLANTICO SUR
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "17",
                    Nombre = "R. ATLANTICO SUR",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1701", Nombre = "PAIWAS" },
                        new Municipio { CodMun = "1702", Nombre = "LA CRUZ DE RIO GRANDE" },
                        new Municipio { CodMun = "1703", Nombre = "EL RAMA" },
                        new Municipio { CodMun = "1704", Nombre = "KUKRA HILL" },
                        new Municipio { CodMun = "1705", Nombre = "LAGUNAS DE PERLAS" },
                        new Municipio { CodMun = "1706", Nombre = "MUELLE DE LOS BUELLES" },
                        new Municipio { CodMun = "1707", Nombre = "NUEVA GUINEA" },
                        new Municipio { CodMun = "1708", Nombre = "BLUEFIELDS" },
                        new Municipio { CodMun = "1709", Nombre = "CORN ISLAND" },
                        new Municipio { CodMun = "1710", Nombre = "EL ALMENDRO" },
                        new Municipio { CodMun = "1711", Nombre = "EL TORTUGUERO" },
                        new Municipio { CodMun = "1712", Nombre = "EL AYOTE" }
                    }
                });

                // Departamento 18 - SIN INFORMACION
                _context.Departamentos.Add(new Departamento
                {
                    CodDepto = "18",
                    Nombre = "SIN INFORMACION",
                    Municipios = new List<Municipio>
                    {
                        new Municipio { CodMun = "1801", Nombre = "SIN INFORMACION" }
                    }
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}