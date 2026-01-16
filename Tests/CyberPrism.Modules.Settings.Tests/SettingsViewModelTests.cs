using Xunit;
using Moq;
using CyberPrism.Core.Services;
using CyberPrism.Core.Models;
using CyberPrism.Modules.Settings.ViewModels;
using CyberPrism.Modules.Settings.Services;

namespace CyberPrism.Modules.Settings.Tests.ViewModels
{
    public class SettingsViewModelTests
    {
        [Fact]
        public void Settings_ShouldInitializeFromService()
        {
            // Arrange
            var serviceMock = new Mock<IAppSettingsService>();
            var dialogMock = new Mock<ISuccessDialogService>();
            var settings = new AppSettings { MachineName = "TestMachine" };
            serviceMock.Setup(s => s.Settings).Returns(settings);

            var viewModel = new SettingsViewModel(serviceMock.Object, dialogMock.Object);

            // Act
            var name = viewModel.Settings.MachineName;

            // Assert
            Assert.Equal("TestMachine", name);
        }

        [Fact]
        public void PropertyChange_ShouldNotCallSave()
        {
            // Arrange
            var serviceMock = new Mock<IAppSettingsService>();
            var dialogMock = new Mock<ISuccessDialogService>();
            var settings = new AppSettings();
            serviceMock.Setup(s => s.Settings).Returns(settings);

            var viewModel = new SettingsViewModel(serviceMock.Object, dialogMock.Object);

            // Act
            viewModel.Settings.MachineName = "NewName";

            // Assert
            serviceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void SyncCommand_ShouldCallSyncWithServerAsync_And_ShowDialog()
        {
            // Arrange
            var serviceMock = new Mock<IAppSettingsService>();
            var dialogMock = new Mock<ISuccessDialogService>();
            var settings = new AppSettings();
            serviceMock.Setup(s => s.Settings).Returns(settings);
            serviceMock.Setup(s => s.SyncWithServerAsync()).Returns(Task.CompletedTask);

            var viewModel = new SettingsViewModel(serviceMock.Object, dialogMock.Object);

            // Act
            viewModel.SyncCommand.Execute();

            // Assert
            serviceMock.Verify(s => s.SyncWithServerAsync(), Times.Once);
            dialogMock.Verify(d => d.ShowSuccessDialog(), Times.Once);
        }
    }
}

