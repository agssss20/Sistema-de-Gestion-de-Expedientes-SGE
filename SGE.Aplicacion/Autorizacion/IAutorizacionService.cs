
namespace SGE.Aplicacion.Autorizacion;

public interface IAutorizacionService
{
    bool TienePermiso(Guid usuario, Permiso permiso);
}
