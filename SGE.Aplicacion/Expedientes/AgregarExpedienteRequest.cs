
namespace SGE.Aplicacion.Expedientes;

public record class AgregarExpedienteRequest(
     String caratula,
     Guid usuarioModificacion
);


