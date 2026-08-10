using System.Reactive.Subjects;
using Todo.Bff.Features.Reminders;

public class RxEventBroker<T> : IObservable<T>
{
    private readonly Subject<T> _subject = new Subject<T>();

    public IDisposable Subscribe(IObserver<T> observer)
    {
        return _subject.Subscribe(observer);
    }

    public void Publish(T data)
    {
        _subject.OnNext(data);
    }
}

public class Queue1Broker : RxEventBroker<List<PendingReminderDto>> { }
public class Queue2Broker : RxEventBroker<List<PendingReminderDto>> { }