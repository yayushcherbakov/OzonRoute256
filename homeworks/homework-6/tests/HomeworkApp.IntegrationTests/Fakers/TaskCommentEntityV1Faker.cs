using AutoBogus;
using Bogus;
using HomeworkApp.Dal.Entities;
using HomeworkApp.IntegrationTests.Creators;

namespace HomeworkApp.IntegrationTests.Fakers;

public static class TaskCommentEntityV1Faker
{
    private static readonly object Lock = new();

    private static readonly Faker<TaskCommentEntityV1> Faker = new AutoFaker<TaskCommentEntityV1>()
        .RuleFor(x => x.Id, _ => Create.RandomId())
        .RuleFor(x => x.TaskId, _ => Create.RandomId())
        .RuleFor(x => x.AuthorUserId, _ => Create.RandomId())
        .RuleFor(x => x.At, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.ModifiedAt, _ => null)
        .RuleFor(x => x.DeletedAt, _ => null)
        .RuleForType(typeof(long), f => f.Random.Long(0L));

    public static TaskCommentEntityV1 Generate()
    {
        lock (Lock)
        {
            return Faker.Generate();
        }
    }
}