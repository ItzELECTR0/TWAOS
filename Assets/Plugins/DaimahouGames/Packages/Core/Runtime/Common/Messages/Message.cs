using System;
using DaimahouGames.Runtime.Core;

namespace DaimahouGames.Runtime.Core
{
    public class Message
    {
        private event Action EventMessage;

        public void Send()
        {
            EventMessage?.Invoke();
        }
        
        public MessageReceipt Subscribe(Action action)
        {
            EventMessage += action;
            return new MessageReceipt(() => EventMessage -= action);
        }
    }
    
    public class Message<T>
    {
        private event Action<T> EventMessage;

        public void Send(T data)
        {
            EventMessage?.Invoke(data);
        }
        
        public MessageReceipt Subscribe(Action<T> action)
        {
            EventMessage += action;
            return new MessageReceipt(() => EventMessage -= action);
        }

        public MessageReceipt Subscribe<TCondition>(Action<TCondition> action)
        {
            var conditionalAction = new Action<T>(arg =>
            {
                if (arg is TCondition conditional) action(conditional);
            });

            EventMessage += conditionalAction;
            return new MessageReceipt(() => EventMessage -= conditionalAction);
        }
    }
}