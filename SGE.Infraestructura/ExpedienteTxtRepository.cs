
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;

namespace SGE.Infraestructura;

public class ExpedienteTxtRepository: IExpedienteRepository
{
    private readonly string _nombreArchivo = "Expediente.txt";
    public void AgregarExpediente(Expediente Exp)
    {
    // Guardamos cada dato en una línea nueva.
    using var sw = new StreamWriter(_nombreArchivo, true);
    sw.WriteLine(Exp.Id);
    sw.WriteLine(Exp.CaratulaExpediente);
    sw.WriteLine(Exp.FechaCreacion);
    sw.WriteLine(Exp.Estado);
    sw.WriteLine(Exp.UsuarioUltModificacion);
    sw.WriteLine(Exp.FechaUltimaModificacion);
    }

    public IEnumerable<Expediente> ListarTodosLosExpedientes()
    {
        var resultado = new List<Expediente>();

        if (!File.Exists(_nombreArchivo))
            return resultado;
    
        using var sr = new StreamReader(_nombreArchivo);
    
        while (!sr.EndOfStream)
        {
            var IdStr = sr.ReadLine() ?? "";
            var CaratulaExpedienteStr = sr.ReadLine() ?? "";
            var fechaCreacionStr = sr.ReadLine() ?? "0";
            var estadoStr = sr.ReadLine() ?? "";
            var UsuarioUltModificacion = sr.ReadLine() ?? "";
            var UltimaFechaModificacionStr = sr.ReadLine() ?? "0";

            var id = Guid.Parse(IdStr);
            var caratulaVO = new Caratula(CaratulaExpedienteStr);
            var fechaCreacionVO = DateTime.Parse(fechaCreacionStr);
            var estadoVO = Enum.Parse<EstadoExpediente>(estadoStr);
            var usuarioUltModificacionVO = Guid.Parse(UsuarioUltModificacion);
            var ultimaFechaModificacionVO = DateTime.Parse(UltimaFechaModificacionStr);

            var expediente = Expediente.ReconstruirExpediente(id, caratulaVO, fechaCreacionVO, estadoVO, usuarioUltModificacionVO, ultimaFechaModificacionVO);
            resultado.Add(expediente);
        }
    return resultado;
    }

    public Expediente? obtenerExpedientePorId(Guid id)
    {
        var expediente = ListarTodosLosExpedientes().FirstOrDefault(e => e.Id == id);
        return expediente;
    }

    public void ExpedienteBaja(Guid idBaja)
    {
        var expedientes = ListarTodosLosExpedientes().ToList();
        var expediente = expedientes.FirstOrDefault(e => e.Id == idBaja);
        if (expediente == null)
            throw new RepositoryException($"No se encontró el expediente con Id {idBaja} para dar de baja.");
        expedientes.Remove(expediente);
        // Reescribimos el archivo sin el expediente dado de baja
        using var sw = new StreamWriter(_nombreArchivo, false);
        foreach (var exp in expedientes)
        {
            sw.WriteLine(exp.Id);
            sw.WriteLine(exp.CaratulaExpediente);
            sw.WriteLine(exp.FechaCreacion);
            sw.WriteLine(exp.Estado);
            sw.WriteLine(exp.UsuarioUltModificacion);
            sw.WriteLine(exp.FechaUltimaModificacion);
        }
    }

    public void ModificarExpediente(Expediente expModificacion)
    {
        var expedientes = ListarTodosLosExpedientes().ToList();
        var index = expedientes.FindIndex(e => e.Id == expModificacion.Id);
        if (index == -1)
            throw new RepositoryException($"No se encontró el expediente con Id {expModificacion.Id} para modificar.");
        expedientes[index] = expModificacion;
        // Reescribimos el archivo con la modificación
        using var sw = new StreamWriter(_nombreArchivo, false);
        foreach (var exp in expedientes)
        {
            sw.WriteLine(exp.Id);
            sw.WriteLine(exp.CaratulaExpediente);
            sw.WriteLine(exp.FechaCreacion);
            sw.WriteLine(exp.Estado);
            sw.WriteLine(exp.UsuarioUltModificacion);
            sw.WriteLine(exp.FechaUltimaModificacion);
        }
    }
}