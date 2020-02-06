using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EventStore.Client.Streams {
	[Trait("Category", "Network")]
	public class read_stream_with_timeout : IClassFixture<read_stream_with_timeout.Fixture> {
		private readonly Fixture _fixture;

		public read_stream_with_timeout(Fixture fixture) {
			_fixture = fixture;
		}

		[Fact]
		public async Task fails_when_operation_expired() {
			var stream = _fixture.GetStreamName();

			var testEvents = _fixture.CreateTestEvents(10).ToArray();

			await _fixture.Client.AppendToStreamAsync(stream, AnyStreamRevision.NoStream, testEvents);

			await Assert.ThrowsAsync<TimeoutException>(() => _fixture.Client
				.ReadStreamAsync(Direction.Backwards, stream, StreamRevision.End, 1,
					options => options.TimeoutAfter = TimeSpan.FromDays(-1))
				.ToArrayAsync().AsTask());
		}

		public class Fixture : EventStoreGrpcFixture {
			protected override Task Given() => Task.CompletedTask;
			protected override Task When() => Task.CompletedTask;
		}
	}
}
