using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class UpdatesRepository : TRepository<UpdatesRepository>
    {
        public const string REPOSITORY_ID = "core.updates";
        
        // REPOSITORY PROPERTIES: -----------------------------------------------------------------
        
        public override string RepositoryID => REPOSITORY_ID;
    }
}