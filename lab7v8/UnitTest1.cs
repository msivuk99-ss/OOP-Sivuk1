using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace lab7v8
{
    public class InMemoryRepositoryTests
    {
        private readonly InMemoryRepository<TaskItem> _repository;

        public InMemoryRepositoryTests()
        {
            _repository = new InMemoryRepository<TaskItem>();
        }

        [Fact]
        public void Add_ValidItem_ShouldAddItemToRepository()
        {
            var task = new TaskItem(1, "Test Task", "Description");

            _repository.Add(task);

            var result = _repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Title);
        }

        [Fact]
        public void Add_DuplicateId_ShouldThrowInvalidOperationException()
        {
            var task1 = new TaskItem(1, "Task 1", "Desc 1");
            var task2 = new TaskItem(1, "Task 2", "Desc 2");

            _repository.Add(task1);

            Assert.Throws<InvalidOperationException>(() => _repository.Add(task2));
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnCorrectItem()
        {
            var task = new TaskItem(1, "Task 1", "Desc 1");
            _repository.Add(task);

            var result = _repository.GetById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldReturnNull()
        {
            var result = _repository.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            _repository.Add(new TaskItem(1, "Task 1", "Desc 1"));
            _repository.Add(new TaskItem(2, "Task 2", "Desc 2"));

            var result = _repository.GetAll();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Update_ExistingItem_ShouldUpdateFields()
        {
            _repository.Add(new TaskItem(1, "Old Title", "Old Desc", false));
            var updatedTask = new TaskItem(1, "New Title", "New Desc", true);

            _repository.Update(updatedTask);

            var result = _repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public void Update_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            var task = new TaskItem(999, "Title", "Desc");

            Assert.Throws<KeyNotFoundException>(() => _repository.Update(task));
        }

        [Fact]
        public void Delete_ExistingItem_ShouldRemoveFromRepository()
        {
            _repository.Add(new TaskItem(1, "Task 1", "Desc 1"));

            _repository.Delete(1);

            Assert.Null(_repository.GetById(1));
        }

        [Fact]
        public void Delete_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Delete(999));
        }
    }
}