using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    public TaskCommentRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at, modified_at, deleted_at)  
select task_id, author_user_id, message, at, modified_at, deleted_at
  from UNNEST(@TaskComments)
returning id;
";

        await using var connection = await GetConnection();
        var id = await connection.QuerySingleAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskComments = new TaskCommentEntityV1[] { model }
                },
                cancellationToken: token));

        return id;
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
   set task_id = @TaskId
     , author_user_id = @AuthorUserId
     , message = @Message
     , at = @At
     , modified_at = @ModifiedAt
     , deleted_at = @DeletedAt
 where id = @Id
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AuthorUserId = model.AuthorUserId,
                    Message = model.Message,
                    At = model.At,
                    ModifiedAt = DateTimeOffset.UtcNow,
                    DeletedAt = model.DeletedAt,
                    Id = model.Id
                },
                cancellationToken: token));
    }

    public async Task SetDeleted(long taskCommentId, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
   set deleted_at = @DeletedAt
 where id = @TaskCommentId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    DeletedAt = DateTimeOffset.UtcNow,
                    TaskCommentId = taskCommentId
                },
                cancellationToken: token));
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var baseSql = @"
select id
     , task_id
     , author_user_id
     , message
     , at
     , modified_at
     , deleted_at
  from task_comments
";

        var conditions = new List<string>();
        var @params = new DynamicParameters();

        conditions.Add("task_id = @TaskId");
        @params.Add("TaskId", model.TaskId);

        if (!model.IncludeDeleted)
        {
            conditions.Add("deleted_at is null");
        }

        var filteredQuery = baseSql + $" where {string.Join(" AND ", conditions)}";
        var orderedQuery = filteredQuery + " order by at desc;";

        var cmd = new CommandDefinition(
            orderedQuery,
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskCommentEntityV1>(cmd))
            .ToArray();
    }
}