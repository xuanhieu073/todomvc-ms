public record StatsOverviewDto(
    int Total,
    int Active,
    int Completed,
    int Overdue,           // DueAt < now && !IsCompleted
    int CompletedToday,
    int CompletedThisWeek,
    double CompletionRate,             // Completed / Total
    IReadOnlyList<DailyCountDto> CompletedByDay);  // 7 ngày gần nhất

public record DailyCountDto(DateOnly Date, int Count);