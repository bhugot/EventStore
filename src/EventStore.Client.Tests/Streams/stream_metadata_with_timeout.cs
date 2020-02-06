using System;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.Streams {
	[Trait("Category", "Network")]
	public class stream_metadata_with_timeout : IAsyncLifetime {
		private readonly Fixture _fixture;

		public stream_metadata_with_timeout() {
			_fixture = new Fixture();
		}

		[Fact]
		public async Task set_with_any_stream_revision_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();
			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.SetStreamMetadataAsync(stream, AnyStreamRevision.Any, new StreamMetadata(),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task set_with_stream_revision_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();

			await _fixture.Client.SetStreamMetadataAsync(stream, AnyStreamRevision.Any, new StreamMetadata());

			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.SetStreamMetadataAsync(stream, new StreamRevision(0), new StreamMetadata(),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task get_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();
			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.GetStreamMetadataAsync(stream,
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
