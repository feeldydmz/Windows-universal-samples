using System;
using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger
{
    public class MessageCenter
    {
        private readonly Dictionary<Type, List<Delegate>> _dictionary =
            new Dictionary<Type, List<Delegate>>();

        private MessageCenter()
        {
        }

        public static MessageCenter Instance { get; } = new MessageCenter();

        public void Regist<T>(OnMessageReceived<T> receiver) where T : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(T)))
                _dictionary[typeof(T)] = new List<Delegate>();
            var list = _dictionary[typeof(T)];
            list.Add(receiver);
        }

        public void Regist<TMessage, TResponse>(RequestToResponse<TMessage, TResponse> receiver)
            where TMessage : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(TMessage)))
                _dictionary[typeof(TMessage)] = new List<Delegate>();
            var list = _dictionary[typeof(TMessage)];
            list.Clear();
            list.Add(receiver);
        }

        public void Unregist<T>(OnMessageReceived<T> receiver) where T : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(T)))
                throw new InvalidOperationException();
            var list = _dictionary[typeof(T)];
            list.Remove(receiver);
        }

        public void Unregist<TMessage, TResponse>(RequestToResponse<TMessage, TResponse> receiver)
            where TMessage : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(TMessage)))
                throw new InvalidOperationException();
            var list = _dictionary[typeof(TMessage)];
            list.Remove(receiver);
        }

        public bool Send<T>(T message) where T : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(T)))
                return false;
            var list = _dictionary[typeof(T)];
            foreach (var @delegate in list)
                (@delegate as OnMessageReceived<T>)?.Invoke(message);
            return true;
        }

        public TResponse Send<TMessage, TResponse>(TMessage message) where TMessage : IMessage
        {
            if (!_dictionary.ContainsKey(typeof(TMessage)))
                return default(TResponse);
            var list = _dictionary[typeof(TMessage)];
            foreach (var @delegate in list)
            {
                var func = @delegate as RequestToResponse<TMessage, TResponse>;

                if (func != null)
                    return func.Invoke(message);
            }
            return default(TResponse);
        }
    }
}