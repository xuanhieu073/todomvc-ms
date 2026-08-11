using System.Reactive.Subjects;

namespace Todo.Bff.Features.Reminders;

public class RxEventBroker<T> : IObservable<T>
{
    private readonly Subject<T> _subject = new();

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _subject.Subscribe(observer);
    }

    public void Publish(T data)
    {
        _subject.OnNext(data);
    }
}