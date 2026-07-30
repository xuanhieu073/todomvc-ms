namespace Todo.Bff.Features.Reminders.DTOs;

public class ReminderDto
{
    public string Id { get; set; }
    public string TodoId { get; set; }
    public DateTime DueAt { get; set; }
    public ReminderState State { get; set; }
    public DateTime? SnoozeUntil { get; set; }
    public DateTime? FiredAt { get; set; }
}

public enum ReminderState { Pending, Snoozed, Dismissed }