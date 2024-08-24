using MT.Bloc;
using NUnit.Framework;

namespace Tests.Runtime.Bloc
{
    internal struct EmptyTestEvent : IBlocEvent { }

    internal struct IntTestEvent : IBlocEvent
    {
        public readonly int Value;
        public IntTestEvent(int value) => Value = value;        
    }

    internal struct StringTestEvent : IBlocEvent
    {
        public readonly string Value;
        public StringTestEvent(string value) => Value = value;        
    }

    internal struct NotHandledTestEvent : IBlocEvent { }
    internal struct InitialTestState : IBlocState { }
    internal struct EmptyTestState : IBlocState { }

    internal struct IntTestState : IBlocState
    {
        public int Value;
        public IntTestState(int value) => Value = value;
    }

    internal  struct StringTestState : IBlocState
    {
        public string Value;
        public StringTestState(string value) => Value = value;
    }
    
    internal class TestBloc : Bloc<IBlocState>
    {
        public TestBloc():  base(new InitialTestState()) {}
        public TestBloc(IBlocState initialState) : base(initialState) { }

        protected override void HandleEvent(IBlocEvent evt)
        {
            State = evt switch
            {
                EmptyTestEvent _ => new EmptyTestState(),
                IntTestEvent e => new IntTestState { Value = e.Value },
                StringTestEvent e => new StringTestState { Value = e.Value },
                _ => State
            };
        }
    }
    
    public class BlocTest
    {

        [Test]
        public void Bloc_InitialState_IsCorrect()
        {
            Assert.IsTrue(new TestBloc().State is InitialTestState);
            var initialState = new EmptyTestState();
            var bloc = new TestBloc(initialState);
            Assert.AreEqual(initialState, bloc.State);
        }

        
        [Test]
        public void Bloc_HandleEvent_UpdatesStateCorrectly()
        {
            var bloc = new TestBloc();
            Assert.IsTrue(bloc.State is InitialTestState);

            bloc.Emit(new EmptyTestEvent());
            Assert.IsTrue(bloc.State is EmptyTestState);

            bloc.Emit(new IntTestEvent(5));
            Assert.IsTrue(bloc.State is IntTestState);
            Assert.AreEqual(((IntTestState) bloc.State).Value, 5);
            
            bloc.Emit(new StringTestEvent("Test"));
            Assert.IsTrue(bloc.State is StringTestState);
            Assert.AreEqual(((StringTestState) bloc.State).Value, "Test");

            bloc.Emit(new EmptyTestEvent());
            bloc.Emit(new NotHandledTestEvent());
            Assert.IsTrue(bloc.State is EmptyTestState);
        }

        
        [Test]
        public void Bloc_OnStateChanged_EventIsRaised()
        {
            var bloc = new TestBloc();
            bool eventRaised = false;

            bloc.OnStateChanged += (prev, current) => 
            {
                eventRaised = true;
                Assert.IsTrue(prev is InitialTestState);
                Assert.AreEqual(1, ((IntTestState) current).Value);
            };


            bloc.Emit(new IntTestEvent(1));
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void Bloc_OnEvent_EventIsRaised()
        {
            var bloc = new TestBloc();
            bool eventRaised = false;

            IBlocEvent receivedEvent = null; 

            bloc.OnEvent += e => 
            {
                eventRaised = true;
                receivedEvent = e; 
            };

            var testEvent = new EmptyTestEvent();
            bloc.Emit(testEvent);

            Assert.IsTrue(eventRaised);
            Assert.AreEqual(testEvent, receivedEvent);
        }        

        [Test]
        public void Bloc_Dispose_UnsubscribesEvents()
        {
            var bloc = new TestBloc();

            bool stateChangedRaised = false;
            bool eventRaised = false;

            bloc.OnStateChanged += (_, _) => stateChangedRaised = true;
            bloc.OnEvent += _ => eventRaised = true;
            
            bloc.Dispose(); 

            bloc.Emit(new EmptyTestEvent()); 
            
            Assert.IsFalse(stateChangedRaised);
            Assert.IsFalse(eventRaised);
        }
    }
}
