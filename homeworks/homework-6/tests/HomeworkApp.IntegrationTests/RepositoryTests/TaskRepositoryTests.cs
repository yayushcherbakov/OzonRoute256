using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);

        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }

    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);

        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();

        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);

        // Act
        await _repository.Assign(assign, default);

        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        results.Should().HaveCount(1);
        var task = results.Single();

        expectedTask = expectedTask with { Status = assign.Status };
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task GetSubTasksInStatus_SingleSubtask_Success()
    {
        // Arrange
        var parentTasks = TaskEntityV1Faker.Generate();
        var parentTaskIds = await _repository.Add(parentTasks, default);

        var parentTaskId = parentTaskIds.Single();

        var childTasks = TaskEntityV1Faker.Generate()
            .Select(x => x.WithParentTaskId(parentTaskId).WithStatus(3))
            .ToArray();

        var childTaskIds = await _repository.Add(childTasks, default);

        // Act
        var results = await _repository.GetSubTasksInStatus
        (
            parentTaskId,
            new int[] { 3 },
            default
        );

        // Asserts
        results.Should().HaveCount(1);
        var subTask = results.Single();

        subTask.TaskId.Should().Be(childTaskIds.Single());
        subTask.Status.Should().Be(3);
    }
    
    [Fact]
    public async Task GetSubTasksInStatus_MultipleSubtask_Success()
    {
        // Arrange
        var parentTasks = TaskEntityV1Faker.Generate();
        var parentTaskIds = await _repository.Add(parentTasks, default);
        var parentTaskId = parentTaskIds.Single();

        var childTasks = TaskEntityV1Faker.Generate(3)
            .Select(x => x.WithParentTaskId(parentTaskId).WithStatus(3))
            .ToArray();

        var childTaskIds = await _repository.Add(childTasks, default);

        // Act
        var results = await _repository.GetSubTasksInStatus
        (
            parentTaskId,
            new int[] { 3 },
            default
        );

        // Asserts
        results.Should().HaveCount(3);
    }
    
    [Fact]
    public async Task GetSubTasksInStatus_MultipleSubtaskWithSeveralStatuses_Success()
    {
        // Arrange
        var parentTasks = TaskEntityV1Faker.Generate();
        var parentTaskIds = await _repository.Add(parentTasks, default);
        var parentTaskId = parentTaskIds.Single();

        var childTasks = TaskEntityV1Faker.Generate(3)
            .Select(x => x.WithParentTaskId(parentTaskId).WithStatus(3))
            .ToArray();

        await _repository.Add(childTasks, default);
        
        var childTasksWithAnotherStatuses = TaskEntityV1Faker.Generate(3)
            .Select(x => x.WithParentTaskId(parentTaskId).WithStatus(4))
            .ToArray();

        await _repository.Add(childTasksWithAnotherStatuses, default);

        // Act
        var results = await _repository.GetSubTasksInStatus
        (
            parentTaskId,
            new int[] { 3 },
            default
        );

        // Asserts
        results.Should().HaveCount(3);
    }
}