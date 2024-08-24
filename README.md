# Bloc
### A minimalistic predictable state management library for Unity inspired by https://bloclibrary.dev/


## Using the GitHub Repo
- **Open Your Unity Project:** Start by opening the Unity project where you want to install Bloc.
- **Access the Package Manager:** In Unity, go to `Window` > `Package Manager` to open the Package Manager window.
- **Add Package from Git URL:** Click on the `+` icon in the top-left corner of the Package Manager, and select `Add package from git URL....`
- **Enter the GitHub URL:** In the dialog box, enter this URL: `https://github.com/mihailt/com.mt.bloc.git`

- **Install the Package:** Click `Add` to install the package. Unity will download and install Bloc into your project.

## How to use

- **Define your state**
```
public class CounterState : IBlocState
{
public int Count { get; }

    public CounterState(int count)
    {
        Count = count;
    }
}
```

- **Define your events**
```
public struct IncrementEvent : IBlocEvent { }
public struct DecrementEvent : IBlocEvent { }
```

- **Define Bloc Class**
```
public class CounterBloc : Bloc<CounterState>
{
    public CounterBloc() : base(new CounterState(0)) { }

    protected override void HandleEvent(IBlocEvent evt)
    {
        State = evt switch
        {
            IncrementEvent _ => new CounterState(State.Count + 1),
            DecrementEvent _ => new CounterState(State.Count - 1),
            _ => State
        };
    }
}
```
- **Use it inside your MonoBehaviour or any other class**
```
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [SerializeField]
    public Text CounterText;

    private CounterBloc _bloc;

    void Start()
    {
        _bloc = new CounterBloc();

        CounterText.text = $"Count: {state.Count};
        _bloc.OnStateChanged += (_, state) => CounterText.text = $"Count: {state.Count}";
    }

    void OnDestroy() =>_bloc.Dispose();

    // Event handlers assigned to UI buttons
    public void OnIncrementButtonClicked() => _bloc.Emit(new IncrementEvent());
    public void OnDecrementButtonClicked() => _bloc.Emit(new DecrementEvent());
}
```