using System;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.Streams {
	[Trait("Category", "Network")]
	public class append_to_stream_with_timeout : IClassFixture<append_to_stream_with_timeout.Fixture> {
		private readonly Fixture _fixture;

		public append_to_stream_with_timeout(Fixture fixture) {
			_fixture = fixture;
		}

		[Fact]
		public async Task any_stream_revision_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();
			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.AppendToStreamAsync(stream, AnyStreamRevision.NoStream, _fixture.CreateTestEvents(),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		[Fact]
		public async Task stream_revision_fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();

			await _fixture.Client.AppendToStreamAsync(stream, AnyStreamRevision.Any, _fixture.CreateTestEvents());

			await Assert.ThrowsAsync<TimeoutException>(() =>
				_fixture.Client.AppendToStreamAsync(stream, new StreamRevision(0), _fixture.CreateTestEvents(),
					options => options.TimeoutAfter = TimeSpan.FromDays(-1)));
		}

		public class Fixture : EventStoreGrpcFixture {
			protected override Task Given() => Task.CompletedTask;
			protected override Task When() => Task.CompletedTask;
		}
	}
}
