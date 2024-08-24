using System;
using System.Collections.Generic;

namespace MT.Bloc
{
    public interface IBlocState { }
    public interface IBlocEvent { }

    public abstract class Bloc<TState>: IDisposable 
        where TState : IBlocState
    {
        private TState _state;

        public event Action<TState, TState> OnStateChanged;
        public event Action<IBlocEvent> OnEvent;

        public TState State
        {
            get => _state;
            protected set
            {
                if (!EqualityComparer<TState>.Default.Equals(_state, value))
                {
                    var prev = _state;
                    _state = value;
                    OnStateChanged?.Invoke(prev, value);
                }
            }
        }

        protected Bloc(TState initialState) => _state = initialState;
        protected abstract void HandleEvent(IBlocEvent evt);
        
        public void Emit(IBlocEvent evt)
        {
            HandleEvent(evt);
            OnEvent?.Invoke(evt);
        }

        public virtual void Dispose()
        {
            OnStateChanged = null;
            OnEvent = null;
        }
    }
}