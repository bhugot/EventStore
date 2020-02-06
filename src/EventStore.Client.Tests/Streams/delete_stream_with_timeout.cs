using System;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.Streams {
	[Trait("Category", "Network")]
	public class deleting_stream_with_timeout : IAsyncLifetime {
		private readonly Fixture _fixture;

		public deleting_stream_with_timeout() {
			_fixture = new Fixture();
		}

		[Fact]
		public async Task any_stream_revision_soft_delete_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();
			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.SoftDeleteAsync(stream, AnyStreamRevision.NoStream,
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task stream_revision_soft_delete_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();

			await _fixture.Client.AppendToStreamAsync(stream, AnyStreamRevision.NoStream, _fixture.CreateTestEvents());

			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.SoftDeleteAsync(stream, new StreamRevision(0),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task any_stream_revision_tombstoning_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();
			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.TombstoneAsync(stream, AnyStreamRevision.NoStream,
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task stream_revision_tombstoning_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();

			await _fixture.Client.AppendToStreamAsync(stream, AnyStreamRevision.NoStream, _fixture.CreateTestEvents());

			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.TombstoneAsync(stream, new StreamRevision(0),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		public class Fixture : EventStoreGrpcFixture {
			protected override Task Given() => Task.CompletedTask;
			protected override Task When() => Task.CompletedTask;
		}

		public Task InitializeAsync() {
			return _fixture.InitializeAsync();
		}

		public Task DisposeAsync() {
			return _fixture.DisposeAsync();
		}
	}
}
