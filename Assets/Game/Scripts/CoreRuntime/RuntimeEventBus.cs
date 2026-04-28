using System;
using System.Collections.Generic;

namespace Game.CoreRuntime
{
    public interface IGameEvent
    {
        DateTime TimestampUtc { get; }
    }

    public interface IEventBus
    {
        void Publish<T>(T gameEvent) where T : struct, IGameEvent;
        IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGameEvent;
        IReadOnlyList<IGameEvent> GetRecentEvents();
    }

    public sealed class RuntimeEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> subscribers = new();
        private readonly Queue<IGameEvent> replayQueue = new();
        private readonly int replayCapacity;

        public RuntimeEventBus(int replayCapacity = 256)
        {
            this.replayCapacity = Math.Max(8, replayCapacity);
        }

        public void Publish<T>(T gameEvent) where T : struct, IGameEvent
        {
            replayQueue.Enqueue(gameEvent);
            while (replayQueue.Count > replayCapacity)
            {
                replayQueue.Dequeue();
            }

            Type key = typeof(T);
            if (!subscribers.TryGetValue(key, out List<Delegate> handlers))
            {
                return;
            }

            for (int i = 0; i < handlers.Count; i++)
            {
                if (handlers[i] is Action<T> typedHandler)
                {
                    typedHandler.Invoke(gameEvent);
                }
            }
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            Type key = typeof(T);
            if (!subscribers.TryGetValue(key, out List<Delegate> handlers))
            {
                handlers = new List<Delegate>();
                subscribers[key] = handlers;
            }

            handlers.Add(handler);
            return new Subscription(() => handlers.Remove(handler));
        }

        public IReadOnlyList<IGameEvent> GetRecentEvents()
        {
            return new List<IGameEvent>(replayQueue);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly Action onDispose;
            private bool isDisposed;

            public Subscription(Action onDispose)
            {
                this.onDispose = onDispose;
            }

            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                isDisposed = true;
                onDispose?.Invoke();
            }
        }
    }
}
