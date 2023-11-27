using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
    }

    [Fact]
    public async Task Add_SingleTaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        // Act
        var createdTaskCommentId = await _repository.Add(taskComment, default);

        // Asserts
        createdTaskCommentId.Should().BePositive();
    }

    [Fact]
    public async Task Update_SingleComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);

        var updatedTaskComment = taskComment with
        {
            Id = taskCommentId,
            Message = "Check message"
        };

        // Act
        await _repository.Update(updatedTaskComment, default);

        // Asserts
        var results = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = false
        }, default);

        results.Should().HaveCount(1);

        var comment = results.Single();

        comment.Id.Should().Be(taskCommentId);
        comment.Message.Should().Be("Check message");
    }

    [Fact]
    public async Task SetDeleted_NotDeletedComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);

        // Act
        await _repository.SetDeleted(taskCommentId, default);

        // Asserts
        var results = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = true
        }, default);

        results.Should().HaveCount(1);

        var comment = results.Single();

        comment.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_SingleTaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);

        // Act
        var results = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = true
        }, default);

        // Asserts
        results.Should().HaveCount(1);
    }

    [Fact]
    public async Task Get_SingleDeletedComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();

        var taskCommentId = await _repository.Add(taskComment, default);

        // Act
        await _repository.SetDeleted(taskCommentId, default);

        // Asserts
        var results = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.TaskId,
            IncludeDeleted = true
        }, default);

        results.Should().HaveCount(1);

        var comment = results.Single();

        comment.DeletedAt.Should().NotBeNull();
    }
}