namespace CleanArch.Domain.Users;

public static class UserMessages
{
    public const string NoUsersFound = "No se encontraron usuarios.";
    public const string UserNotFound = "El usuario no existe.";
    public const string UserCreated = "Usuario creado exitosamente.";
    public const string UserUpdated = "Usuario actualizado exitosamente.";
    public const string UserDeleted = "Usuario eliminado exitosamente.";
    public const string UserNoMatch = "Error, el id no coincide.";
    public const string UserCreatedError = "Error al crear el usuario.";
    public const string UserUpdatedError = "Error al actualizar el usuario con ID {0}";
    public const string UserDeletedError = "Error al eliminar el usuario con ID {0}";
    public const string UsernameAlreadyExists = "El nombre de usuario ya está en uso.";
    public const string EmailAlreadyExists = "El correo electrónico ya está en uso.";

    public const string InvalidCredentials = "Usuario o contraseña incorrectos.";    
}
