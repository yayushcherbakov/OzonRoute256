﻿namespace HomeworkApp.Dal.Entities;

public record TaskCommentEntityV1
{
    public long Id { get; init; }

    public required long TaskId { get; init; }

    public required long AuthorUserId { get; init; }

    public required string Message { get; init; }

    public required DateTimeOffset At { get; init; }

    public DateTimeOffset? ModifiedAt { get; init; }

    public DateTimeOffset? DeletedAt { get; init; }
}