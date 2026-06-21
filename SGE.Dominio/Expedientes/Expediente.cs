
using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;


namespace SGE.Dominio.Expedientes;

public class Expediente
{
    //utilizo encapsulamiento (private set),Propiedades con set privado para proteger el encapsulamiento
    public Guid Id { get; private set; }
    public Caratula CaratulaExpediente { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltModificacion { get; private set; }
    public EstadoExpediente Estado { get; private set; }


    //1.Constructor Privado (Centraliza lógica y asigna ID),Se usa tanto para la creación nueva como para la reconstrucción.
    private Expediente(Guid id , Caratula caratula, DateTime fechaCreacion, EstadoExpediente estado, Guid usuarioUltModificacion, DateTime ultimaFechaModificacion)
    {
        if (id == Guid.Empty)
            throw new DominioException("El Id del expediente no puede ser un Guid vacío."); // en algun momento es posible que salte esta excepcion?
        Id = id;
        CaratulaExpediente = caratula ?? throw new DominioException("La caratula es obligatoria");
        FechaCreacion = fechaCreacion;
        Estado = estado;
        UsuarioUltModificacion = usuarioUltModificacion;
        FechaUltimaModificacion = ultimaFechaModificacion;
    }


    //2.Constructor Público (NUEVOS: Autogenera ID y delega)No pide el Id, lo autogenera para garantizar autonomía.
    public Expediente(Caratula caratula):this(Guid.NewGuid(),caratula, DateTime.Now, EstadoExpediente.RecienIniciado, Guid.Empty, DateTime.Now){}


    // 3. Factory Method (Para mecanismos de persistencia),Permite "hidratar"(para no perder información, en algún momento querremos recuperarla.) un objeto con datos que vienen de la persistencia.
    public static Expediente ReconstruirExpediente(Guid id , Caratula caratula, DateTime fechaCreacion, EstadoExpediente estado, Guid usuarioUltModificacion, DateTime ultimaFechaModificacion)
    {
        return new Expediente(id,caratula, fechaCreacion, estado, usuarioUltModificacion, ultimaFechaModificacion);
    }


    //metodo para actualizar caratula , el usuario responsable y la fecha de modificación.
    public void ModificarCaratula(Caratula nuevaCaratula , Guid idUsuario)
    {
        CaratulaExpediente = nuevaCaratula ?? throw new DominioException("la caratula no puede ser nula"); //fecha 3/5, si yo mando algo del tipo caratula, es porque ya paso la excepcion, entonces no es necesario volver a preguntar si es null
        UsuarioUltModificacion = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }

    //comportamiento para actualizar el estado del expediente con metodo booleano
    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        switch (ultimaEtiqueta)
        {
            case EtiquetaTramite.EscritoPresentado:
                return false;
            case EtiquetaTramite.Notificacion:
                return false;
            case EtiquetaTramite.Despacho:
                return false;
            case EtiquetaTramite.Resolucion:
                Estado = EstadoExpediente.ConResolucion;
                UsuarioUltModificacion = idUsuario;
                FechaUltimaModificacion = DateTime.Now;
                return true;
            case EtiquetaTramite.PaseAEstudio:
                Estado = EstadoExpediente.ParaResolver;
                UsuarioUltModificacion = idUsuario;
                FechaUltimaModificacion = DateTime.Now;
                return true;
            case EtiquetaTramite.PaseAlArchivo:
                Estado = EstadoExpediente.Finalizado;
                UsuarioUltModificacion = idUsuario;
                FechaUltimaModificacion = DateTime.Now;
                return true;
            default:
                Estado = EstadoExpediente.RecienIniciado;
                FechaUltimaModificacion = DateTime.Now;
                UsuarioUltModificacion = idUsuario;
                return true;
        }
    }

    //comportamiento con el cual cambio el estado de forma manual mediante un usuario
    public void CambiarEstado(EstadoExpediente nuevoEstado, Guid idUsuario)
    {
        Estado = nuevoEstado ;
        UsuarioUltModificacion = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }

    public void SetFechamodificacion(DateTime nuevaFecha)
    {
        if(nuevaFecha < FechaCreacion)
        {
            throw new DominioException("Error : la FechaUltimaModificacion no puede ser menor que la FechaCreacion.");
        }
        FechaUltimaModificacion = nuevaFecha;
    }

    public void ActualizarUsuarioModificacion(Guid usuario)
    {
        UsuarioUltModificacion = usuario; 
    }
    public override string ToString()
    {
        return $"(Id: {Id}) - Caratula: {CaratulaExpediente} - Estado: {Estado}  ";
    }
}
