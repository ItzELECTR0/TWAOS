using System;

namespace GameCreator.Editor.Search
{
    internal class Document
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public int DocumentId { get; }
        public Type Type { get; }

        public int Boost => Favorites.IsFavorite(this.Type) ? Favorites.BOOST : 1;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Document(Type type)
        {
            this.DocumentId = IdProvider.DocumentId;
            this.Type = type;
        }
    }
}