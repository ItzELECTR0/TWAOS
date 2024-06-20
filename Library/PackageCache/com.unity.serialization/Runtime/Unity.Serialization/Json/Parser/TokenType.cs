namespace Unity.Serialization.Json
{
    /// <summary>
    /// The <see cref="TokenType"/> is used to describe the high level structure of a data tree.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Unknown token type. Usually this means the token is not initialized.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The token holds a reference to all characters between object characters '{'..'}'.
        /// </summary>
        /// <remarks>
        /// This includes the "begin" and "end" object characters.
        /// </remarks>
        Object,

        /// <summary>
        /// The token holds a reference to all characters between array characters '['..']'.
        ///
        /// @NOTE This includes the "begin" and "end" array characters.
        /// </summary>
        Array,

        /// <summary>
        /// The token holds a reference to all characters between string characters '"'..'"'.
        /// </summary>
        String,

        /// <summary>
        /// Holds a reference to characters that represent any value that does not fit into the above categories.
        /// </summary>
        Primitive,
        
        /// <summary>
        /// The token holds a reference to all characters of a comment block.
        /// </summary>
        Comment
    }
}