namespace Guardian.Domain.User.Enums
{
    /// <summary>
    /// Cargos existentes no sistema
    /// </summary>
    public enum ERoles
    {
        /// <summary>
        /// Super Administrador.
        /// </summary>
        SuperAdmin,

        /// <summary>
        /// Administrador.
        /// </summary>
        Admin,

        /// <summary>
        /// Usuário.
        /// </summary>
        User,

        /// <summary>
        /// Apenas leitura e visualização.
        /// </summary>
        ReadOnly
    }
}
