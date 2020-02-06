using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client.Streams;

namespace EventStore.Client {
	public partial class EventStoreClient {
		private Task<DeleteResult> TombstoneAsync(
			string streamName,
			StreamRevision expectedRevision,
			EventStoreClientOperationOptions operationOptions,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) =>
			TombstoneInternal(new TombstoneReq {
				Options = new TombstoneReq.Types.Options {
					StreamName = streamName,
					Revision = expectedRevision
				}
			}, operationOptions, userCredentials, cancellationToken);

		public Task<DeleteResult> TombstoneAsync(
			string streamName,
			StreamRevision expectedRevision,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) => TombstoneAsync(streamName, expectedRevision,
			_settings.OperationOptions, userCredentials, cancellationToken);

		public Task<DeleteResult> TombstoneAsync(
			string streamName,
			StreamRevision expectedRevision,
			Action<EventStoreClientOperationOptions> tombstoneOptions,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) {
			var options = _settings.OperationOptions.Clone();
			tombstoneOptions(options);
			return TombstoneAsync(streamName, expectedRevision, options, userCredentials, cancellationToken);
		}

		private Task<DeleteResult> TombstoneAsync(
			string streamName,
			AnyStreamRevision expectedRevision,
			EventStoreClientOperationOptions operationOptions,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) =>
			TombstoneInternal(new TombstoneReq {
				Options = new TombstoneReq.Types.Options {
					StreamName = streamName
				}
			}.WithAnyStreamRevision(expectedRevision), operationOptions, userCredentials, cancellationToken);

		public Task<DeleteResult> TombstoneAsync(
			string streamName,
			AnyStreamRevision expectedRevision,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) => TombstoneAsync(streamName, expectedRevision,
			_settings.OperationOptions, userCredentials, cancellationToken);

		public Task<DeleteResult> TombstoneAsync(
			string streamName,
			AnyStreamRevision expectedRevision,
			Action<EventStoreClientOperationOptions> operationOptions,
			UserCredentials userCredentials = default,
			CancellationToken cancellationToken = default) {
			var options = _settings.OperationOptions.Clone();
			operationOptions(options);
			return TombstoneAsync(streamName, expectedRevision, options, userCredentials, cancellationToken);
		}

		private async Task<DeleteResult> TombstoneInternal(TombstoneReq request,
			EventStoreClientOperationOptions operationOptions, UserCredentials userCredentials,
			CancellationToken cancellationToken) {
			var result = await _client.TombstoneAsync(request, RequestMetadata.Create(userCredentials),
				deadline: DeadLine.After(operationOptions.TimeoutAfter), cancellationToken);

			return new DeleteResult(new Position(result.Position.CommitPosition, result.Position.PreparePosition));
		}
	}
}
