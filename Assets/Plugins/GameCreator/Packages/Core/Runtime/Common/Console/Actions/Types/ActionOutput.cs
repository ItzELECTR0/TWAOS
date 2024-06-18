using System;

namespace GameCreator.Runtime.Console
{
    public class ActionOutput : TAction<Output>
    {
        public ActionOutput(string name, string description, Func<string, Output> method) 
            : base(name, description, method)
        { }
    }
}